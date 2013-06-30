using MultiMiner.Xgminer.Parsers;
using System.Collections.Generic;
using System.Diagnostics;

namespace MultiMiner.Xgminer
{
    public class Miner
    {
        private readonly MinerConfiguration minerConfig;

        public Miner(MinerConfiguration minerConfig)
        {
            this.minerConfig = minerConfig;
        }

        public List<Device> GetDevices()
        {
            List<Device> result = new List<Device>();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = minerConfig.ExecutablePath;
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

            DeviceParser.ParseTextForDevices(output, result);

            return result;
        }

        public Process StartMining(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = minerConfig.ExecutablePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = arguments;

            return Process.Start(startInfo);
        }
    }
}
