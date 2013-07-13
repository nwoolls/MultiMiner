using MultiMiner.Coinchoose.Api;
using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
                minerProcess.StopMining();

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
            //make a copy as we'll be modifying individual coin properties (profitability)
            //if no copy is made this could lead to a compounding effect
            List<CoinInformation> coinInformationCopy = CopyCoinInformation(coinInformation);

            if (engineConfiguration.StrategyConfiguration.MineProfitableCoins)
            {
                //get a list of the coins that are configured
                IEnumerable<string> configuredSymbols = engineConfiguration.CoinConfigurations.Select(c => c.Coin.Symbol);

                //filter the coin info by that list
                //use the copy here
                IEnumerable<CoinInformation> configuredProfitableCoins = coinInformationCopy.Where(c => configuredSymbols.Contains(c.Symbol));

                if (configuredProfitableCoins.Count() > 0)
                {
                    //adjust profitabilities based on config adjustments
                    ApplyProfitabilityAdjustments(configuredProfitableCoins);

                    List<CoinInformation> orderedProfitableCoins = GetCoinsOrderedByProfitability(configuredProfitableCoins);

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

        private List<CoinInformation> CopyCoinInformation(List<CoinInformation> coinInformation)
        {
            List<CoinInformation> coinInformationCopy = new List<CoinInformation>();
            foreach (CoinInformation realCoin in coinInformation)
            {
                CoinInformation coinCopy = new CoinInformation();
                CopyPoco(realCoin, coinCopy);
                coinInformationCopy.Add(coinCopy);
            }
            return coinInformationCopy;
        }

        private static void CopyPoco(object source, object destination)
        {
            Type destionationType = destination.GetType();
            foreach (PropertyInfo sourceProperty in source.GetType().GetProperties())
            {
                MethodInfo sourcePropertyGetter = sourceProperty.GetGetMethod();
                MethodInfo sourcePropertySetter = destionationType.GetProperty(sourceProperty.Name).GetSetMethod();
                object valueToSet = sourcePropertyGetter.Invoke(source, null);
                sourcePropertySetter.Invoke(destination, new[] { valueToSet });
            }
        }

        private List<CoinInformation> GetCoinsOrderedByProfitability(IEnumerable<CoinInformation> configuredProfitableCoins)
        {
            List<CoinInformation> orderedProfitableCoins = configuredProfitableCoins.ToList();

            switch (engineConfiguration.StrategyConfiguration.ProfitabilityBasis)
            {
                case StrategyConfiguration.CoinProfitabilityBasis.AdjustedProfitability:
                    orderedProfitableCoins = orderedProfitableCoins.OrderByDescending(c => c.AdjustedProfitability).ToList();
                    break;
                case StrategyConfiguration.CoinProfitabilityBasis.AverageProfitability:
                    orderedProfitableCoins = orderedProfitableCoins.OrderByDescending(c => c.AverageProfitability).ToList();
                    break;
                case StrategyConfiguration.CoinProfitabilityBasis.StraightProfitability:
                    orderedProfitableCoins = orderedProfitableCoins.OrderByDescending(c => c.Profitability).ToList();
                    break;
            }

            return orderedProfitableCoins;
        }

        private void ApplyProfitabilityAdjustments(IEnumerable<CoinInformation> configuredProfitableCoins)
        {
            foreach (CoinInformation configuredProfitableCoin in configuredProfitableCoins)
            {
                CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Symbol.Equals(configuredProfitableCoin.Symbol));

                if (coinConfiguration.ProfitabilityAdjustmentType == CoinConfiguration.AdjustmentType.Addition)
                {
                    configuredProfitableCoin.AdjustedProfitability += coinConfiguration.ProfitabilityAdjustment;
                    configuredProfitableCoin.AverageProfitability += coinConfiguration.ProfitabilityAdjustment;
                    configuredProfitableCoin.Profitability += coinConfiguration.ProfitabilityAdjustment;
                }
                else if (coinConfiguration.ProfitabilityAdjustmentType == CoinConfiguration.AdjustmentType.Multiplication)
                {
                    configuredProfitableCoin.AdjustedProfitability *= coinConfiguration.ProfitabilityAdjustment;
                    configuredProfitableCoin.AverageProfitability *= coinConfiguration.ProfitabilityAdjustment;
                    configuredProfitableCoin.Profitability *= coinConfiguration.ProfitabilityAdjustment;
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
                CoinInformation sha256Info = unfilteredProfitableCoins.Where(c => c.Algorithm.Equals("SHA-256")).FirstOrDefault();
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
                    profitableCoin = GetProfitableCoinFromList(allProfitableCoins, gpuIterator);

                    gpuIterator++;
                    if (gpuIterator >= allProfitableCoins.Count())
                        gpuIterator = 0;
                }
                else if (sha256ProfitableCoins.Count > 0)
                {
                    //sha256 only
                    profitableCoin = GetProfitableCoinFromList(sha256ProfitableCoins, amuIterator);

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

        private CoinInformation GetProfitableCoinFromList(List<CoinInformation> coinList, int deviceIterator)
        {
            CoinInformation profitableCoin;

            bool mineSingle = engineConfiguration.StrategyConfiguration.SwitchStrategy == StrategyConfiguration.CoinSwitchStrategy.SingleMostProfitable;

            if (!mineSingle && engineConfiguration.StrategyConfiguration.MineMostProfitableOverridePercentage.HasValue)
            {
                switch (engineConfiguration.StrategyConfiguration.ProfitabilityBasis)
                {
                    case StrategyConfiguration.CoinProfitabilityBasis.AdjustedProfitability:
                        mineSingle = coinList.First().AdjustedProfitability > engineConfiguration.StrategyConfiguration.MineMostProfitableOverridePercentage;
                        break;
                    case StrategyConfiguration.CoinProfitabilityBasis.AverageProfitability:
                        mineSingle = coinList.First().AverageProfitability > engineConfiguration.StrategyConfiguration.MineMostProfitableOverridePercentage;
                        break;
                    case StrategyConfiguration.CoinProfitabilityBasis.StraightProfitability:
                        mineSingle = coinList.First().Profitability > engineConfiguration.StrategyConfiguration.MineMostProfitableOverridePercentage;
                        break;
                }
            }

            if (mineSingle)
                profitableCoin = coinList.First();
            else
                profitableCoin = coinList[deviceIterator];

            return profitableCoin;
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
                double minimumValue = engineConfiguration.StrategyConfiguration.MinimumProfitabilityPercentage.Value;

                switch (engineConfiguration.StrategyConfiguration.ProfitabilityBasis)
                {
                    case Configuration.StrategyConfiguration.CoinProfitabilityBasis.AdjustedProfitability:
                        filteredProfitableCoins = filteredProfitableCoins.Where(c => c.AdjustedProfitability > minimumValue).ToList();
                        break;
                    case Configuration.StrategyConfiguration.CoinProfitabilityBasis.AverageProfitability:
                        filteredProfitableCoins = filteredProfitableCoins.Where(c => c.AverageProfitability > minimumValue).ToList();
                        break;
                    case Configuration.StrategyConfiguration.CoinProfitabilityBasis.StraightProfitability:
                        filteredProfitableCoins = filteredProfitableCoins.Where(c => c.Profitability > minimumValue).ToList();
                        break;
                }
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
                MinerConfiguration minerConfiguration = CreateMinerConfiguration(port, coinSymbol);

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

        private static Process LaunchMinerProcess(MinerConfiguration minerConfiguration)
        {
            Miner miner = new Miner(minerConfiguration);
            Process process = miner.Launch();
            return process;
        }

        private MinerConfiguration CreateMinerConfiguration(int port, string coinSymbol)
        {
            CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Symbol.Equals(coinSymbol));

            IEnumerable<DeviceConfiguration> coinGpuConfigurations = engineConfiguration.DeviceConfigurations.Where(c => c.CoinSymbol.Equals(coinSymbol));

            MinerConfiguration minerConfiguration = new MinerConfiguration();

            string minerName = engineConfiguration.XgminerConfiguration.MinerName;

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
            minerConfiguration.CoinName = coinConfiguration.Coin.Name;
            minerConfiguration.DisableGpu = engineConfiguration.XgminerConfiguration.DisableGpu;

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

            if (engineConfiguration.XgminerConfiguration.DesktopMode)
                minerConfiguration.Arguments = minerConfiguration.Arguments + " -I D";

            return minerConfiguration;
        }
    }
}
