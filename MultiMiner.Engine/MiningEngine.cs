using MultiMiner.CoinApi.Data;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Xgminer.Data;
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
        public event Xgminer.Miner.LogLaunchHandler LogProcessLaunch;
        public event Xgminer.Miner.LaunchFailedHandler ProcessLaunchFailed;
        public event Xgminer.Miner.AuthenticationFailedHandler ProcessAuthenticationFailed;
        private List<MinerProcess> minerProcesses = new List<MinerProcess>();
        private Data.Configuration.Engine engineConfiguration;
        private List<Xgminer.Data.Device> devices;
        private Version backendVersion;
        private int donationPercent;
        
        public bool Donating
        {
            get { return donationPercent > 0; }
        }        

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
        public void StartMining(Data.Configuration.Engine engineConfiguration, List<Xgminer.Data.Device> devices, List<CoinInformation> coinInformation, int donationPercent)
        {
            StopMining();

            startingMining = true;
            try
            {
                this.engineConfiguration = engineConfiguration;
                this.devices = devices;
                this.backendVersion = new Version(Xgminer.Installer.GetInstalledMinerVersion(MinerPath.GetPathToInstalledMiner()));
                this.donationPercent = donationPercent;

                if (coinInformation != null) //null if no network connection
                    ApplyMiningStrategy(coinInformation);

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
        
        public bool RelaunchCrashedMiners()
        {
            if (stoppingMining || startingMining)
                return false; //don't try to relaunch miners we are stopping or starting

            foreach (MinerProcess minerProcess in MinerProcesses)
            {
                if (minerProcess.Process.HasExited)
                {
                    logProcessClose(minerProcess);
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Process crashed");
                    setupProcessStartInfo(minerProcess);
                    return true;
                }

                else if (minerProcess.HasDeadDevice)
                {
                    logProcessClose(minerProcess);
                    minerProcess.StopMining();
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Dead device");
                    setupProcessStartInfo(minerProcess);
                    return true;
                }

                else if (minerProcess.HasSickDevice)
                {
                    logProcessClose(minerProcess);
                    minerProcess.StopMining();
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Sick device");
                    setupProcessStartInfo(minerProcess);
                    return true;
                }

                else if (minerProcess.HasZeroHashrateDevice || minerProcess.MinerIsFrozen || minerProcess.HasPoorPerformingDevice)
                {
                    TimeSpan processAge = DateTime.Now - minerProcess.Process.StartTime;
                    //this needs to give the devices long enough to spin up
                    //making this longer (than 120) on 12/10 to account for proxy devices that take longer
                    //to spin up, such as Raspberry Pi
                    if (processAge.TotalSeconds > 240)
                    {
                        logProcessClose(minerProcess);
                        minerProcess.StopMining();
                        string reason = minerProcess.HasZeroHashrateDevice ? "Zero hashrate" : minerProcess.HasPoorPerformingDevice ? "Subpar hashrate" : "Frozen miner";
                        minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, reason);
                        setupProcessStartInfo(minerProcess);
                        return true;
                    }
                }
            }
            return false;
        }

        private void setupProcessStartInfo(MinerProcess minerProcess)
        {
            string coinName = minerProcess.MinerConfiguration.CoinName;
            string coinSymbol = engineConfiguration.CoinConfigurations.Single(c => c.CryptoCoin.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase)).CryptoCoin.Symbol;

            CoinInformation processCoinInfo = null;
            if (coinInformation != null) //null if no network connection
                processCoinInfo = coinInformation.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            
            //coin may not be in Coin API
            if (processCoinInfo != null)
                minerProcess.CoinInformation = processCoinInfo;
            
            minerProcess.StartDate = DateTime.Now;
        }

        private void logProcessClose(MinerProcess minerProcess)
        {
            if (this.LogProcessClose == null)
                return;

            DateTime startDate = minerProcess.StartDate;
            DateTime endDate = DateTime.Now;
            string coinName = minerProcess.MinerConfiguration.CoinName;

            double priceAtStart = 0;
            string coinSymbol = String.Empty;
            //coin may not be in Coin API
            if (minerProcess.CoinInformation != null)
            {
                coinSymbol = minerProcess.CoinInformation.Symbol;
                priceAtStart = minerProcess.CoinInformation.Price;
            }

            double priceAtEnd = priceAtStart;

            //can't use Single here - coin info may be gone now and we crash
            CoinInformation coinInfo = null;
            if (coinInformation != null) //null if no internet connection
                coinInfo = coinInformation.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            if (coinInfo != null)
                priceAtEnd = coinInfo.Price;

            //get a copy using ToList() so we can change the list in the event handler without
            //affecting relaunching processes
            List<DeviceDescriptor> deviceDescriptors = minerProcess.MinerConfiguration.DeviceDescriptors.ToList();
                        
            LogProcessCloseArgs args = new LogProcessCloseArgs();
            args.StartDate = startDate;
            args.EndDate = endDate;
            args.CoinName = coinName;
            args.CoinSymbol = coinSymbol;
            args.StartPrice = priceAtStart;
            args.EndPrice = priceAtEnd;
            args.DeviceDescriptors = deviceDescriptors;
            args.MinerConfiguration = minerProcess.MinerConfiguration;
            args.AcceptedShares = minerProcess.AcceptedShares;
            this.LogProcessClose(this, args);
        }

        private List<CoinInformation> coinInformation;
        //update engineConfiguration.DeviceConfiguration based on mining strategy & coin info
        public void ApplyMiningStrategy(List<CoinInformation> coinInformation)
        {
            if (coinInformation == null) //null if no network connection
                return;
            
            //store this off so we can reference prices for logging
            this.coinInformation = coinInformation;

            //make a copy as we'll be modifying individual coin properties (profitability)
            //if no copy is made this could lead to a compounding effect
            List<CoinInformation> coinInformationCopy = CopyCoinInformation(coinInformation);

            if (engineConfiguration.StrategyConfiguration.AutomaticallyMineCoins)
            {
                //get a list of the coins that are enabled and have at least one pool and pools aren't flagged down
                IEnumerable<string> configuredSymbols = engineConfiguration.CoinConfigurations.Where(
                    c => c.Enabled && 
                        !c.PoolsDown && 
                        (c.Pools.Count > 0)
                    ).Select(c => c.CryptoCoin.Symbol);

                //filter the coin info by that list
                //use the copy here
                IEnumerable<CoinInformation> filteredCoinInformation = coinInformationCopy.Where(c => configuredSymbols.Contains(c.Symbol));

                if (filteredCoinInformation.Count() > 0)
                {
                    //adjust profitabilities based on config adjustments
                    ApplyProfitabilityAdjustments(filteredCoinInformation);

                    List<CoinInformation> orderedCoinInformation = GetCoinInformationOrderedByMiningBasis(filteredCoinInformation);

                    List<Engine.Data.Configuration.Device> newConfiguration = CreateAutomaticDeviceConfiguration(orderedCoinInformation);

                    //compare newConfiguration to engineConfiguration.DeviceConfiguration
                    //store if different
                    bool configDifferent = DeviceConfigurationsDiffer(engineConfiguration.DeviceConfigurations, newConfiguration);

                    //apply newConfiguration to engineConfiguration.DeviceConfiguration
                    engineConfiguration.DeviceConfigurations.Clear();
                    foreach (Engine.Data.Configuration.Device deviceConfiguration in newConfiguration)
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

        private List<CoinInformation> GetCoinInformationOrderedByMiningBasis(IEnumerable<CoinInformation> configuredCoins)
        {
            List<CoinInformation> orderedCoins = configuredCoins.ToList();

            switch (engineConfiguration.StrategyConfiguration.MiningBasis)
            {
                case Strategy.CoinMiningBasis.Profitability:
                    switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
                    {
                        case Strategy.CoinProfitabilityKind.AdjustedProfitability:
                            orderedCoins = orderedCoins.OrderByDescending(c => c.AdjustedProfitability).ToList();
                            break;
                        case Strategy.CoinProfitabilityKind.AverageProfitability:
                            orderedCoins = orderedCoins.OrderByDescending(c => c.AverageProfitability).ToList();
                            break;
                        case Strategy.CoinProfitabilityKind.StraightProfitability:
                            orderedCoins = orderedCoins.OrderByDescending(c => c.Profitability).ToList();
                            break;
                    }
                    break;
                case Strategy.CoinMiningBasis.Difficulty:
                    orderedCoins = orderedCoins.OrderBy(c => c.Difficulty).ToList();
                    break;
                case Strategy.CoinMiningBasis.Price:
                    orderedCoins = orderedCoins.OrderByDescending(c => c.Price).ToList();
                    break;
            }

            return orderedCoins;
        }

        private void ApplyProfitabilityAdjustments(IEnumerable<CoinInformation> coinInformation)
        {
            foreach (CoinInformation configuredProfitableCoin in coinInformation)
            {
                Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.CryptoCoin.Symbol.Equals(configuredProfitableCoin.Symbol));

                if (coinConfiguration.ProfitabilityAdjustmentType == Data.Configuration.Coin.AdjustmentType.Addition)
                {
                    configuredProfitableCoin.AdjustedProfitability += coinConfiguration.ProfitabilityAdjustment;
                    configuredProfitableCoin.AverageProfitability += coinConfiguration.ProfitabilityAdjustment;
                    configuredProfitableCoin.Profitability += coinConfiguration.ProfitabilityAdjustment;
                }
                else if (coinConfiguration.ProfitabilityAdjustmentType == Data.Configuration.Coin.AdjustmentType.Multiplication)
                {
                    configuredProfitableCoin.AdjustedProfitability *= coinConfiguration.ProfitabilityAdjustment;
                    configuredProfitableCoin.AverageProfitability *= coinConfiguration.ProfitabilityAdjustment;
                    configuredProfitableCoin.Profitability *= coinConfiguration.ProfitabilityAdjustment;
                }
            }
        }

        private List<Engine.Data.Configuration.Device> CreateAutomaticDeviceConfiguration(IEnumerable<CoinInformation> orderedCoinInformation)
        {
            //order by adjusted profitability
            List<CoinInformation> filteredCoinInformation = GetFilteredCoinInformation(orderedCoinInformation);

            //get algorithm only options
            List<CoinInformation> sha256ProfitableCoins = filteredCoinInformation.Where(c => c.Algorithm.Equals(AlgorithmNames.SHA256, StringComparison.OrdinalIgnoreCase)).ToList();
            List<CoinInformation> scryptProfitableCoins = filteredCoinInformation.Where(c => c.Algorithm.Equals(AlgorithmNames.Scrypt, StringComparison.OrdinalIgnoreCase)).ToList();


            //ABM - always be mining
            if (filteredCoinInformation.Count == 0)
                filteredCoinInformation.Add(orderedCoinInformation.First());

            if (sha256ProfitableCoins.Count == 0)
            {
                CoinInformation sha256Coin = orderedCoinInformation.Where(c => c.Algorithm.Equals(AlgorithmNames.SHA256)).FirstOrDefault();
                if (sha256Coin != null)
                    sha256ProfitableCoins.Add(sha256Coin);
            }

            if (scryptProfitableCoins.Count == 0)
            {
                CoinInformation scryptCoin = orderedCoinInformation.Where(c => c.Algorithm.Equals(AlgorithmNames.Scrypt)).FirstOrDefault();
                if (scryptCoin != null)
                    scryptProfitableCoins.Add(scryptCoin);
            }
            //end ABM

            return CreateDeviceConfigurationForProfitableCoins(filteredCoinInformation, sha256ProfitableCoins, scryptProfitableCoins);
        }

        private List<Engine.Data.Configuration.Device> CreateDeviceConfigurationForProfitableCoins(List<CoinInformation> allProfitableCoins,
            List<CoinInformation> sha256ProfitableCoins, List<CoinInformation> scryptProfitableCoins)
        {
            List<Engine.Data.Configuration.Device> newConfiguration = new List<Engine.Data.Configuration.Device>();
            CoinInformation profitableCoin = null;

            int comboAlgoIterator = 0;
            int sha256Iterator = 0;
            int scryptIterator = 0;

            for (int i = 0; i < devices.Count; i++)
            {
                Xgminer.Data.Device device = devices[i];

                //there should be a 1-to-1 relationship of devices and device configurations
                Engine.Data.Configuration.Device existingConfiguration = engineConfiguration.DeviceConfigurations[i];

                if (existingConfiguration.Enabled)
                {
                    profitableCoin = null;

                    if (device.SupportsAlgorithm(CoinAlgorithm.Scrypt) && !device.SupportsAlgorithm(CoinAlgorithm.SHA256))
                    {
                        //scrypt only
                        profitableCoin = ChooseCoinFromList(scryptProfitableCoins, scryptIterator);

                        scryptIterator++;
                        if (scryptIterator >= scryptProfitableCoins.Count())
                            scryptIterator = 0;
                    }
                    else if (device.SupportsAlgorithm(CoinAlgorithm.Scrypt) && device.SupportsAlgorithm(CoinAlgorithm.SHA256))
                    {
                        //sha256 or scrypt
                        profitableCoin = ChooseCoinFromList(allProfitableCoins, comboAlgoIterator);

                        comboAlgoIterator++;
                        if (comboAlgoIterator >= allProfitableCoins.Count())
                            comboAlgoIterator = 0;
                    }
                    else if (sha256ProfitableCoins.Count > 0)
                    {
                        //sha256 only
                        profitableCoin = ChooseCoinFromList(sha256ProfitableCoins, sha256Iterator);

                        sha256Iterator++;
                        if (sha256Iterator >= sha256ProfitableCoins.Count())
                            sha256Iterator = 0;
                    }

                    Engine.Data.Configuration.Device configEntry = new Engine.Data.Configuration.Device();

                    configEntry.Assign(device);

                    configEntry.CoinSymbol = profitableCoin == null ? string.Empty : profitableCoin.Symbol;
                    
                    newConfiguration.Add(configEntry);
                }
                else
                {
                    Engine.Data.Configuration.Device configEntry = new Engine.Data.Configuration.Device();

                    configEntry.Assign(device);

                    configEntry.CoinSymbol = existingConfiguration.CoinSymbol;
                    configEntry.Enabled = false;

                    newConfiguration.Add(configEntry);
                }
            }

            return newConfiguration;
        }

        private CoinInformation ChooseCoinFromList(List<CoinInformation> coinList, int deviceIterator)
        {
            CoinInformation profitableCoin;

            bool mineSingle = engineConfiguration.StrategyConfiguration.SwitchStrategy == Strategy.CoinSwitchStrategy.SingleMost;

            if (!mineSingle && engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue.HasValue)
            {
                switch (engineConfiguration.StrategyConfiguration.MiningBasis)
                {
                    case Strategy.CoinMiningBasis.Profitability:
                        switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
                        {
                            case Strategy.CoinProfitabilityKind.AdjustedProfitability:
                                mineSingle = coinList.First().AdjustedProfitability > engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                                break;
                            case Strategy.CoinProfitabilityKind.AverageProfitability:
                                mineSingle = coinList.First().AverageProfitability > engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                                break;
                            case Strategy.CoinProfitabilityKind.StraightProfitability:
                                mineSingle = coinList.First().Profitability > engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                                break;
                        }
                        break;
                    case Strategy.CoinMiningBasis.Difficulty:
                        mineSingle = coinList.First().Difficulty < engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                        break;
                    case Strategy.CoinMiningBasis.Price:
                        mineSingle = coinList.First().Price > engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                        break;
                }                
            }

            if (mineSingle)
                profitableCoin = coinList.First();
            else
                profitableCoin = coinList[deviceIterator];

            return profitableCoin;
        }

        //filter the coin information list by MinimumThresholdValue and MinimumThresholdSymbol
        private List<CoinInformation> GetFilteredCoinInformation(IEnumerable<CoinInformation> unfilteredCoinInformation)
        {
            List<CoinInformation> filteredCoinInformation = unfilteredCoinInformation.ToList(); //call ToList to get a copy

            if (!string.IsNullOrEmpty(engineConfiguration.StrategyConfiguration.MinimumThresholdSymbol))
            {
                CoinInformation minimumCoin = filteredCoinInformation.SingleOrDefault(c => c.Symbol.Equals(engineConfiguration.StrategyConfiguration.MinimumThresholdSymbol));
                int index = filteredCoinInformation.IndexOf(minimumCoin);
                index++;
                filteredCoinInformation.RemoveRange(index, filteredCoinInformation.Count - index);
            }

            if (engineConfiguration.StrategyConfiguration.MinimumThresholdValue.HasValue)
            {
                double minimumValue = engineConfiguration.StrategyConfiguration.MinimumThresholdValue.Value;

                switch (engineConfiguration.StrategyConfiguration.MiningBasis)
                {
                    case Strategy.CoinMiningBasis.Profitability:
                        switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
                        {
                            case Strategy.CoinProfitabilityKind.AdjustedProfitability:
                                filteredCoinInformation = filteredCoinInformation.Where(c => c.AdjustedProfitability > minimumValue).ToList();
                                break;
                            case Strategy.CoinProfitabilityKind.AverageProfitability:
                                filteredCoinInformation = filteredCoinInformation.Where(c => c.AverageProfitability > minimumValue).ToList();
                                break;
                            case Strategy.CoinProfitabilityKind.StraightProfitability:
                                filteredCoinInformation = filteredCoinInformation.Where(c => c.Profitability > minimumValue).ToList();
                                break;
                        }
                        break;
                    case Strategy.CoinMiningBasis.Difficulty:
                        filteredCoinInformation = filteredCoinInformation.Where(c => c.Difficulty < minimumValue).ToList();
                        break;
                    case Strategy.CoinMiningBasis.Price:
                        filteredCoinInformation = filteredCoinInformation.Where(c => c.Price > minimumValue).ToList();
                        break;
                }
            }

            return filteredCoinInformation;
        }

        private static bool DeviceConfigurationsDiffer(List<Engine.Data.Configuration.Device> configuration1, List<Engine.Data.Configuration.Device> configuration2)
        {
            bool configDifferent = configuration1.Count != configuration2.Count;

            if (!configDifferent)
            {
                for (int i = 0; i < configuration1.Count; i++)
                {
                    Engine.Data.Configuration.Device entry1 = configuration1[i];
                    Engine.Data.Configuration.Device entry2 = configuration2[i];

                    configDifferent = (!entry1.CoinSymbol.Equals(entry2.CoinSymbol)
                        || (entry1.Kind != entry2.Kind)
                        || (entry1.Driver != entry2.Driver)
                        || (entry1.Path != entry2.Path));
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

            int port = engineConfiguration.XgminerConfiguration.StartingApiPort;

            foreach (string coinSymbol in coinSymbols)
            {
                //launch separate processes for CPU & GPU vs USB & PXY (for stability)
                Xgminer.Data.Configuration.Miner minerConfiguration = CreateMinerConfiguration(port, coinSymbol, DeviceKind.CPU | DeviceKind.GPU);
                if (minerConfiguration != null)
                {
                    Process process = LaunchMinerProcess(minerConfiguration, "Starting mining");
                    if (!process.HasExited)
                    {
                        MinerProcess minerProcess = StoreMinerProcess(process, coinSymbol, minerConfiguration, port);
                        minerProcess.TerminateProcess = engineConfiguration.XgminerConfiguration.TerminateGpuMiners;
                    }

                    port++;
                }

                minerConfiguration = CreateMinerConfiguration(port, coinSymbol, DeviceKind.PXY | DeviceKind.USB);
                if (minerConfiguration != null)
                {
                    Process process = LaunchMinerProcess(minerConfiguration, "Starting mining");
                    if (!process.HasExited)
                        StoreMinerProcess(process, coinSymbol, minerConfiguration, port);

                    port++;
                }
            }

            mining = true;
        }

        private MinerProcess StoreMinerProcess(Process process, string coinSymbol, Xgminer.Data.Configuration.Miner minerConfiguration, int port)
        {
            MinerProcess minerProcess = new MinerProcess();

            minerProcess.Process = process;
            minerProcess.ApiPort = port;
            minerProcess.MinerConfiguration = minerConfiguration;
            minerProcess.CoinSymbol = coinSymbol;

            setupProcessStartInfo(minerProcess);

            minerProcesses.Add(minerProcess);

            return minerProcess;
        }

        private Process LaunchMinerProcess(Xgminer.Data.Configuration.Miner minerConfiguration, string reason)
        {
            minerConfiguration.Priority = this.engineConfiguration.XgminerConfiguration.Priority;
            Xgminer.Miner miner = new Xgminer.Miner(minerConfiguration);
            miner.LogLaunch += this.LogProcessLaunch;
            miner.LaunchFailed += this.ProcessLaunchFailed;
            miner.AuthenticationFailed += this.ProcessAuthenticationFailed;
            Process process = miner.Launch(reason);
            return process;
        }

        private Xgminer.Data.Configuration.Miner CreateMinerConfiguration(int port, string coinSymbol, DeviceKind includeKinds)
        {
            Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.CryptoCoin.Symbol.Equals(coinSymbol));
            if (coinConfiguration.Pools.Count == 0)
                // no pools configured
                return null;

            IList<Engine.Data.Configuration.Device> enabledConfigurations = engineConfiguration.DeviceConfigurations.Where(c => c.Enabled && c.CoinSymbol.Equals(coinSymbol)).ToList();

            Xgminer.Data.Configuration.Miner minerConfiguration = new Xgminer.Data.Configuration.Miner() 
            { 
                ExecutablePath = MinerPath.GetPathToInstalledMiner(), 
                Algorithm = coinConfiguration.CryptoCoin.Algorithm, 
                ApiPort = port, 
                ApiListen = true, 
                AllowedApiIps = engineConfiguration.XgminerConfiguration.AllowedApiIps, 
                CoinName = coinConfiguration.CryptoCoin.Name, 
                DisableGpu = engineConfiguration.XgminerConfiguration.DisableGpu 
            };
            
            SetupConfigurationPools(minerConfiguration, coinConfiguration);

            int deviceCount = SetupConfigurationDevices(minerConfiguration, includeKinds, enabledConfigurations);
            if (deviceCount == 0)
                return null;

            SetupConfigurationArguments(minerConfiguration, coinConfiguration);

            return minerConfiguration;
        }

        private void SetupConfigurationArguments(Xgminer.Data.Configuration.Miner minerConfiguration, Data.Configuration.Coin coinConfiguration)
        {
            string arguments = string.Empty;

            //apply algorithm-specific parameters
            if (engineConfiguration.XgminerConfiguration.AlgorithmFlags.ContainsKey(coinConfiguration.CryptoCoin.Algorithm))
                arguments = String.Format("{0} {1}", arguments,
                    engineConfiguration.XgminerConfiguration.AlgorithmFlags[coinConfiguration.CryptoCoin.Algorithm]);

            //apply coin-specific parameters
            if (!string.IsNullOrEmpty(coinConfiguration.MinerFlags))
                arguments = string.Format("{0} {1}", arguments, coinConfiguration.MinerFlags);

            if (engineConfiguration.XgminerConfiguration.DesktopMode)
                arguments = arguments + " -I D";

            if (donationPercent > 0)
                arguments = arguments + " --load-balance";

            minerConfiguration.LaunchArguments = arguments;
        }

        private int SetupConfigurationDevices(Xgminer.Data.Configuration.Miner minerConfiguration, DeviceKind deviceKinds, IList<Engine.Data.Configuration.Device> deviceConfigurations)
        {
            int deviceCount = 0;
            for (int i = 0; i < deviceConfigurations.Count; i++)
            {
                Engine.Data.Configuration.Device enabledConfiguration = deviceConfigurations[i];

                Xgminer.Data.Device device = devices.SingleOrDefault(d => d.Equals(enabledConfiguration));

                if ((deviceKinds & device.Kind) == 0)
                    continue;

                deviceCount++;

                //don't actually add stratum device as a device index
                if (device.Kind != DeviceKind.PXY)
                {
                    minerConfiguration.DeviceDescriptors.Add(device);
                }
                else
                {
                    //only enable the stratum proxy if these devices contain the PXY device
                    minerConfiguration.StratumProxy = engineConfiguration.XgminerConfiguration.StratumProxy;
                    minerConfiguration.StratumProxyPort = engineConfiguration.XgminerConfiguration.StratumProxyPort;
                    minerConfiguration.StratumProxyStratumPort = engineConfiguration.XgminerConfiguration.StratumProxyStratumPort;
                }
            }
            return deviceCount;
        }

        private void SetupConfigurationPools(Xgminer.Data.Configuration.Miner minerConfiguration, Data.Configuration.Coin coinConfiguration)
        {
            //minerConfiguration.Pools = coinConfiguration.Pools;
            foreach (MiningPool pool in coinConfiguration.Pools)
            {
                pool.Quota = 0;
                minerConfiguration.Pools.Add(pool);
            }

            //using bfgminer quotas for failover, that way we can augment for donations
            minerConfiguration.Pools.First().Quota = 100 - donationPercent;
            if (donationPercent > 0)
                AddDonationPool(coinConfiguration.CryptoCoin.Symbol, minerConfiguration);

            foreach (MiningPool pool in minerConfiguration.Pools)
                pool.QuotaEnabled = donationPercent > 0;
        }

        private readonly List<Data.Configuration.Coin> donationConfigurations = InitializeDonationConfigurations();

        private static List<Data.Configuration.Coin> InitializeDonationConfigurations()
        {
            List<Data.Configuration.Coin> result = new List<Data.Configuration.Coin>();
            Helpers.DonationPools.Seed(result);
            return result;
        }

        private readonly Random random = new Random(Guid.NewGuid().GetHashCode()); //seed so we don't keep getting the same indexes
        private void AddDonationPool(string coinSymbol, Xgminer.Data.Configuration.Miner minerConfiguration)
        {
            MiningPool donationPool = null;

            Data.Configuration.Coin donationConfiguration = this.donationConfigurations.SingleOrDefault(dc => dc.CryptoCoin.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            if (donationConfiguration != null)
            {
                //inclusive lower, exclusive upper
                //int index = random.Next(0, donationConfiguration.Pools.Count);

                //for now just use the first pool, too many donors is *not* an issue
                //if it becomes an issue we can revert to the above
                int index = 0;
                donationPool = donationConfiguration.Pools[index];
            }

            if (donationPool != null)
            {
                donationPool.Quota = donationPercent;
                minerConfiguration.Pools.Add(donationPool);
            }
        }
    }
}
