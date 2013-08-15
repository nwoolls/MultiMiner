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
        //events
        //delegate declarations
        public delegate void LogProcessCloseHandler(object sender, LogProcessCloseArgs ea);

        //event declarations        
        public event LogProcessCloseHandler LogProcessClose;
        public event Miner.LogLaunchHandler LogProcessLaunch;
        public event Miner.LaunchFailedHandler ProcessLaunchFailed;

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

        private bool startingMining = false;
        public void StartMining(EngineConfiguration engineConfiguration, List<Device> devices, List<CoinInformation> coinInformation)
        {
            StopMining();

            startingMining = true;
            try
            {
                this.engineConfiguration = engineConfiguration;

                ApplyMiningStrategy(devices, coinInformation);

                if (!mining) //above call to ApplyMiningStrategy may have started mining due to config change
                    StartMining();

                mining = true;
            }
            finally
            {
                startingMining = false;
            }
        }

        private bool stoppingMining = false;
        public void StopMining()
        {
            stoppingMining = true;
            try
            {
                foreach (MinerProcess minerProcess in minerProcesses)
                {
                    logProcessClose(minerProcess);
                    minerProcess.StopMining();
                }

                minerProcesses.Clear();

                mining = false;
            }
            finally
            {
                stoppingMining = false;
            }
        }
        
        public void RelaunchCrashedMiners()
        {
            if (stoppingMining || startingMining)
                return; //don't try to relaunch miners we are stopping or starting

            foreach (MinerProcess minerProcess in MinerProcesses)
            {
                if (minerProcess.Process.HasExited)
                {
                    logProcessClose(minerProcess);
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Process crashed");
                    setupProcessStartInfo(minerProcess);
                }

                else if (minerProcess.HasDeadDevice)
                {
                    minerProcess.StopMining();
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Dead device");
                    setupProcessStartInfo(minerProcess);
                }

                else if (minerProcess.HasSickDevice)
                {
                    minerProcess.StopMining();
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Sick device");
                    setupProcessStartInfo(minerProcess);
                }

                else if (minerProcess.HasZeroHashrateDevice || minerProcess.HasFrozenDevice)
                {
                    TimeSpan processAge = DateTime.Now - minerProcess.Process.StartTime;
                    if (processAge.TotalSeconds > 60)
                    {
                        minerProcess.StopMining();
                        minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Zero hashrate");
                        setupProcessStartInfo(minerProcess);
                    }
                }
            }
        }

        private void setupProcessStartInfo(MinerProcess minerProcess)
        {
            string coinName = minerProcess.MinerConfiguration.CoinName;
            string coinSymbol = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase)).Coin.Symbol;
            CoinInformation processCoinInfo = coinInformation.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            
            //coin may not be in CoinChoose.com
            if (coinInformation != null)
                minerProcess.CoinInformation = processCoinInfo;
            
            minerProcess.StartDate = DateTime.Now;
        }

        private void logProcessClose(MinerProcess minerProcess)
        {
            DateTime startDate = minerProcess.StartDate;
            DateTime endDate = DateTime.Now;
            string coinName = minerProcess.MinerConfiguration.CoinName;

            double priceAtStart = 0;
            string coinSymbol = String.Empty;
            //coin may not be in CoinChoose.com
            if (minerProcess.CoinInformation != null)
            {
                coinSymbol = minerProcess.CoinInformation.Symbol;
                priceAtStart = minerProcess.CoinInformation.Price;
            }

            double priceAtEnd = priceAtStart;

            //can't use Single here - coin info may be gone now and we crash
            CoinInformation coinInfo = coinInformation.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            if (coinInfo != null)
                priceAtEnd = coinInfo.Price;

            List<int> deviceIndexes = minerProcess.MinerConfiguration.DeviceIndexes;

            logProcessClose(startDate, endDate, coinName, coinSymbol, priceAtStart, priceAtEnd, deviceIndexes);
        }

        private void logProcessClose(DateTime startDate, DateTime endDate, string coinName, string coinSymbol,
            double priceAtStart, double priceAtEnd, List<int> deviceIndexes)
        {
            if (this.LogProcessClose != null)
            {
                LogProcessCloseArgs args = new LogProcessCloseArgs();
                args.StartDate = startDate;
                args.EndDate = endDate;
                args.CoinName = coinName;
                args.CoinSymbol = coinSymbol;
                args.StartPrice = priceAtStart;
                args.EndPrice = priceAtEnd;
                args.DeviceIndexes = deviceIndexes;

                this.LogProcessClose(this, args);
            }
        }

        private List<CoinInformation> coinInformation;
        //update engineConfiguration.DeviceConfiguration based on mining strategy & coin info
        public void ApplyMiningStrategy(List<Device> devices, List<CoinInformation> coinInformation)
        {
            //store this off so we can reference prices for logging
            this.coinInformation = coinInformation;

            //make a copy as we'll be modifying individual coin properties (profitability)
            //if no copy is made this could lead to a compounding effect
            List<CoinInformation> coinInformationCopy = CopyCoinInformation(coinInformation);

            if (engineConfiguration.StrategyConfiguration.MineProfitableCoins)
            {
                //get a list of the coins that are configured
                IEnumerable<string> configuredSymbols = engineConfiguration.CoinConfigurations.Where(c => c.Enabled).Select(c => c.Coin.Symbol);

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

        private static List<CoinInformation> CopyCoinInformation(List<CoinInformation> coinInformation)
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
                //there should be a 1-to-1 relationship of devices and device configurations
                DeviceConfiguration existingConfiguration = engineConfiguration.DeviceConfigurations[i];

                if (existingConfiguration.Enabled)
                {
                    profitableCoin = null;

                    if (device.Kind == DeviceKind.GPU)
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

                    DeviceConfiguration configEntry = new DeviceConfiguration();

                    configEntry.DeviceIndex = i;
                    configEntry.CoinSymbol = profitableCoin == null ? string.Empty : profitableCoin.Symbol;
                    
                    newConfiguration.Add(configEntry);
                }
                else
                {
                    DeviceConfiguration configEntry = new DeviceConfiguration();

                    configEntry.DeviceIndex = i;
                    configEntry.CoinSymbol = existingConfiguration.CoinSymbol;
                    configEntry.Enabled = false;

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
            IEnumerable<string> coinSymbols = engineConfiguration.DeviceConfigurations
                .Where(c => c.Enabled && !string.IsNullOrEmpty(c.CoinSymbol))
                .Select(c => c.CoinSymbol)
                .Distinct();

            int port = 4028;

            foreach (string coinSymbol in coinSymbols)
            {
                MinerConfiguration minerConfiguration = CreateMinerConfiguration(port, coinSymbol);

                Process process = LaunchMinerProcess(minerConfiguration, "Starting mining");

                if (!process.HasExited)
                {
                    MinerProcess minerProcess = new MinerProcess();

                    minerProcess.Process = process;
                    minerProcess.ApiPort = port;
                    minerProcess.MinerConfiguration = minerConfiguration;

                    setupProcessStartInfo(minerProcess);

                    minerProcesses.Add(minerProcess);
                }

                port++;
            }

            mining = true;
        }

        private Process LaunchMinerProcess(MinerConfiguration minerConfiguration, string reason)
        {
            minerConfiguration.Priority = this.engineConfiguration.XgminerConfiguration.Priority;
            Miner miner = new Miner(minerConfiguration);
            miner.LogLaunch += this.LogProcessLaunch;
            miner.LaunchFailed += this.ProcessLaunchFailed;
            Process process = miner.Launch(reason);
            return process;
        }

        private MinerConfiguration CreateMinerConfiguration(int port, string coinSymbol)
        {
            CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Symbol.Equals(coinSymbol));

            IEnumerable<DeviceConfiguration> coinGpuConfigurations = engineConfiguration.DeviceConfigurations.Where(c => c.Enabled && c.CoinSymbol.Equals(coinSymbol));

            MinerConfiguration minerConfiguration = new MinerConfiguration();
            
            minerConfiguration.MinerBackend = engineConfiguration.XgminerConfiguration.MinerBackend;
            minerConfiguration.ExecutablePath = MinerPath.GetPathToInstalledMiner(minerConfiguration.MinerBackend);
            
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

            if (engineConfiguration.XgminerConfiguration.DesktopMode)
                arguments = arguments + " -I D";
            
            minerConfiguration.Arguments = arguments;

            return minerConfiguration;
        }
    }
}
