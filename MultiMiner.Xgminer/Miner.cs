using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MultiMiner.Xgminer
{
    public class Miner
    {
        private Process minerProcess;
        private readonly MinerConfig minerConfig;

        public Miner(MinerConfig minerConfig)
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
            minerProcess = Process.Start(startInfo);


            List<string> output = new List<string>();

            while (!minerProcess.StandardOutput.EndOfStream)
            {
                string line = minerProcess.StandardOutput.ReadLine();
                output.Add(line);
            }

            DeviceParser.ParseOutputForDevices(output, result);

            return result;
        }
    }
}
