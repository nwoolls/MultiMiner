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
                MultiMiner.Xgminer.MinerConfiguration minerConfiguration = CreateMinerConfiguration(port, coinSymbol);

                Process process = LaunchMinerProcess(minerConfiguration);
                
                if (!process.HasExited)
                {
                    MinerProcess minerProcess = new MinerProcess();

                    minerProcess.Process = process;
                    minerProcess.ApiPort = port;
                    minerProcess.MinerConfiguration = minerConfiguration;

                    minerProcesses.Add(minerProcess);
                }

                port++;
            }
        }

        private static Process LaunchMinerProcess(MultiMiner.Xgminer.MinerConfiguration minerConfiguration)
        {
            Miner miner = new Miner(minerConfiguration);
            Process process = miner.Launch();
            return process;
        }

        private MultiMiner.Xgminer.MinerConfiguration CreateMinerConfiguration(int port, string coinSymbol)
        {
            CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Symbol.Equals(coinSymbol));

            IEnumerable<DeviceConfiguration> coinGpuConfigurations = engineConfiguration.DeviceConfigurations.Where(c => c.CoinSymbol.Equals(coinSymbol));

            MultiMiner.Xgminer.MinerConfiguration minerConfiguration = new MultiMiner.Xgminer.MinerConfiguration();

            string minerName = "cgminer";
            if (engineConfiguration.XgminerConfiguration.MinerBackend == MinerBackend.Bfgminer)
                minerName = "bfgminer";

            switch (Environment.OSVersion.Platform) 
            {
                case PlatformID.Unix:
                    minerConfiguration.ExecutablePath = minerName;
                    break;
                default:
                    minerConfiguration.ExecutablePath = string.Format(@"Miners\{0}\{0}.exe", minerName);
                    break;
            }

            minerConfiguration.MinerBackend = engineConfiguration.XgminerConfiguration.MinerBackend;
            
            minerConfiguration.Pools = coinConfiguration.Pools;
            minerConfiguration.Algorithm = coinConfiguration.Coin.Algorithm;
            minerConfiguration.ApiPort = port;
            minerConfiguration.ApiListen = true;

            foreach (DeviceConfiguration coinGpuConfiguration in coinGpuConfigurations)
                minerConfiguration.DeviceIndexes.Add(coinGpuConfiguration.DeviceIndex);

            string arguments = string.Empty;
            if (engineConfiguration.XgminerConfiguration.AlgorithmFlags.ContainsKey(coinConfiguration.Coin.Algorithm))
                arguments = String.Format("{0} {1}", arguments,
                    engineConfiguration.XgminerConfiguration.AlgorithmFlags[coinConfiguration.Coin.Algorithm]);

            minerConfiguration.Arguments = arguments;

            return minerConfiguration;
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

        public void RelaunchCrashedMiners()
        {
            foreach (MinerProcess minerProcess in MinerProcesses)
                if (minerProcess.Process.HasExited)
                    minerProcess.Process = new Miner(minerProcess.MinerConfiguration).Launch();
        }
    }
}
