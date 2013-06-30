using MultiMiner.Xgminer.Parsers;
using System.Collections.Generic;
using System.Diagnostics;

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

        public Process Launch(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = minerConfiguration.ExecutablePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;

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

            return Process.Start(startInfo);
        }
    }
}
