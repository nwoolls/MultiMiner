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

        public List<Device> GetDevices()
        {
            List<Device> result = new List<Device>();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = minerConfiguration.ExecutablePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = MinerParameter.EnumerateDevices;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            Process minerProcess = Process.Start(startInfo);

            List<string> output = new List<string>();

            while (!minerProcess.StandardOutput.EndOfStream)
            {
                string line = minerProcess.StandardOutput.ReadLine();
                output.Add(line);
            }

            DeviceOutputParser.ParseTextForDevices(output, result);

            return result;
        }

        public Process Launch()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = minerConfiguration.ExecutablePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            //otherwise cgminer output shows under *nix in the terminal
            //DONT do it for Windows though or cgminer will close after opening
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                startInfo.RedirectStandardOutput = true;

            string arguments = minerConfiguration.Arguments;

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

            startInfo.Arguments = arguments;

            Process process = StartMiningProcess(startInfo);

            return process;
        }

        private static Process StartMiningProcess(ProcessStartInfo startInfo)
        {
            Process process = Process.Start(startInfo);

            //newest cgminer, paired with USB ASIC's, likes to die on startup a few times saying the specified device
            //wasn't detected, happens when starting/stopping mining on USB ASIC's repeatedly
            Thread.Sleep(2000);

            int retries = 0;
            const int maxRetries = 5;

            while (process.HasExited)
            {
                if (retries >= maxRetries)
                    throw new Exception(string.Format("Miner keeps exiting after launching - retried {0} times. Exit code {1}.", retries, process.ExitCode));

                process = process = Process.Start(startInfo);
                Thread.Sleep(2000);
                retries++;
            }

            return process;
        }
    }
}
