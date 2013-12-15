using MultiMiner.Coin.Api;
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
        public event Miner.AuthenticationFailedHandler ProcessAuthenticationFailed;
        private List<MinerProcess> minerProcesses = new List<MinerProcess>();
        private EngineConfiguration engineConfiguration;
        private List<Device> devices;
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
        public void StartMining(EngineConfiguration engineConfiguration, List<Device> devices, List<CoinInformation> coinInformation, int donationPercent)
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
                    logProcessClose(minerProcess);
                    minerProcess.StopMining();
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Dead device");
                    setupProcessStartInfo(minerProcess);
                }

                else if (minerProcess.HasSickDevice)
                {
                    logProcessClose(minerProcess);
                    minerProcess.StopMining();
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Sick device");
                    setupProcessStartInfo(minerProcess);
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
                    }
                }
            }
        }

        private void setupProcessStartInfo(MinerProcess minerProcess)
        {
            string coinName = minerProcess.MinerConfiguration.CoinName;
            string coinSymbol = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase)).Coin.Symbol;

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
                //get a list of the coins that are enabled and have at least one pool
                IEnumerable<string> configuredSymbols = engineConfiguration.CoinConfigurations.Where(c => c.Enabled && (c.Pools.Count > 0)).Select(c => c.Coin.Symbol);

                //filter the coin info by that list
                //use the copy here
                IEnumerable<CoinInformation> filteredCoinInformation = coinInformationCopy.Where(c => configuredSymbols.Contains(c.Symbol));

                if (filteredCoinInformation.Count() > 0)
                {
                    //adjust profitabilities based on config adjustments
                    ApplyProfitabilityAdjustments(filteredCoinInformation);

                    List<CoinInformation> orderedCoinInformation = GetCoinInformationOrderedByMiningBasis(filteredCoinInformation);

                    List<DeviceConfiguration> newConfiguration = CreateAutomaticDeviceConfiguration(orderedCoinInformation);

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

        private List<CoinInformation> GetCoinInformationOrderedByMiningBasis(IEnumerable<CoinInformation> configuredCoins)
        {
            List<CoinInformation> orderedCoins = configuredCoins.ToList();

            switch (engineConfiguration.StrategyConfiguration.MiningBasis)
            {
                case StrategyConfiguration.CoinMiningBasis.Profitability:
                    switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
                    {
                        case StrategyConfiguration.CoinProfitabilityKind.AdjustedProfitability:
                            orderedCoins = orderedCoins.OrderByDescending(c => c.AdjustedProfitability).ToList();
                            break;
                        case StrategyConfiguration.CoinProfitabilityKind.AverageProfitability:
                            orderedCoins = orderedCoins.OrderByDescending(c => c.AverageProfitability).ToList();
                            break;
                        case StrategyConfiguration.CoinProfitabilityKind.StraightProfitability:
                            orderedCoins = orderedCoins.OrderByDescending(c => c.Profitability).ToList();
                            break;
                    }
                    break;
                case StrategyConfiguration.CoinMiningBasis.Difficulty:
                    orderedCoins = orderedCoins.OrderBy(c => c.Difficulty).ToList();
                    break;
                case StrategyConfiguration.CoinMiningBasis.Price:
                    orderedCoins = orderedCoins.OrderByDescending(c => c.Price).ToList();
                    break;
            }

            return orderedCoins;
        }

        private void ApplyProfitabilityAdjustments(IEnumerable<CoinInformation> coinInformation)
        {
            foreach (CoinInformation configuredProfitableCoin in coinInformation)
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

        private List<DeviceConfiguration> CreateAutomaticDeviceConfiguration(IEnumerable<CoinInformation> orderedCoinInformation)
        {
            //order by adjusted profitability
            List<CoinInformation> filteredCoinInformation = GetFilteredCoinInformation(orderedCoinInformation);

            //get sha256 only options
            List<CoinInformation> sha256ProfitableCoins = filteredCoinInformation.Where(c => c.Algorithm.Equals("SHA-256")).ToList();

            //ABM - always be mining
            if (filteredCoinInformation.Count == 0)
                filteredCoinInformation.Add(orderedCoinInformation.First());

            if (sha256ProfitableCoins.Count == 0)
            {
                CoinInformation sha256Info = orderedCoinInformation.Where(c => c.Algorithm.Equals("SHA-256")).FirstOrDefault();
                if (sha256Info != null)
                    sha256ProfitableCoins.Add(sha256Info);
            }
            //end ABM

            return CreateDeviceConfigurationForProfitableCoins(filteredCoinInformation, sha256ProfitableCoins);
        }

        private List<DeviceConfiguration> CreateDeviceConfigurationForProfitableCoins(List<CoinInformation> allProfitableCoins, List<CoinInformation> sha256ProfitableCoins)
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
                        profitableCoin = ChooseCoinFromList(allProfitableCoins, gpuIterator);

                        gpuIterator++;
                        if (gpuIterator >= allProfitableCoins.Count())
                            gpuIterator = 0;
                    }
                    else if (sha256ProfitableCoins.Count > 0)
                    {
                        //sha256 only
                        profitableCoin = ChooseCoinFromList(sha256ProfitableCoins, amuIterator);

                        amuIterator++;
                        if (amuIterator >= sha256ProfitableCoins.Count())
                            amuIterator = 0;
                    }

                    DeviceConfiguration configEntry = new DeviceConfiguration();

                    configEntry.Assign(device);

                    configEntry.CoinSymbol = profitableCoin == null ? string.Empty : profitableCoin.Symbol;
                    
                    newConfiguration.Add(configEntry);
                }
                else
                {
                    DeviceConfiguration configEntry = new DeviceConfiguration();

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

            bool mineSingle = engineConfiguration.StrategyConfiguration.SwitchStrategy == StrategyConfiguration.CoinSwitchStrategy.SingleMost;

            if (!mineSingle && engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue.HasValue)
            {
                switch (engineConfiguration.StrategyConfiguration.MiningBasis)
                {
                    case StrategyConfiguration.CoinMiningBasis.Profitability:
                        switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
                        {
                            case StrategyConfiguration.CoinProfitabilityKind.AdjustedProfitability:
                                mineSingle = coinList.First().AdjustedProfitability > engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                                break;
                            case StrategyConfiguration.CoinProfitabilityKind.AverageProfitability:
                                mineSingle = coinList.First().AverageProfitability > engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                                break;
                            case StrategyConfiguration.CoinProfitabilityKind.StraightProfitability:
                                mineSingle = coinList.First().Profitability > engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                                break;
                        }
                        break;
                    case StrategyConfiguration.CoinMiningBasis.Difficulty:
                        mineSingle = coinList.First().Difficulty < engineConfiguration.StrategyConfiguration.MineSingleMostOverrideValue;
                        break;
                    case StrategyConfiguration.CoinMiningBasis.Price:
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
                    case StrategyConfiguration.CoinMiningBasis.Profitability:
                        switch (engineConfiguration.StrategyConfiguration.ProfitabilityKind)
                        {
                            case StrategyConfiguration.CoinProfitabilityKind.AdjustedProfitability:
                                filteredCoinInformation = filteredCoinInformation.Where(c => c.AdjustedProfitability > minimumValue).ToList();
                                break;
                            case StrategyConfiguration.CoinProfitabilityKind.AverageProfitability:
                                filteredCoinInformation = filteredCoinInformation.Where(c => c.AverageProfitability > minimumValue).ToList();
                                break;
                            case StrategyConfiguration.CoinProfitabilityKind.StraightProfitability:
                                filteredCoinInformation = filteredCoinInformation.Where(c => c.Profitability > minimumValue).ToList();
                                break;
                        }
                        break;
                    case StrategyConfiguration.CoinMiningBasis.Difficulty:
                        filteredCoinInformation = filteredCoinInformation.Where(c => c.Difficulty < minimumValue).ToList();
                        break;
                    case StrategyConfiguration.CoinMiningBasis.Price:
                        filteredCoinInformation = filteredCoinInformation.Where(c => c.Price > minimumValue).ToList();
                        break;
                }
            }

            return filteredCoinInformation;
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
                MinerConfiguration minerConfiguration = CreateMinerConfiguration(port, coinSymbol, DeviceKind.CPU | DeviceKind.GPU);
                if (minerConfiguration != null)
                {
                    Process process = LaunchMinerProcess(minerConfiguration, "Starting mining");
                    if (!process.HasExited)
                        StoreMinerProcess(process, minerConfiguration, port);

                    port++;
                }

                minerConfiguration = CreateMinerConfiguration(port, coinSymbol, DeviceKind.PXY | DeviceKind.USB);
                if (minerConfiguration != null)
                {
                    Process process = LaunchMinerProcess(minerConfiguration, "Starting mining");
                    if (!process.HasExited)
                        StoreMinerProcess(process, minerConfiguration, port);

                    port++;
                }
            }

            mining = true;
        }

        private void StoreMinerProcess(Process process, MinerConfiguration minerConfiguration, int port)
        {
            MinerProcess minerProcess = new MinerProcess();

            minerProcess.Process = process;
            minerProcess.ApiPort = port;
            minerProcess.MinerConfiguration = minerConfiguration;

            setupProcessStartInfo(minerProcess);

            minerProcesses.Add(minerProcess);
        }

        private Process LaunchMinerProcess(MinerConfiguration minerConfiguration, string reason)
        {
            minerConfiguration.Priority = this.engineConfiguration.XgminerConfiguration.Priority;
            Miner miner = new Miner(minerConfiguration);
            miner.LogLaunch += this.LogProcessLaunch;
            miner.LaunchFailed += this.ProcessLaunchFailed;
            miner.AuthenticationFailed += this.ProcessAuthenticationFailed;
            Process process = miner.Launch(reason);
            return process;
        }

        private MinerConfiguration CreateMinerConfiguration(int port, string coinSymbol, DeviceKind includeKinds)
        {
            CoinConfiguration coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.Coin.Symbol.Equals(coinSymbol));

            IList<DeviceConfiguration> enabledConfigurations = engineConfiguration.DeviceConfigurations.Where(c => c.Enabled && c.CoinSymbol.Equals(coinSymbol)).ToList();

            MinerConfiguration minerConfiguration = new MinerConfiguration();

            minerConfiguration.ExecutablePath = MinerPath.GetPathToInstalledMiner();

            //minerConfiguration.Pools = coinConfiguration.Pools;
            foreach (MiningPool pool in coinConfiguration.Pools)
            {
                pool.Quota = 0;
                minerConfiguration.Pools.Add(pool);
            }

            //using bfgminer quotas for failover, that way we can augment for donations
            minerConfiguration.Pools.First().Quota = 100 - donationPercent;
            if (donationPercent > 0)
                AddDonationPool(coinSymbol, minerConfiguration);

            minerConfiguration.Algorithm = coinConfiguration.Coin.Algorithm;
            minerConfiguration.ApiPort = port;
            minerConfiguration.ApiListen = true;
            minerConfiguration.AllowedApiIps = engineConfiguration.XgminerConfiguration.AllowedApiIps;
            minerConfiguration.CoinName = coinConfiguration.Coin.Name;
            minerConfiguration.DisableGpu = engineConfiguration.XgminerConfiguration.DisableGpu;

            int deviceCount = 0;
            for (int i = 0; i < enabledConfigurations.Count; i++)
            {
                DeviceConfiguration enabledConfiguration = enabledConfigurations[i];

                Device device = devices.SingleOrDefault(d => d.Equals(enabledConfiguration));

                if ((includeKinds & device.Kind) == 0)
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

            if (deviceCount == 0)
                return null;

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

            if (donationPercent > 0)
                arguments = arguments + " --load-balance";

            minerConfiguration.LaunchArguments = arguments;

            return minerConfiguration;
        }

        private readonly List<CoinConfiguration> donationConfigurations = InitializeDonationConfigurations();

        private static List<CoinConfiguration> InitializeDonationConfigurations()
        {
            List<CoinConfiguration> result = new List<CoinConfiguration>();

            //BTC
            CoinConfiguration donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "BTC";

            MiningPool donationPool = new MiningPool() 
            { 
                Host = "stratum+tcp://stratum.mining.eligius.st", 
                Port = 3334, 
                Username = "1LKwyLK4KhojsJUEvUx8bEmnmjohNMjRDM",
                Password = "X" 
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool() 
            { 
                Host = "stratum+tcp://mint.bitminter.com", 
                Port = 3333, 
                Username = "nwoolls.mmdonations", 
                Password = "X" 
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool() 
            { 
                Host = "stratum+tcp://stratum.bitcoin.cz", 
                Port = 3333, 
                Username = "nwoolls.mmdonations", 
                Password = "X" 
            };
            donationConfiguration.Pools.Add(donationPool);

            result.Add(donationConfiguration);

            //LTC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "LTC";

            donationPool = new MiningPool() 
            { 
                Host = "stratum+tcp://world.wemineltc.com", 
                Port = 3333, 
                Username = "nwoolls.mmdonations", 
                Password = "X" 
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool() 
            { 
                Host = "stratum+tcp://primary.coinhuntr.com", 
                Port = 3333, 
                Username = "nwoolls.mmdonations", 
                Password = "X" 
            };
            donationConfiguration.Pools.Add(donationPool);

            result.Add(donationConfiguration);

            //BQC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "BQC";

            donationPool = new MiningPool()
            {
                Host = "http://de1.bigbbq.cc",
                Port = 8446,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            result.Add(donationConfiguration);

            //FTC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "FTC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://stratum.wemineftc.com",
                Port = 4444,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool()
            {
                Host = "http://feathercoin.is-a-geek.com",
                Port = 8341,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            result.Add(donationConfiguration);

            //MEC
            donationConfiguration = new CoinConfiguration();
            donationConfiguration.Coin.Symbol = "MEC";

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://us.miningpool.co",
                Port = 9002,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            donationPool = new MiningPool()
            {
                Host = "stratum+tcp://au1.miningpool.co",
                Port = 9001,
                Username = "nwoolls.mmdonations",
                Password = "X"
            };
            donationConfiguration.Pools.Add(donationPool);

            result.Add(donationConfiguration);

            return result;
        }

        private readonly Random random = new Random(Guid.NewGuid().GetHashCode()); //seed so we don't keep getting the same indexes
        private void AddDonationPool(string coinSymbol, MinerConfiguration minerConfiguration)
        {
            MiningPool donationPool = null;

            CoinConfiguration donationConfiguration = this.donationConfigurations.SingleOrDefault(dc => dc.Coin.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
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
