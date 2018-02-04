using MultiMiner.CoinApi.Data;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Utility.Serialization;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MultiMiner.Engine.Data;
using System.IO;

namespace MultiMiner.Engine
{
    public class MiningEngine
    {
        public const string AdvancedProxiesRequirePerksMessage = "Mining with multiple Stratum Proxies requires Perks to be enabled.";
        //events
        //delegate declarations
        public delegate void LogProcessCloseHandler(object sender, LogProcessCloseArgs ea);

        //event declarations        
        public event LogProcessCloseHandler LogProcessClose;
        public event Xgminer.Miner.LogLaunchHandler LogProcessLaunch;
        public event Xgminer.Miner.LaunchFailedHandler ProcessLaunchFailed;
        public event Xgminer.Miner.AuthenticationFailedHandler ProcessAuthenticationFailed;
        private readonly List<MinerProcess> minerProcesses = new List<MinerProcess>();
        private Data.Configuration.Engine engineConfiguration;
        private List<Xgminer.Data.Device> devices;
        private int donationPercent;

        public MiningEngine()
        {
            RegisterMiners();
            RegisterAlgorithms();
        }

        private static void RegisterAlgorithms()
        {
            RegisterBuiltInAgorithms();

            RegisterCustomAlgorithms();

            SaveAlgorithmConfigurations();
        }

        private static void RegisterCustomAlgorithms()
        {
            string dataFilePath = AlgorithmConfigurationsFileName();
            List<CoinAlgorithm> existingAlgorithms = MinerFactory.Instance.Algorithms;

            if (File.Exists(dataFilePath))
            {
                List<CoinAlgorithm> customAlgorithms = ConfigurationReaderWriter.ReadConfiguration<List<CoinAlgorithm>>(dataFilePath);
                foreach (CoinAlgorithm customAlgorithm in customAlgorithms)
                {
                    CoinAlgorithm existingAlgorithm = existingAlgorithms.SingleOrDefault(ea => ea.Name.Equals(customAlgorithm.Name, StringComparison.OrdinalIgnoreCase));
                    if (existingAlgorithm != null)
                    {
                        //don't override BuiltIn - user may have added an algo before we support it
                        //don't override Multipliers - user may have added an algo before we introduced these
                        ObjectCopier.CopyObject(customAlgorithm, existingAlgorithm, "BuiltIn", "PoolMultiplier", "DifficultyMultiplier");

                        //we still need to support the user customizing these while also introducing defaults for OOTB
                        if (customAlgorithm.PoolMultiplier > 0)
                            existingAlgorithm.PoolMultiplier = customAlgorithm.PoolMultiplier;

                        if (customAlgorithm.DifficultyMultiplier > 0)
                            existingAlgorithm.DifficultyMultiplier = customAlgorithm.DifficultyMultiplier;
                    }
                    else
                        existingAlgorithms.Add(customAlgorithm);
                }
            }
        }

        public static void SaveAlgorithmConfigurations()
        {
            ConfigurationReaderWriter.WriteConfiguration(MinerFactory.Instance.Algorithms, AlgorithmConfigurationsFileName());
        }

        private static string AlgorithmConfigurationsFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "AlgorithmConfigurations.xml");
        }

        private static void RegisterBuiltInAgorithms()
        {
            MinerFactory factory = MinerFactory.Instance;

            CoinAlgorithm algorithm = factory.RegisterAlgorithm(AlgorithmNames.SHA256, AlgorithmFullNames.SHA256);
            algorithm.PoolMultiplier = 1;
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.BFGMiner;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Scrypt, AlgorithmFullNames.Scrypt);
            algorithm.PoolMultiplier = Math.Pow(2, 16);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.BFGMiner;
            algorithm.MinerArguments[MinerNames.BFGMiner] = AlgorithmParameter.Scrypt;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Keccak, AlgorithmFullNames.Keccak);
            algorithm.PoolMultiplier = Math.Pow(2, 8);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmKeccak;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Groestl, AlgorithmFullNames.Groestl);
            algorithm.PoolMultiplier = Math.Pow(2, 8);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmGroestlcoin;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.ScryptN, AlgorithmFullNames.ScryptN);
            algorithm.PoolMultiplier = Math.Pow(2, 16);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmScryptNFactor11;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.ScryptJane, AlgorithmFullNames.ScryptJane);
            algorithm.PoolMultiplier = Math.Pow(2, 16);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.KalrothSJCGMiner;
            algorithm.MinerArguments[MinerNames.KalrothSJCGMiner] = AlgorithmParameter.ScryptJane;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Quark, AlgorithmFullNames.Quark);
            algorithm.PoolMultiplier = 1;
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmQuarkcoin;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.X11, AlgorithmFullNames.X11);
            algorithm.PoolMultiplier = 1;
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmX11;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.X13, AlgorithmFullNames.X13);
            algorithm.PoolMultiplier = 1;
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmX13;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.X14, AlgorithmFullNames.X14);
            algorithm.PoolMultiplier = 1;
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmX14;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.X15, AlgorithmFullNames.X15);
            algorithm.PoolMultiplier = 1;
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmX15;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.NeoScrypt, AlgorithmFullNames.NeoScrypt);
            algorithm.PoolMultiplier = Math.Pow(2, 16);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmNeoScrypt;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Lyra2RE, AlgorithmFullNames.Lyra2RE);
            algorithm.PoolMultiplier = Math.Pow(2, 7);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMiner5;
            algorithm.MinerArguments[MinerNames.SGMiner5] = AlgorithmParameter.AlgorithmLyra2RE;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Ethash, AlgorithmFullNames.Ethash);
            algorithm.PoolMultiplier = Math.Pow(2, 32);
            algorithm.DifficultyMultiplier = 1;
            algorithm.DefaultMiner = MinerNames.SGMinerGM;
            algorithm.MinerArguments[MinerNames.SGMinerGM] = AlgorithmParameter.AlgorithmEthash;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.CryptoNight, AlgorithmFullNames.CryptoNight);
            algorithm.PoolMultiplier = Math.Pow(2, 32);
            algorithm.DifficultyMultiplier = 1;
            algorithm.DefaultMiner = MinerNames.SGMinerGM;
            algorithm.MinerArguments[MinerNames.SGMinerGM] = AlgorithmParameter.AlgorithmCryptoNight;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Equihash, AlgorithmFullNames.Equihash);
            algorithm.PoolMultiplier = Math.Pow(2, 20);
            algorithm.DifficultyMultiplier = Math.Pow(2, 13);
            algorithm.DefaultMiner = MinerNames.SGMinerGM;
            algorithm.MinerArguments[MinerNames.SGMinerGM] = AlgorithmParameter.AlgorithmEquihash;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Pascal, AlgorithmFullNames.Pascal);
            algorithm.PoolMultiplier = 1;
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMinerPascal;
            algorithm.MinerArguments[MinerNames.SGMinerPascal] = AlgorithmParameter.AlgorithmPascal;
            algorithm.BuiltIn = true;

            // required editing lbry.cl, ripemd160.cl and wolf-sha512.cl
            algorithm = factory.RegisterAlgorithm(AlgorithmNames.LBRY, AlgorithmFullNames.LBRY);
            algorithm.PoolMultiplier = Math.Pow(2, 9);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMinerNH;
            algorithm.MinerArguments[MinerNames.SGMinerNH] = AlgorithmParameter.AlgorithmLbry;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Lyra2REv2, AlgorithmFullNames.Lyra2REv2);
            algorithm.PoolMultiplier = Math.Pow(2, 9);
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMinerNH;
            algorithm.MinerArguments[MinerNames.SGMinerNH] = AlgorithmParameter.AlgorithmLyra2REv2;
            algorithm.BuiltIn = true;

            algorithm = factory.RegisterAlgorithm(AlgorithmNames.Nist5, AlgorithmFullNames.Nist5);
            algorithm.PoolMultiplier = 1;
            algorithm.DifficultyMultiplier = Math.Pow(2, 32);
            algorithm.DefaultMiner = MinerNames.SGMinerNH;
            algorithm.MinerArguments[MinerNames.SGMinerNH] = AlgorithmParameter.AlgorithmNist5;
            algorithm.BuiltIn = true;
        }

        private static void RegisterMiners()
        {
            MinerFactory factory = MinerFactory.Instance;

            factory.RegisterMiner(MinerNames.BFGMiner, MinerNames.BFGMiner, false);
            factory.RegisterMiner(MinerNames.KalrothSJCGMiner, MinerNames.CGMiner, true);            
            factory.RegisterMiner(MinerNames.SGMiner, MinerNames.SGMiner, true);
            factory.RegisterMiner(MinerNames.LBSPHSGMiner, MinerNames.SGMiner, true);
            factory.RegisterMiner(MinerNames.AZNSGMiner, MinerNames.SGMiner, true);
            factory.RegisterMiner(MinerNames.SPHSGMiner, MinerNames.SGMiner, true);
            factory.RegisterMiner(MinerNames.MaxcoinCGMiner, MinerNames.CGMiner, true);
            factory.RegisterMiner(MinerNames.SGMiner5, MinerNames.SGMiner, true);
            factory.RegisterMiner(MinerNames.SGMinerGM, MinerNames.SGMiner, true);
            factory.RegisterMiner(MinerNames.SGMinerPascal, MinerNames.SGMiner, true);
            factory.RegisterMiner(MinerNames.SGMinerNH, MinerNames.SGMiner, true);

            string minersDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Miners");
            factory.RegisterMiners(minersDirectory);
        }

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

        public bool Mining { get; private set; }

        private void RestartMining()
        {
            StopMining();
            StartMining();
        }

        private bool startingMining;
        public void StartMining(Data.Configuration.Engine engineConfiguration, List<Xgminer.Data.Device> devices, List<CoinInformation> coinInformation, int donationPercent)
        {
            StopMining();

            startingMining = true;
            try
            {
                this.engineConfiguration = engineConfiguration;
                this.devices = devices;
                this.donationPercent = donationPercent;

                if (coinInformation != null) //null if no network connection
                    ApplyMiningStrategy(coinInformation);

                if (!Mining) //above call to ApplyMiningStrategy may have started mining due to config change
                    StartMining();

                Mining = true;
            }
            finally
            {
                startingMining = false;
            }
        }

        private bool stoppingMining;
        public void StopMining()
        {
            stoppingMining = true;
            try
            {
                foreach (MinerProcess minerProcess in minerProcesses)
                {
                    DoLogProcessClose(minerProcess);
                    minerProcess.StopMining();
                }

                minerProcesses.Clear();

                Mining = false;
            }
            finally
            {
                stoppingMining = false;
            }
        }

        public const int SecondsToWarmUpMiner = 60 * 5;
        
        public bool RelaunchCrashedMiners()
        {
            if (stoppingMining || startingMining)
                return false; //don't try to relaunch miners we are stopping or starting

            foreach (MinerProcess minerProcess in MinerProcesses)
            {
                if (minerProcess.Process.HasExited)
                {
                    DoLogProcessClose(minerProcess);
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Process crashed");
                    SetupProcessStartInfo(minerProcess);
                    return true;
                }

                if (minerProcess.HasDeadDevice)
                {
                    DoLogProcessClose(minerProcess);
                    minerProcess.StopMining();
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Dead device");
                    SetupProcessStartInfo(minerProcess);
                    return true;
                }

                if (minerProcess.HasSickDevice)
                {
                    DoLogProcessClose(minerProcess);
                    minerProcess.StopMining();
                    minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Sick device");
                    SetupProcessStartInfo(minerProcess);
                    return true;
                }

                if (minerProcess.HasZeroHashrateDevice || minerProcess.MinerIsFrozen || minerProcess.HasPoorPerformingDevice)
                {
                    TimeSpan processAge = DateTime.Now - minerProcess.Process.StartTime;
                    //this needs to give the devices long enough to spin up
                    //making this longer (than 120) on 12/10 to account for proxy devices that take longer
                    //to warm up, such as Raspberry Pi
                    if (processAge.TotalSeconds > SecondsToWarmUpMiner)
                    {
                        DoLogProcessClose(minerProcess);
                        minerProcess.StopMining();
                        string reason = minerProcess.HasZeroHashrateDevice ? "Zero hashrate" : minerProcess.HasPoorPerformingDevice ? "Subpar hashrate" : "Frozen miner";
                        minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, reason);
                        SetupProcessStartInfo(minerProcess);
                        return true;
                    }
                }

                else if (minerProcess.StoppedAcceptingShares)
                {
                    TimeSpan processAge = DateTime.Now - minerProcess.Process.StartTime;
                    //this needs to give the devices long enough to spin up
                    //AND get accepted shares
                    //AND have luck play out...
                    if (processAge.TotalSeconds > (30 * 60))
                    {
                        DoLogProcessClose(minerProcess);
                        minerProcess.StopMining();
                        minerProcess.Process = LaunchMinerProcess(minerProcess.MinerConfiguration, "Subpar shares");
                        SetupProcessStartInfo(minerProcess);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SetupProcessStartInfo(MinerProcess minerProcess)
        {
            string coinName = minerProcess.MinerConfiguration.CoinName;
            string coinSymbol = engineConfiguration.CoinConfigurations.Single(c => c.PoolGroup.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase)).PoolGroup.Id;

            CoinInformation processCoinInfo = null;
            if (coinInformation != null) //null if no network connection
                processCoinInfo = coinInformation.SingleOrDefault(c => c.Symbol.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
            
            //coin may not be in Coin API
            if (processCoinInfo != null)
                minerProcess.CoinInformation = processCoinInfo;
            
            minerProcess.StartDate = DateTime.Now;
        }

        private void DoLogProcessClose(MinerProcess minerProcess)
        {
            if (LogProcessClose == null)
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

            LogProcessCloseArgs args = new LogProcessCloseArgs
            {
                StartDate = startDate,
                EndDate = endDate,
                CoinName = coinName,
                CoinSymbol = coinSymbol,
                StartPrice = priceAtStart,
                EndPrice = priceAtEnd,
                DeviceDescriptors = deviceDescriptors,
                MinerConfiguration = minerProcess.MinerConfiguration,
                AcceptedShares = minerProcess.AcceptedShares
            };
            LogProcessClose(this, args);
        }

        private List<CoinInformation> coinInformation;
        //update engineConfiguration.DeviceConfiguration based on mining strategy & coin info
        public bool ApplyMiningStrategy(List<CoinInformation> coinInformation)
        {
            if (coinInformation == null) //null if no network connection
                return false;

            bool changed = false;

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
                    ).Select(c => c.PoolGroup.Id);

                //filter the coin info by that list
                //use the copy here
                IEnumerable<CoinInformation> filteredCoinInformation = coinInformationCopy.Where(c => configuredSymbols.Contains(c.Symbol));

                if (filteredCoinInformation.Any())
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

                    changed = configDifferent;
                }
            }

            return changed;
        }

        private static List<CoinInformation> CopyCoinInformation(List<CoinInformation> coinInformation)
        {
            List<CoinInformation> coinInformationCopy = new List<CoinInformation>();
            foreach (CoinInformation realCoin in coinInformation)
            {
                CoinInformation coinCopy = new CoinInformation();
                ObjectCopier.CopyObject(realCoin, coinCopy);
                coinInformationCopy.Add(coinCopy);
            }
            return coinInformationCopy;
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
                Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.PoolGroup.Id.Equals(configuredProfitableCoin.Symbol));

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
            List<CoinInformation> sha256ProfitableCoins = filteredCoinInformation.Where(c => c.Algorithm.Equals(AlgorithmFullNames.SHA256, StringComparison.OrdinalIgnoreCase)).ToList();
            List<CoinInformation> scryptProfitableCoins = filteredCoinInformation.Where(c => c.Algorithm.Equals(AlgorithmFullNames.Scrypt, StringComparison.OrdinalIgnoreCase)).ToList();


            //ABM - always be mining
            if (filteredCoinInformation.Count == 0)
                filteredCoinInformation.Add(orderedCoinInformation.First());

            if (sha256ProfitableCoins.Count == 0)
            {
                CoinInformation sha256Coin = orderedCoinInformation.FirstOrDefault(c => c.Algorithm.Equals(AlgorithmFullNames.SHA256));
                if (sha256Coin != null)
                    sha256ProfitableCoins.Add(sha256Coin);
            }

            if (scryptProfitableCoins.Count == 0)
            {
                CoinInformation scryptCoin = orderedCoinInformation.FirstOrDefault(c => c.Algorithm.Equals(AlgorithmFullNames.Scrypt));
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

                    if (device.Kind == DeviceKind.PXY)
                    {
						//we can only auto-switch a Proxy if we know the current algo
						//so we only consider PXY configurations with an existing Symbol
						if (!String.IsNullOrEmpty(existingConfiguration.CoinSymbol))
						{
							Data.Configuration.Coin existingCoinConfig = engineConfiguration.CoinConfigurations.Single(cc => cc.PoolGroup.Id.Equals(existingConfiguration.CoinSymbol, StringComparison.OrdinalIgnoreCase));

							//keep Proxies on the same algo - don't know what is pointed at them
							if (existingCoinConfig.PoolGroup.Algorithm.Equals(AlgorithmNames.Scrypt, StringComparison.OrdinalIgnoreCase))
							{
								profitableCoin = ChooseCoinFromList(scryptProfitableCoins, scryptIterator);

								scryptIterator++;
							}
							else
							{
								profitableCoin = ChooseCoinFromList(sha256ProfitableCoins, sha256Iterator);

								sha256Iterator++;
							}
						}
                    }
                    else if (device.SupportsAlgorithm(AlgorithmNames.Scrypt) && !device.SupportsAlgorithm(AlgorithmNames.SHA256) &&
                        (scryptProfitableCoins.Count > 0))
                    {
                        //scrypt only
                        profitableCoin = ChooseCoinFromList(scryptProfitableCoins, scryptIterator);

                        scryptIterator++;
                    }
                    else if (device.SupportsAlgorithm(AlgorithmNames.Scrypt) && device.SupportsAlgorithm(AlgorithmNames.SHA256) &&
                        (allProfitableCoins.Count > 0))
                    {
                        //sha256 or scrypt
                        profitableCoin = ChooseCoinFromList(allProfitableCoins, comboAlgoIterator);

                        comboAlgoIterator++;
                    }
                    else if (sha256ProfitableCoins.Count > 0)
                    {
                        //sha256 only
                        profitableCoin = ChooseCoinFromList(sha256ProfitableCoins, sha256Iterator);

                        sha256Iterator++;
                    }

                    if (comboAlgoIterator >= allProfitableCoins.Count())
                        comboAlgoIterator = 0;
                    if (sha256Iterator >= sha256ProfitableCoins.Count())
                        sha256Iterator = 0;
                    if (scryptIterator >= scryptProfitableCoins.Count())
                        scryptIterator = 0;

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

            int startingApiPort = engineConfiguration.XgminerConfiguration.StartingApiPort;

            foreach (string coinSymbol in coinSymbols)
            {
                LaunchGPUMiners(ref startingApiPort, coinSymbol);

                LaunchCPUMiners(ref startingApiPort, coinSymbol);

                LaunchUSBMiners(ref startingApiPort, coinSymbol);
            }

            //launch proxies separately now that we support any number of proxies pointed at any coin
            //e.g. could have two setup and pointed at BTC, cannot handle launching with above processes
            LaunchProxyMiners(startingApiPort);

            Mining = true;
        }

        private void LaunchProxyMiners(int apiPort)
        {
            IEnumerable<Xgminer.Data.Device> proxyDevices = devices.Where(d => d.Kind == DeviceKind.PXY);
            foreach (Xgminer.Data.Device proxyDevice in proxyDevices)
            {
                Data.Configuration.Device deviceConfiguration = engineConfiguration.DeviceConfigurations.Single(d => d.Equals(proxyDevice));

                //proxy is disabled
                if (!deviceConfiguration.Enabled)
                    continue;

                //no coin assigned
                if (String.IsNullOrEmpty(deviceConfiguration.CoinSymbol))
                    continue;

                Xgminer.Data.Configuration.Miner minerConfiguration = CreateProxyConfiguration(apiPort, deviceConfiguration.CoinSymbol);
                //null if no pools configured
                if (minerConfiguration != null)
                {
                    minerConfiguration.DeviceDescriptors.Add(proxyDevice);

                    minerConfiguration.StratumProxy = engineConfiguration.XgminerConfiguration.StratumProxy;

                    int index = Math.Max(0, proxyDevice.RelativeIndex);

                    if ((donationPercent == 0) && (index > 0))
                    {
                        throw new Xgminer.MinerLaunchException(AdvancedProxiesRequirePerksMessage);
                    }

                    MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor proxyDescriptor = engineConfiguration.XgminerConfiguration.StratumProxies[index];

                    minerConfiguration.StratumProxyPort = proxyDescriptor.GetworkPort;
                    minerConfiguration.StratumProxyStratumPort = proxyDescriptor.StratumPort;

                    Process process = LaunchMinerProcess(minerConfiguration, "Starting mining");
                    if (!process.HasExited)
                        StoreMinerProcess(process, MinerFactory.Instance.GetDefaultMiner(), deviceConfiguration.CoinSymbol, minerConfiguration, apiPort);

                    apiPort++;
                }
            }
        }

        private void LaunchCPUMiners(ref int apiPort, string coinSymbol)
        {
            LaunchMiners(ref apiPort, coinSymbol, DeviceKind.CPU, false);
        }

        private void LaunchGPUMiners(ref int apiPort, string coinSymbol)
        {
            LaunchMiners(ref apiPort, coinSymbol, DeviceKind.GPU,
                engineConfiguration.XgminerConfiguration.TerminateGpuMiners);
        }

        private void LaunchUSBMiners(ref int apiPort, string coinSymbol)
        {
            LaunchMiners(ref apiPort, coinSymbol, DeviceKind.USB, false);
        }

        private void LaunchMiners(ref int apiPort, string coinSymbol, DeviceKind deviceKind, bool terminateProcess)
        {
            Xgminer.Data.Configuration.Miner minerConfiguration = CreateMinerConfiguration(apiPort, coinSymbol, deviceKind);

            
            //null if no pools configured
            if (minerConfiguration != null)
            {
                Process process = LaunchMinerProcess(minerConfiguration, "Starting mining");
                if (!process.HasExited)
                {
                    MinerDescriptor miner = MinerFactory.Instance.GetMiner(deviceKind, minerConfiguration.Algorithm, engineConfiguration.XgminerConfiguration.AlgorithmMiners);
                    MinerProcess minerProcess = StoreMinerProcess(process, miner, coinSymbol, minerConfiguration, apiPort);
                    minerProcess.TerminateProcess = terminateProcess;
                }

                apiPort++;
            }
        }

        private MinerProcess StoreMinerProcess(Process process, MinerDescriptor miner, string coinSymbol, Xgminer.Data.Configuration.Miner minerConfiguration, int apiPort)
        {
            MinerProcess minerProcess = new MinerProcess() 
            { 
                Process = process,
                Miner = miner,
                ApiPort = apiPort, 
                MinerConfiguration = minerConfiguration, 
                CoinSymbol = coinSymbol
            };

            SetupProcessStartInfo(minerProcess);

            minerProcesses.Add(minerProcess);

            return minerProcess;
        }

        private Process LaunchMinerProcess(Xgminer.Data.Configuration.Miner minerConfiguration, string reason)
        {
            minerConfiguration.Priority = engineConfiguration.XgminerConfiguration.Priority;

            //we launch 1 process per device kind now
            DeviceKind deviceKind = minerConfiguration.DeviceDescriptors.First().Kind;

            MinerDescriptor descriptor = MinerFactory.Instance.GetMiner(deviceKind, minerConfiguration.Algorithm, engineConfiguration.XgminerConfiguration.AlgorithmMiners);

            Xgminer.Miner miner = new Xgminer.Miner(minerConfiguration, descriptor.LegacyApi);
            miner.LogLaunch += LogProcessLaunch;
            miner.LaunchFailed += ProcessLaunchFailed;
            miner.AuthenticationFailed += ProcessAuthenticationFailed;
            Process process = miner.Launch(reason);
            return process;
        }

        private Xgminer.Data.Configuration.Miner CreateMinerConfiguration(int apiPort, string coinSymbol, DeviceKind deviceKind)
        {
            Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.PoolGroup.Id.Equals(coinSymbol));
            if (coinConfiguration.Pools.Count == 0)
                // no pools configured
                return null;
            
            MinerDescriptor miner = MinerFactory.Instance.GetMiner(deviceKind, coinConfiguration.PoolGroup.Algorithm, engineConfiguration.XgminerConfiguration.AlgorithmMiners);

            if (miner == null)
            {
                //no miner configured for the algorithm / custom algorithm
                throw new Xgminer.MinerLaunchException(String.Format("No miner defined for algorithm {0}.", coinConfiguration.PoolGroup.Algorithm));
            }

            Xgminer.Data.Configuration.Miner minerConfiguration = CreateBasicConfiguration(miner, coinConfiguration, apiPort);

            IList<Engine.Data.Configuration.Device> enabledConfigurations = 
                engineConfiguration.DeviceConfigurations
                .Where(c => c.Enabled && c.CoinSymbol.Equals(coinSymbol)).ToList();

            int deviceCount = SetupConfigurationDevices(minerConfiguration, deviceKind, enabledConfigurations);
            if (deviceCount == 0)
                return null;

            return minerConfiguration;
        }

        private Xgminer.Data.Configuration.Miner CreateProxyConfiguration(int apiPort, string coinSymbol)
        {
            Data.Configuration.Coin coinConfiguration = engineConfiguration.CoinConfigurations.Single(c => c.PoolGroup.Id.Equals(coinSymbol));
            if (coinConfiguration.Pools.Count == 0)
                // no pools configured
                return null;

            //BFGMiner for proxying
            MinerDescriptor miner = MinerFactory.Instance.GetDefaultMiner(); 

            return CreateBasicConfiguration(miner, coinConfiguration, apiPort);
        }

        private Xgminer.Data.Configuration.Miner CreateBasicConfiguration(
            MinerDescriptor miner, 
            Data.Configuration.Coin coinConfiguration, 
            int apiPort)
        {            
            Xgminer.Data.Configuration.Miner minerConfiguration = new Xgminer.Data.Configuration.Miner()
            {
                ExecutablePath = MinerPath.GetPathToInstalledMiner(miner),
                Algorithm = MinerFactory.Instance.GetAlgorithm(coinConfiguration.PoolGroup.Algorithm),
                ApiPort = apiPort,
                ApiListen = true,
                AllowedApiIps = engineConfiguration.XgminerConfiguration.AllowedApiIps,
                CoinName = coinConfiguration.PoolGroup.Name,
                DisableGpu = engineConfiguration.XgminerConfiguration.DisableGpu
            };

            SetupConfigurationPools(minerConfiguration, coinConfiguration);

            SetupConfigurationArguments(minerConfiguration, coinConfiguration, miner);

            return minerConfiguration;
        }

        private void SetupConfigurationArguments(Xgminer.Data.Configuration.Miner minerConfiguration, Data.Configuration.Coin coinConfiguration,
            MinerDescriptor miner)
        {
            string arguments = string.Empty;

            //apply algorithm kernel arguments
            if (minerConfiguration.Algorithm.MinerArguments.ContainsKey(miner.Name))
            {
                arguments = String.Format("{0} {0} {1}", arguments, minerConfiguration.Algorithm.MinerArguments[miner.Name], arguments.TrimStart());
            }
            
            //apply algorithm-specific parameters
            if (engineConfiguration.XgminerConfiguration.AlgorithmFlags.ContainsKey(coinConfiguration.PoolGroup.Algorithm))
                arguments = String.Format("{0} {1}", arguments,
                    engineConfiguration.XgminerConfiguration.AlgorithmFlags[coinConfiguration.PoolGroup.Algorithm]);

            //apply coin-specific parameters
            if (!string.IsNullOrEmpty(coinConfiguration.MinerFlags))
                arguments = string.Format("{0} {1}", arguments, coinConfiguration.MinerFlags);

            if (engineConfiguration.XgminerConfiguration.DesktopMode)
                arguments = arguments + " -I D";

            if (donationPercent > 0)
                arguments = arguments + " --load-balance";

            minerConfiguration.LaunchArguments = arguments;
        }

        private int SetupConfigurationDevices(Xgminer.Data.Configuration.Miner minerConfiguration, DeviceKind deviceKind, IList<Engine.Data.Configuration.Device> deviceConfigurations)
        {
            int deviceCount = 0;
            for (int i = 0; i < deviceConfigurations.Count; i++)
            {
                Engine.Data.Configuration.Device enabledConfiguration = deviceConfigurations[i];

                Xgminer.Data.Device device = devices.SingleOrDefault(d => d.Equals(enabledConfiguration));

                if (deviceKind != device.Kind)
                    continue;

                deviceCount++;

                minerConfiguration.DeviceDescriptors.Add(device);
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
                AddDonationPool(coinConfiguration.PoolGroup.Id, minerConfiguration);

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

            Data.Configuration.Coin donationConfiguration = donationConfigurations.SingleOrDefault(dc => dc.PoolGroup.Id.Equals(coinSymbol, StringComparison.OrdinalIgnoreCase));
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
