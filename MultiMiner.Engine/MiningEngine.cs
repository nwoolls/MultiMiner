using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MultiMiner.Engine
{
    public class MiningEngine
    {
        private List<MinerProcess> minerProcesses = new List<MinerProcess>();
        private EngineConfiguration engineConfiguration;

        public List<MinerProcess> MinerProcesses
        {
            get
            {
                return minerProcesses;
            }
        }

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

            int port = 4028;

            foreach (string coinSymbol in coinSymbols)
            {
                CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Symbol.Equals(coinSymbol));

                IEnumerable<DeviceConfiguration> coinGpuConfigurations = engineConfiguration.DeviceConfigurations.Where(c => c.CoinSymbol.Equals(coinSymbol));
                                
                MultiMiner.Xgminer.MinerConfiguration minerConfig = new MultiMiner.Xgminer.MinerConfiguration();

                minerConfig.ExecutablePath = @"Miners\cgminer\cgminer.exe";
                minerConfig.Pools = coinConfiguration.Pools;
                minerConfig.Algorithm = coinConfiguration.Coin.Algorithm;
                minerConfig.ApiPort = port;
                minerConfig.ApiListen = true;

                foreach (DeviceConfiguration coinGpuConfiguration in coinGpuConfigurations)
                    minerConfig.DeviceIndexes.Add(coinGpuConfiguration.DeviceIndex);

                Miner miner = new Miner(minerConfig);

                string arguments = string.Empty;                
                if (engineConfiguration.MinerConfiguration.AlgorithmFlags.ContainsKey(coinConfiguration.Coin.Algorithm))
                    arguments = String.Format("{0} {1}", arguments, 
                        engineConfiguration.MinerConfiguration.AlgorithmFlags[coinConfiguration.Coin.Algorithm]);
                                                
                Process process = miner.Launch(arguments);

                //newest cgminer, paired with USB ASIC's, likes to die on startup a few times saying the specified device
                //wasn't detected, happens when starting/stopping mining on USB ASIC's repeatedly
                Thread.Sleep(5000);

                while (process.HasExited)
                {
                    process = miner.Launch(arguments);
                    Thread.Sleep(2000);
                }
                
                if (!process.HasExited)
                {
                    MinerProcess minerProcess = new MinerProcess();

                    minerProcess.Process = process;
                    minerProcess.ApiPort = port;
                    minerProcess.DevicesIndexes = minerConfig.DeviceIndexes;

                    minerProcesses.Add(minerProcess);
                }

                port++;
            }
        }

        public void StopMining()
        {
            foreach (MinerProcess minerProcess in minerProcesses)
                if (!minerProcess.Process.HasExited)
                {
                    ApiContext apiContext = minerProcess.ApiContext;
                    if (apiContext != null)
                    {
                        apiContext.QuitMining();
                        Thread.Sleep(250);
                    }
                    minerProcess.Process.Kill();
                }

            minerProcesses.Clear();

            mining = false;
        }
    }
}
