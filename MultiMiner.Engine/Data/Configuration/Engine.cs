using MultiMiner.Utility.Serialization;
using MultiMiner.Xgminer.Data;
using MultiMiner.Engine.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiMiner.Engine.Data.Configuration
{
    public class Engine
    {
        public Engine()
        {
            DeviceConfigurations = new List<Device>();
            CoinConfigurations = new List<Coin>();
            XgminerConfiguration = new Xgminer();
            StrategyConfiguration = new Strategy();
        }

        public List<Device> DeviceConfigurations { get; set; }
        public List<Coin> CoinConfigurations { get; set; }
        public Xgminer XgminerConfiguration { get; set; }
        public Strategy StrategyConfiguration { get; set; }

        private void RemoveBlankPoolConfigurations()
        {
            foreach (Coin coinConfiguration in CoinConfigurations)
            {
                for (int i = coinConfiguration.Pools.Count - 1; i >= 0; i--)
                {
                    MiningPool pool = coinConfiguration.Pools[i];
                    if (String.IsNullOrEmpty(pool.Host) &&
                        String.IsNullOrEmpty(pool.Username))
                        coinConfiguration.Pools.Remove(pool);
                }
            }
        }

        private string configDirectory;
        public void LoadAllConfigurations(string configDirectory)
        {
            InitializeConfigDirectory(configDirectory);

            LoadCoinConfigurations(configDirectory);
            LoadDeviceConfigurations();
            LoadMinerConfiguration();
            LoadStrategyConfiguration(configDirectory);
        }

        public string StrategyConfigurationsFileName()
        {
            return Path.Combine(configDirectory, "StrategyConfiguration.xml");
        }

        public void SaveStrategyConfiguration(string configDirectory = null)
        {
            InitializeConfigDirectory(configDirectory);

            ConfigurationReaderWriter.WriteConfiguration(StrategyConfiguration, StrategyConfigurationsFileName());
        }

        private void InitializeConfigDirectory(string configDirectory)
        {
            if (!String.IsNullOrEmpty(configDirectory))
                this.configDirectory = configDirectory;
            else if (String.IsNullOrEmpty(this.configDirectory))
                this.configDirectory = ApplicationPaths.AppDataPath();
        }

        public void LoadStrategyConfiguration(string configDirectory)
        {
            InitializeConfigDirectory(configDirectory);
            StrategyConfiguration = ConfigurationReaderWriter.ReadConfiguration<Strategy>(StrategyConfigurationsFileName());
            FixStrategyConfigurationSymbolDiscrepencies();
        }

        private static string DeviceConfigurationsFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "DeviceConfigurations.xml");
        }
        
        public void SaveDeviceConfigurations()
        {
            ConfigurationReaderWriter.WriteConfiguration(DeviceConfigurations,
                DeviceConfigurationsFileName(), "ArrayOfDeviceConfiguration");
        }

        public void LoadDeviceConfigurations()
        {
            DeviceConfigurations = ConfigurationReaderWriter.ReadConfiguration<List<Device>>(
                DeviceConfigurationsFileName(), "ArrayOfDeviceConfiguration");
            FixDeviceConfigurationSymbolDiscrepencies();
            RemoveIvalidCoinsFromDeviceConfigurations();
            RemoveDuplicateDeviceConfigurations();
        }

        //this is necessary due to large changes to the class definition and streaming in / deserializing
        //older legacy XML
        public void RemoveDuplicateDeviceConfigurations()
        {
            DeviceConfigurations = DeviceConfigurations
                .GroupBy(c => new { c.Kind, c.RelativeIndex, c.Driver, c.Path, c.Serial })
                .Select(c => c.First())
                .ToList();
        }

        public string CoinConfigurationsFileName()
        {
            return Path.Combine(configDirectory, "CoinConfigurations.xml");
        }

        public void LoadCoinConfigurations(string configDirectory)
        {
            InitializeConfigDirectory(configDirectory);

            CoinConfigurations = ConfigurationReaderWriter.ReadConfiguration<List<Coin>>(CoinConfigurationsFileName());
           
            FixCoinConfigurationSymbolDiscrepencies();
            FixWrongAlgorithmsFromCoinChoose();
            FixWrongAlgorithmsFromCoinWarz();
            RemoveIvalidCoinsFromDeviceConfigurations();
            RemoveBlankPoolConfigurations();
        }

        //configurations save with algorithm FullName instead of Name
        private void FixWrongAlgorithmsFromCoinWarz()
        {
            List<CoinAlgorithm> algorithms = MinerFactory.Instance.Algorithms;
            IEnumerable<Coin> issues = CoinConfigurations.Where(
                cc => algorithms.Any(a => a.FullName.Equals(cc.PoolGroup.Algorithm, StringComparison.OrdinalIgnoreCase) 
                    && !a.Name.Equals(cc.PoolGroup.Algorithm, StringComparison.OrdinalIgnoreCase)));

            foreach (Coin issue in issues)
            {
                CoinAlgorithm algorithm = algorithms.Single(a => a.FullName.Equals(issue.PoolGroup.Algorithm, StringComparison.OrdinalIgnoreCase));
                issue.PoolGroup.Algorithm = algorithm.Name;
            }
        }

        //configurations saved with algorithm Name with spaces
        private void FixWrongAlgorithmsFromCoinChoose()
        {
            IEnumerable<Coin> potentialIssues = CoinConfigurations.Where(cc => !cc.PoolGroup.Algorithm.Replace(" ", String.Empty).Equals(cc.PoolGroup.Algorithm));
            foreach (Coin potentialIssue in potentialIssues)
            {
                string algorithmName = potentialIssue.PoolGroup.Algorithm.Replace(" ", String.Empty);
                CoinAlgorithm algorithm = MinerFactory.Instance.GetAlgorithm(algorithmName);
                if (algorithm != null)
                    //only make the change if there is an algorithm found
                    //a user may add an algorithm with a space in the name - we don't want to change that
                    potentialIssue.PoolGroup.Algorithm = algorithm.Name;
            }
        }

        private void RemoveDisabledCoinsFromDeviceConfigurations()
        {
            foreach (Coin coinConfiguration in CoinConfigurations.Where(c => !c.Enabled))
            {
                IEnumerable<Device> coinDeviceConfigurations = DeviceConfigurations.Where(c => !String.IsNullOrEmpty(c.CoinSymbol) && c.CoinSymbol.Equals(coinConfiguration.PoolGroup.Id));
                foreach (Device coinDeviceConfiguration in coinDeviceConfigurations)
                    coinDeviceConfiguration.CoinSymbol = string.Empty;
            }
        }

        private void RemoveDeletedCoinsFromDeviceConfigurations()
        {
            foreach (Device deviceConfiguration in DeviceConfigurations)
                if (CoinConfigurations.Count(c => c.PoolGroup.Id.Equals(deviceConfiguration.CoinSymbol)) == 0)
                    deviceConfiguration.CoinSymbol = string.Empty;
        }

        private void RemoveIvalidCoinsFromDeviceConfigurations()
        {
            RemoveDisabledCoinsFromDeviceConfigurations();
            RemoveDeletedCoinsFromDeviceConfigurations();
        }

        public void SaveCoinConfigurations(string configDirectory = null)
        {
            InitializeConfigDirectory(configDirectory);

            RemoveBlankPoolConfigurations();
            ConfigurationReaderWriter.WriteConfiguration(CoinConfigurations, CoinConfigurationsFileName());
            RemoveIvalidCoinsFromDeviceConfigurations();
        }

        public void SaveAllConfigurations()
        {
            SaveCoinConfigurations();
            SaveMinerConfiguration();
            SaveStrategyConfiguration();
            SaveDeviceConfigurations();
        }

        public void LoadMinerConfiguration()
        {
            XgminerConfiguration.LoadMinerConfiguration();
        }

        public void SaveMinerConfiguration()
        {
            XgminerConfiguration.SaveMinerConfiguration();
        }

        private void FixDeviceConfigurationSymbolDiscrepencies()
        {
            bool save = DeviceConfigurations.Any(dc => dc.CoinSymbol.Equals(KnownCoins.BadDogecoinSymbol, StringComparison.OrdinalIgnoreCase));
            foreach (Device deviceConfiguration in DeviceConfigurations.Where(dc => dc.CoinSymbol.Equals(KnownCoins.BadDogecoinSymbol, StringComparison.OrdinalIgnoreCase)))
                deviceConfiguration.CoinSymbol = KnownCoins.DogecoinSymbol;
            if (save)
                SaveDeviceConfigurations();
        }

        private void FixStrategyConfigurationSymbolDiscrepencies()
        {
            if (StrategyConfiguration.MinimumThresholdSymbol.Equals(KnownCoins.BadDogecoinSymbol, StringComparison.OrdinalIgnoreCase))
            {
                StrategyConfiguration.MinimumThresholdSymbol = KnownCoins.DogecoinSymbol;
                SaveStrategyConfiguration();
            }
        }

        private void FixCoinConfigurationSymbolDiscrepencies()
        {
            Coin badConfiguration = CoinConfigurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(KnownCoins.BadDogecoinSymbol, StringComparison.OrdinalIgnoreCase));
            if (badConfiguration != null)
            {
                Coin goodConfiguration = CoinConfigurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(KnownCoins.DogecoinSymbol, StringComparison.OrdinalIgnoreCase));

                if (goodConfiguration == null)
                    badConfiguration.PoolGroup.Id = KnownCoins.DogecoinSymbol;
                else
                    CoinConfigurations.Remove(badConfiguration);

                SaveCoinConfigurations();
            }
        }
    }
}
