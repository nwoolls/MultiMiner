using MultiMiner.Engine.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Engine
{
    public class MiningEngine
    {
        private List<Process> miningProcesses = new List<Process>();
        private EngineConfiguration engineConfiguration;
        
        private bool mining = false;
        public bool Mining
        {
            get
            {
                return mining;
            }
        }

        public void RestartMining()
        {
            StartMining(engineConfiguration);
        }

        public void StartMining(EngineConfiguration engineConfiguration)
        {
            StopMining();

            this.engineConfiguration = engineConfiguration;

            StartMining();

            mining = true;
        }
        
        private void StartMining()
        {
            IEnumerable<string> coinSymbols = engineConfiguration.DeviceConfigurations.Select(c => c.CoinSymbol).Distinct();

            foreach (string coinSymbol in coinSymbols)
            {
                CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Symbol.Equals(coinSymbol));

                IEnumerable<DeviceConfiguration> coinGpuConfigurations = engineConfiguration.DeviceConfigurations.Where(c => c.CoinSymbol.Equals(coinSymbol));
                
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"Miners\cgminer\cgminer.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = string.Empty;

                if (coinConfiguration.Coin.Algorithm == CoinAlgorithm.Scrypt)
                    startInfo.Arguments = startInfo.Arguments + " --scrypt";

                foreach (MiningPool pool in coinConfiguration.Pools)
                {
                    string argument = string.Format("-o {0}:{1} -u {2} -p {3}", pool.Host, pool.Port, pool.Username, pool.Password);
                    startInfo.Arguments = String.Format("{0} {1}", startInfo.Arguments, argument);
                }

                if (engineConfiguration.MinerConfiguration.AlgorithmFlags.ContainsKey(coinConfiguration.Coin.Algorithm))
                    startInfo.Arguments = String.Format("{0} {1}", startInfo.Arguments, 
                        engineConfiguration.MinerConfiguration.AlgorithmFlags[coinConfiguration.Coin.Algorithm]);

                foreach (DeviceConfiguration coinGpuConfiguration in coinGpuConfigurations)
                {
                    startInfo.Arguments = String.Format("{0} -d {1}", startInfo.Arguments, coinGpuConfiguration.DeviceIndex);
                }

                Process minerProcess = Process.Start(startInfo);
                                
                if (!minerProcess.HasExited)
                    miningProcesses.Add(minerProcess);
            }
        }

        public void StopMining()
        {
            foreach (Process miningProcess in miningProcesses)
                if (!miningProcess.HasExited)
                    miningProcess.Kill();

            miningProcesses.Clear();

            mining = false;
        }
    }
}
