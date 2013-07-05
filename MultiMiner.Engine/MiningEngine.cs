using MultiMiner.Coinchoose.Api;
using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
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
            StopMining();
            StartMining();
        }

        public void StartMining(EngineConfiguration engineConfiguration, List<Device> devices, List<CoinInformation> coinInformation)
        {
            StopMining();

            this.engineConfiguration = engineConfiguration;

            ApplyMiningStrategy(devices, coinInformation);
            
            if (!mining) //above call to ApplyMiningStrategy may have started mining due to config change
                StartMining();

            mining = true;
        }

        public void StopMining()
        {
            foreach (MinerProcess minerProcess in minerProcesses)
                if (!minerProcess.Process.HasExited)
                {
                    Xgminer.Api.ApiContext apiContext = minerProcess.ApiContext;
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

        private bool DeviceIsGpu(Device device)
        {
            return device.Identifier.Equals("GPU") || device.Identifier.Equals("OCL");
        }

        //update engineConfiguration.DeviceConfiguration based on mining strategy & coin info
        public void ApplyMiningStrategy(List<Device> devices, List<CoinInformation> coinInformation)
        {
            if (engineConfiguration.StrategyConfiguration.MineProfitableCoins)
            {
                //get a list of the coins that are configured
                IEnumerable<string> configuredSymbols = engineConfiguration.CoinConfigurations.Select(c => c.Coin.Symbol);

                //filter the coin info by that list
                IEnumerable<CoinInformation> configuredProfitableCoins = coinInformation.Where(c => configuredSymbols.Contains(c.Symbol));

                if (configuredProfitableCoins.Count() > 0)
                {
                    List<CoinInformation> orderedProfitableCoins = configuredProfitableCoins.OrderByDescending(c => c.AdjustedProfitability).ToList();

                    List<DeviceConfiguration> newConfiguration = CreateAutomaticDeviceConfiguration(devices, orderedProfitableCoins);

                    //compare newConfiguration to engineConfiguration.DeviceConfiguration
                    //store if different
                    bool configDifferent = DeviceConfigurationsDiffer(engineConfiguration.DeviceConfigurations, newConfiguration);

                    //apply newConfiguration to engineConfiguration.DeviceConfiguration
                    engineConfiguration.DeviceConfigurations.Clear();
                    foreach (DeviceConfiguration deviceConfiguration in newConfiguration)
                        engineConfiguration.DeviceConfigurations.Add(deviceConfiguration);

                    //restart mining if stored bool is true
                    if (configDifferent)
                        RestartMining();
                }
            }
        }

        private List<DeviceConfiguration> CreateAutomaticDeviceConfiguration(List<Device> devices, IEnumerable<CoinInformation> unfilteredProfitableCoins)
        {            
            //order by adjusted profitability
            List<CoinInformation> filteredProfitableCoins = GetFilteredProfitableCoins(unfilteredProfitableCoins);

            //get sha256 only options
            List<CoinInformation> sha256ProfitableCoins = filteredProfitableCoins.Where(c => c.Algorithm.Equals("SHA-256")).ToList();

            //ABM - always be mining
            if (filteredProfitableCoins.Count == 0)
                filteredProfitableCoins.Add(unfilteredProfitableCoins.First());

            if (sha256ProfitableCoins.Count == 0)
            {
                Coinchoose.Api.CoinInformation sha256Info = unfilteredProfitableCoins.Where(c => c.Algorithm.Equals("SHA-256")).FirstOrDefault();
                if (sha256Info != null)
                    sha256ProfitableCoins.Add(sha256Info);
            }
            //end ABM

            return CreateDeviceConfigurationForProfitableCoins(devices, filteredProfitableCoins, sha256ProfitableCoins);
        }

        private List<DeviceConfiguration> CreateDeviceConfigurationForProfitableCoins(List<Device> devices, List<CoinInformation> allProfitableCoins, List<CoinInformation> sha256ProfitableCoins)
        {
            List<DeviceConfiguration> newConfiguration = new List<DeviceConfiguration>();
            CoinInformation profitableCoin = null;

            int gpuIterator = 0;
            int amuIterator = 0;

            for (int i = 0; i < devices.Count; i++)
            {
                Device device = devices[i];
                profitableCoin = null;

                if (DeviceIsGpu(device))
                {
                    //sha256 or scrypt
                    if (engineConfiguration.StrategyConfiguration.SwitchStrategy == StrategyConfiguration.CoinSwitchStrategy.SingleMostProfitable)
                        profitableCoin = allProfitableCoins.First();
                    else
                        profitableCoin = allProfitableCoins[gpuIterator];

                    gpuIterator++;
                    if (gpuIterator >= allProfitableCoins.Count())
                        gpuIterator = 0;
                }
                else if (sha256ProfitableCoins.Count > 0)
                {
                    //sha256 only
                    if (engineConfiguration.StrategyConfiguration.SwitchStrategy == StrategyConfiguration.CoinSwitchStrategy.SingleMostProfitable)
                        profitableCoin = sha256ProfitableCoins.First();
                    else
                        profitableCoin = sha256ProfitableCoins[amuIterator];

                    amuIterator++;
                    if (amuIterator >= sha256ProfitableCoins.Count())
                        amuIterator = 0;
                }

                if (profitableCoin != null)
                {
                    DeviceConfiguration configEntry = new DeviceConfiguration();

                    configEntry.DeviceIndex = i;
                    configEntry.CoinSymbol = profitableCoin.Symbol;

                    newConfiguration.Add(configEntry);
                }
            }

            return newConfiguration;
        }

        private List<CoinInformation> GetFilteredProfitableCoins(IEnumerable<CoinInformation> unfilteredProfitableCoins)
        {
            List<CoinInformation> filteredProfitableCoins = unfilteredProfitableCoins.ToList(); //call ToList to get a copy

            if (!string.IsNullOrEmpty(engineConfiguration.StrategyConfiguration.MinimumProfitabilitySymbol))
            {
                CoinInformation minimumCoin = filteredProfitableCoins.SingleOrDefault(c => c.Symbol.Equals(engineConfiguration.StrategyConfiguration.MinimumProfitabilitySymbol));
                int index = filteredProfitableCoins.IndexOf(minimumCoin);
                index++;
                filteredProfitableCoins.RemoveRange(index, filteredProfitableCoins.Count - index);
            }

            if (engineConfiguration.StrategyConfiguration.MinimumProfitabilityPercentage.HasValue)
            {
                filteredProfitableCoins = filteredProfitableCoins.Where(
                    c => c.AdjustedProfitability > engineConfiguration.StrategyConfiguration.MinimumProfitabilityPercentage).ToList();
            }

            return filteredProfitableCoins;
        }

        private static bool DeviceConfigurationsDiffer(List<DeviceConfiguration> configuration1, List<DeviceConfiguration> configuration2)
        {
            bool configDifferent = configuration1.Count != configuration2.Count;

            if (!configDifferent)
            {
                for (int i = 0; i < configuration1.Count; i++)
                {
                    DeviceConfiguration entry1 = configuration1[i];
                    DeviceConfiguration entry2 = configuration2[i];

                    configDifferent = (!entry1.CoinSymbol.Equals(entry2.CoinSymbol) || (entry1.DeviceIndex != entry2.DeviceIndex));
                    if (configDifferent)
                        break;
                }
            }

            return configDifferent;
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

            mining = true;
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

            //apply algorithm-specific parameters
            if (engineConfiguration.XgminerConfiguration.AlgorithmFlags.ContainsKey(coinConfiguration.Coin.Algorithm))
                arguments = String.Format("{0} {1}", arguments,
                    engineConfiguration.XgminerConfiguration.AlgorithmFlags[coinConfiguration.Coin.Algorithm]);

            //apply coin-specific parameters
            if (!string.IsNullOrEmpty(coinConfiguration.MinerFlags))
                arguments = string.Format("{0} {1}", arguments, coinConfiguration.MinerFlags);

            minerConfiguration.Arguments = arguments;

            return minerConfiguration;
        }
    }
}
