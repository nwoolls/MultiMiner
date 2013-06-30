using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                                
                MultiMiner.Xgminer.MinerConfiguration minerConfig = new MultiMiner.Xgminer.MinerConfiguration();
                minerConfig.ExecutablePath = @"Miners\cgminer\cgminer.exe";
                Miner miner = new Miner(minerConfig);

                string arguments = string.Empty;

                if (coinConfiguration.Coin.Algorithm == CoinAlgorithm.Scrypt)
                    arguments = arguments + " --scrypt";

                foreach (MiningPool pool in coinConfiguration.Pools)
                {
                    string argument = string.Format("-o {0}:{1} -u {2} -p {3}", pool.Host, pool.Port, pool.Username, pool.Password);
                    arguments = String.Format("{0} {1}", arguments, argument);
                }

                if (engineConfiguration.MinerConfiguration.AlgorithmFlags.ContainsKey(coinConfiguration.Coin.Algorithm))
                    arguments = String.Format("{0} {1}", arguments, 
                        engineConfiguration.MinerConfiguration.AlgorithmFlags[coinConfiguration.Coin.Algorithm]);

                foreach (DeviceConfiguration coinGpuConfiguration in coinGpuConfigurations)
                    arguments = String.Format("{0} -d {1}", arguments, coinGpuConfiguration.DeviceIndex);

                Process minerProcess = miner.StartMining(arguments);
                                
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
