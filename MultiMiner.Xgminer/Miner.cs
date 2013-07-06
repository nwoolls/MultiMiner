using MultiMiner.Xgminer.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MultiMiner.Xgminer
{
    public class Miner
    {
        private readonly MinerConfiguration minerConfiguration;

        public Miner(MinerConfiguration minerConfig)
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
        public List<Device> DeviceList()
        {
            string arguments = MinerParameter.DeviceList;
            bool redirectOutput = true;

            if (minerConfiguration.MinerBackend == MinerBackend.Bfgminer)
                arguments = String.Format("{0} {1}", arguments, Bfgminer.MinerParameter.ScanSerialAll);

            Process minerProcess = StartMinerProcess(arguments, redirectOutput);

            List<string> output = new List<string>();

            while (!minerProcess.StandardOutput.EndOfStream)
            {
                string line = minerProcess.StandardOutput.ReadLine();
                output.Add(line);
            }

            List<Device> result = new List<Device>();
            DeviceListParser.ParseTextForDevices(output, result);

            return result;
        }

        public Process Launch()
        {
            bool redirectOutput = false;
            bool ensureProcessStarts = true;

            //otherwise cgminer output shows under *nix in the terminal
            //DONT do it for Windows though or cgminer will close after opening

            //commented out for now - seems to eventually cause issues under .nix too
            //the miner keeps mining but Accepted shares stop updating
            //if (Environment.OSVersion.Platform == PlatformID.Unix)
            //    redirectOutput = true;

            string arguments = minerConfiguration.Arguments;

            if (minerConfiguration.MinerBackend == MinerBackend.Bfgminer)
                arguments = String.Format("{0} {1}", arguments, Bfgminer.MinerParameter.ScanSerialAll);

            foreach (MiningPool pool in minerConfiguration.Pools)
            {
                string argument = string.Format("-o {0}:{1} -u {2} -p {3}", pool.Host, pool.Port, pool.Username, pool.Password);
                arguments = string.Format("{0} {1}", arguments, argument);
            }

            foreach (int deviceIndex in minerConfiguration.DeviceIndexes)
                arguments = string.Format("{0} -d {1}", arguments, deviceIndex);

            if (minerConfiguration.Algorithm == CoinAlgorithm.Scrypt)
                arguments = arguments + " --scrypt";

            if (minerConfiguration.ApiListen)
                arguments = string.Format("{0} --api-listen --api-port {1} --api-allow W:127.0.0.1", arguments, minerConfiguration.ApiPort);

            Process process = StartMinerProcess(arguments, redirectOutput, ensureProcessStarts);

            return process;
        }

        private Process StartMinerProcess(string arguments, bool redirectOutput, bool ensureProcessStarts = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = minerConfiguration.ExecutablePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = redirectOutput;

            Process process = Process.Start(startInfo);

            if (ensureProcessStarts)
                EnsureProcessStarts(process, startInfo);

            return process;
        }

        private static void EnsureProcessStarts(Process process, ProcessStartInfo startInfo)
        {
            const int timeout = 2500;

            //newest cgminer, paired with USB ASIC's, likes to die on startup a few times saying the specified device
            //wasn't detected, happens when starting/stopping mining on USB ASIC's repeatedly
            Thread.Sleep(timeout);

            int retries = 0;
            const int maxRetries = 5;

            while (process.HasExited)
            {
                if (retries >= maxRetries)
                    throw new Exception(string.Format("Miner keeps exiting after launching - retried {0} times. Exit code {1}.", retries, process.ExitCode));

                process = process = Process.Start(startInfo);
                Thread.Sleep(timeout);
                retries++;
            }
        }
    }
}
