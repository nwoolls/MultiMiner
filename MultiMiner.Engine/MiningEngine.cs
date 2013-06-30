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

                arguments = String.Format("{0} --api-listen --api-port {1}", arguments, port);

                Process process = miner.StartMining(arguments);

                Thread.Sleep(2000);
                if (process.HasExited)
                    process = miner.StartMining(arguments);
                
                if (!process.HasExited)
                {
                    MinerProcess minerProcess = new MinerProcess();

                    minerProcess.Process = process;

                    minerProcess.ApiPort = port;

                    foreach (DeviceConfiguration coinGpuConfiguration in coinGpuConfigurations)
                        minerProcess.DevicesIndexes.Add(coinGpuConfiguration.DeviceIndex);

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
