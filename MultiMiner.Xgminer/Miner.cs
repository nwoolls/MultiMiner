using MultiMiner.Xgminer.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using MultiMiner.Xgminer.Data;

namespace MultiMiner.Xgminer
{
    public class Miner
    {
        //events
        // delegate declaration 
        public delegate void LogLaunchHandler(object sender, LogLaunchArgs ea);
        public delegate void LaunchFailedHandler(object sender, LaunchFailedArgs ea);
        public delegate void AuthenticationFailedHandler(object sender, AuthenticationFailedArgs ea);

        // event declaration 
        public event LogLaunchHandler LogLaunch;
        public event LaunchFailedHandler LaunchFailed;
        public event AuthenticationFailedHandler AuthenticationFailed;

        private readonly Data.Configuration.Miner minerConfiguration;

        public Miner(Data.Configuration.Miner minerConfig)
        {
            this.minerConfiguration = minerConfig;
        }

        //uses --ndevs, returns platform information
        public List<Device> EnumerateDevices()
        {
            string arguments = MinerParameter.EnumerateDevices;
            bool redirectOutput = true;

            Process minerProcess = StartMinerProcess(arguments, redirectOutput);

            List<string> output = new List<string>();

            while (!minerProcess.StandardOutput.EndOfStream)
            {
                string line = minerProcess.StandardOutput.ReadLine();
                output.Add(line);
            }

            List<Device> result = new List<Device>();
            EnumerateDevicesParser.ParseTextForDevices(output, result);

            return result;
        }

        //uses -d?, returns driver information
        public List<Device> ListDevices(bool prettyNames = false)
        {
            string arguments = MinerParameter.DeviceList;
            bool redirectOutput = true;

            string serialArg = GetListSerialArguments();

            arguments = String.Format("{0} {1}", arguments, serialArg);

            //include Scrypt ASICs
            arguments = String.Format("{0} {1}", arguments, MinerParameter.Scrypt);

            //include the args specified by the user so we pickup manual devices (e.g. Avalon)
            arguments = String.Format("{0} {1}", arguments, minerConfiguration.ScanArguments);

            //ADL mismatch with OCL can cause an error / exception, disable ADL when enumerating devices
            //user can then disable for mining in-app using settings
            //this also prevents nice names for GPUs
            //arguments = arguments + " --no-adl";
            
            //this must be done async, with 70+ devices doing this synchronous
            //locks up the process
            Process minerProcess = StartMinerProcess(arguments, redirectOutput, false, "", false);
            List<string> output = new List<string>();
            minerProcess.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    string s = e.Data;
                    output.Add(s);
                }
            };

            minerProcess.Start();

            minerProcess.BeginOutputReadLine();
            //calling BeginErrorReadLine here is *required* on at least one user's machine
            minerProcess.BeginErrorReadLine();

            //wait 5 minutes - scans may take a long time
            minerProcess.WaitForExit(5 * 60 * 1000);

            List<Device> result = new List<Device>();
            
            DeviceListParser.ParseTextForDevices(output, result);

            if (prettyNames)
                MakeNamesPretty(result);

            return result;
        }

        private readonly string[] genericNames = 
        { 
            "Device",
            "CP2102 USB to UART Bridge Controller by Silicon Labs"
        };

        private void MakeNamesPretty(List<Device> devices)
        {
            foreach (Device device in devices)
                if (genericNames.Contains(device.Name) && !String.IsNullOrEmpty(device.Driver))
                {
                    if (device.Driver.Equals("erupter", StringComparison.OrdinalIgnoreCase))
                        device.Name = "Block Erupter";
                    else if (device.Driver.Equals("antminer", StringComparison.OrdinalIgnoreCase))
                        device.Name = "AntMiner U1";
                    else
                        device.Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(device.Driver);
                }
        }

        private string GetListSerialArguments()
        {
            string serialArg = String.Empty;

            if (minerConfiguration.DisableUsbProbe)
                serialArg = MinerParameter.ScanSerialAuto;
            else
                //the order specified here is so that Block Erupters use the Erupter driver without interfering with Nanofury detection
                serialArg = MinerParameter.ScanSerialNanofuryAll + " " + MinerParameter.ScanSerialErupterAll + " " + MinerParameter.ScanSerialAll;

            if (!minerConfiguration.DisableGpu)
                //openCL disabled by default in bfgminer 3.3.0+
                serialArg = String.Format("{0} {1}", serialArg, MinerParameter.ScanSerialOpenCL);

            serialArg = String.Format("{0} {1}", serialArg, MinerParameter.ScanSerialCpu);

            return serialArg;
        }

        [Obsolete("DeviceList is deprecated, please use ListDevices instead.")]
        public List<Device> DeviceList()
        {
            return ListDevices();
        }

        public Process Launch(string reason = "")
        {
            bool redirectOutput = false;
            bool ensureProcessStarts = true;

            //otherwise cgminer output shows under *nix in the terminal
            //DONT do it for Windows though or cgminer will close after opening

            //commented out for now - seems to eventually cause issues under .nix too
            //the miner keeps mining but Accepted shares stop updating
            //if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
            //    redirectOutput = true;

            string arguments = minerConfiguration.LaunchArguments;

            string serialArg = MinerParameter.ScanSerialNoAuto;
            arguments = String.Format("{0} {1}", arguments, serialArg);

            if (minerConfiguration.StratumProxy)
            {
                if (minerConfiguration.StratumProxyPort > 0)
                    arguments = String.Format("{0} --http-port {1}", arguments, minerConfiguration.StratumProxyPort);
                if (minerConfiguration.StratumProxyStratumPort > 0)
                    arguments = String.Format("{0} --stratum-port {1}", arguments, minerConfiguration.StratumProxyStratumPort);
            }

            foreach (MiningPool pool in minerConfiguration.Pools)
            {
                string argument;
                //trim Host to ensure proper formatting
                if (pool.QuotaEnabled)
                    argument = string.Format("--quota \"{3};{0}:{1}\" -u {2}", pool.Host.Trim(), pool.Port, pool.Username, pool.Quota);
                else
                    argument = string.Format("-o {0}:{1} -u {2}", pool.Host.Trim(), pool.Port, pool.Username);

                //some pools do not require a password
                //but the miners require some password
                string password = "\"\"";
                if (!String.IsNullOrEmpty(pool.Password))
                    password = pool.Password;

                argument = String.Format("{0} -p {1}", argument, password);

                if (!String.IsNullOrEmpty(pool.MinerFlags))
                    argument = String.Format("{0} {1}", argument, pool.MinerFlags);

                arguments = string.Format("{0} {1}", arguments, argument);
            }

            if (minerConfiguration.DeviceDescriptors.Exists(d => d.Kind == DeviceKind.GPU))
                arguments = String.Format("{0} {1}", arguments, MinerParameter.ScanSerialOpenCL);

            if (minerConfiguration.DeviceDescriptors.Exists(d => d.Kind == DeviceKind.CPU))
                arguments = String.Format("{0} {1}", arguments, MinerParameter.ScanSerialCpu);

            foreach (DeviceDescriptor deviceDescriptor in minerConfiguration.DeviceDescriptors)
            {
                if (deviceDescriptor.Kind == DeviceKind.GPU)
                    arguments = string.Format("{0} -d OCL{1}", arguments, deviceDescriptor.RelativeIndex);
                else if (deviceDescriptor.Kind == DeviceKind.CPU)
                    arguments = string.Format("{0} -d CPU{1}", arguments, deviceDescriptor.RelativeIndex);
                else if (deviceDescriptor.Kind == DeviceKind.USB)
                {
                    //hashbuster may not have path - but does in later versions of bfgminer
                    //leaving as a code-path (for now)
                    if (deviceDescriptor.Driver.Equals("hashbusterusb", StringComparison.OrdinalIgnoreCase))
                    {
                        //FIXED IN BFGMINER 3.9
                        //SAFE TO REMOVE THIS CODE PATH ONCE 3.9 HAS BEEN RELEASE & STABLE

                        //hard-coded implementation for now until the bfgminer implementation is a bit more stable
                        //3.8.1 introduced a Path for HashBuster Micro, but the path is not usable with -S -d
                        arguments = string.Format("{0} -S {1}:auto -d {1}@{2}", arguments, deviceDescriptor.Driver, deviceDescriptor.Serial);
                    }
                    else if (String.IsNullOrEmpty(deviceDescriptor.Path))
                        arguments = string.Format("{0} -S {1}:{2} -d {1}@{2}", arguments, deviceDescriptor.Driver, deviceDescriptor.Serial);
                    else
                        arguments = string.Format("{0} -S {1}:{2} -d {1}@{2}", arguments, deviceDescriptor.Driver, deviceDescriptor.Path);
                }
            }

            if (minerConfiguration.Algorithm == CoinAlgorithm.Scrypt)
                //the --scrypt param must come before the --intensity params to use over 13 in latest cgminer
                arguments = MinerParameter.Scrypt + " " + arguments.TrimStart();

            if (minerConfiguration.ApiListen)
            {
                const string localIp = "127.0.0.1";
                string allowedApiIps = minerConfiguration.AllowedApiIps;

                if (String.IsNullOrEmpty(allowedApiIps))
                    allowedApiIps = String.Empty;
                else
                    allowedApiIps = allowedApiIps.Replace(" ", "");

                if (!allowedApiIps.Contains(localIp))
                    allowedApiIps = String.Format("{0},{1}", localIp, allowedApiIps);
                arguments = string.Format("{0} --api-listen --api-port {1} --api-allow W:{2}", arguments,
                    minerConfiguration.ApiPort, allowedApiIps);
            }

            //don't mine with GPUs if this is solely a stratum instance
            if (minerConfiguration.StratumProxy && (minerConfiguration.DeviceDescriptors.Count == 0))
                minerConfiguration.DisableGpu = true;

            //required to run from inside an .app package on OS X
            //also required under Windows to avoid "initscr(): Unable to create SP"
            arguments = arguments + " -T";

            //limits console output
            //we don't read the console output anyway and this is proven to improve performance (at least with very fast devices, e.g. KnC)
            arguments = arguments + " -q";

            //specify a value for --log so we can depend on the API RPC current hashrate key
            arguments = String.Format("{0} --log {1}", arguments, minerConfiguration.LogInterval);

            Process process = StartMinerProcess(arguments, redirectOutput, ensureProcessStarts, reason);

            return process;
        }

        private Process StartMinerProcess(string arguments, bool redirectOutput, 
            bool ensureProcessStarts = false, string reason = "", bool startProcess = true)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            
            startInfo.FileName = minerConfiguration.ExecutablePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;

            startInfo.Arguments = arguments.Trim();
            if (minerConfiguration.DisableGpu)
            {
                startInfo.Arguments = String.Format("{0} {1}", startInfo.Arguments, MinerParameter.ScanSerialOpenCLNoAuto);
            }

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = redirectOutput;

            if (LogLaunch != null)
            {
                LogLaunchArgs args = new LogLaunchArgs();

                args.DateTime = DateTime.Now;
                args.ExecutablePath = startInfo.FileName;
                args.Arguments = startInfo.Arguments;
                args.Reason = reason;
                args.CoinName = minerConfiguration.CoinName;

                LogLaunch(this, args);
            }

            //requiest UTF-8 encoding so that characters from bfgminer are read 
            startInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            
            Process process = StartProcessAndCheckResponse(startInfo, startProcess);
            
            if (startProcess)
            {
                if (ensureProcessStarts)
                    //store the returned process
                    process = EnsureProcessStarts(process, startInfo);
                
                if (!process.HasExited)
                    process.PriorityClass = minerConfiguration.Priority;
            }

            return process;
        }

        private Process StartProcessAndCheckResponse(ProcessStartInfo startInfo,
            bool startProcess = true)
        {
            bool userWillWatchOutput = startInfo.RedirectStandardOutput;

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            Process process = new Process();
            process.StartInfo = startInfo;

            if (!userWillWatchOutput)
                process.OutputDataReceived += HandleProcessOutput;
            process.ErrorDataReceived += HandleProcessError;

            if (startProcess)
            {
                if (process.Start() && !userWillWatchOutput)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }
            }

            return process;
        }

        private bool errorHandledByEvent = false;
        private void HandleProcessOutput(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
                return;

            errorHandledByEvent = false;

            if ((e.Data.IndexOf("auth failed", StringComparison.OrdinalIgnoreCase) >= 0) && (AuthenticationFailed != null))
            {
                errorHandledByEvent = true; //must be done first as its async
                AuthenticationFailedArgs args = new AuthenticationFailedArgs();
                Match match = Regex.Match(e.Data, @" pool (\d) ");
                string poolIndex = "??";
                if (match.Success)
                    poolIndex = match.Groups[1].Value;
                args.Reason = String.Format("Authentication failed ({0} pool {1})",
                    minerConfiguration.CoinName, poolIndex);
                AuthenticationFailed(this, args);
            }

            if (((e.Data.IndexOf("No servers could be used", StringComparison.OrdinalIgnoreCase) >= 0) ||
                (e.Data.IndexOf("No servers were found", StringComparison.OrdinalIgnoreCase) >= 0))
                && (LaunchFailed != null))
            {
                errorHandledByEvent = true; //must be done first as its async
                LaunchFailedArgs args = new LaunchFailedArgs();
                args.Reason = String.Format("None of the pools configured for {0} could be used. Verify your pool settings and try again.",
                    minerConfiguration.CoinName);
                args.CoinName = minerConfiguration.CoinName;
                LaunchFailed(this, args);
            }

            if (e.Data.IndexOf("detected new block", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ((Process)sender).CancelOutputRead();
                ((Process)sender).CancelErrorRead();
            }
        }

        private string processLaunchError = string.Empty;
        private void HandleProcessError(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data))
                return;

            processLaunchError = e.Data;
        }
        
        private Process EnsureProcessStarts(Process process, ProcessStartInfo startInfo)
        {
            //any lower than this seems to have a decent chance of a USB ASIC miner process not
            //successfully stopping & restarting
            //const int timeout = 3500;

            //newest cgminer, paired with USB ASIC's, likes to die on startup a few times saying the specified device
            //wasn't detected, happens when stopping/starting mining on USB ASIC's repeatedly
            //Thread.Sleep(timeout); //no more cgminer, can we do this?

            //new code for bfgminer, a sleep is still needed for starting many instances in a loop
            Thread.Sleep(500);

            int retries = 0;
            const int maxRetries = 0; //no more cgminer, can we do this?

            while (process.HasExited)
            {
                if ((retries >= maxRetries)
                    //only throw an error if we haven't already triggered an event
                    && !errorHandledByEvent)
                {
                    string errors = processLaunchError;

                    throw new MinerLaunchException(
                        string.Format("{5} is exiting after launching with exit code {1}.\n\n" +
                        "Details: {4}\n\nExecutable: {2}\nArguments: {3}",
                        retries, process.ExitCode, startInfo.FileName, startInfo.Arguments, errors, 
                        Path.GetFileNameWithoutExtension(startInfo.FileName)));
                }

                //ensure the new process is stored and returned
                process = StartProcessAndCheckResponse(startInfo);
                //Thread.Sleep(timeout); //no more cgminer, can we do this?
                retries++;
            }

            return process;
        }
    }
}
