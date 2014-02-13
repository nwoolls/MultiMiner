using MultiMiner.Utility.Serialization;
using MultiMiner.Xgminer.Data;
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

        public void RemoveBlankPoolConfigurations()
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
            RemoveIvalidCoinsFromDeviceConfigurations();
            RemoveBlankPoolConfigurations();
        }

        private void RemoveDisabledCoinsFromDeviceConfigurations()
        {
            foreach (Coin coinConfiguration in CoinConfigurations.Where(c => !c.Enabled))
            {
                IEnumerable<Device> coinDeviceConfigurations = DeviceConfigurations.Where(c => !String.IsNullOrEmpty(c.CoinSymbol) && c.CoinSymbol.Equals(coinConfiguration.CryptoCoin.Symbol));
                foreach (Device coinDeviceConfiguration in coinDeviceConfigurations)
                    coinDeviceConfiguration.CoinSymbol = string.Empty;
            }
        }

        private void RemoveDeletedCoinsFromDeviceConfigurations()
        {
            foreach (Device deviceConfiguration in DeviceConfigurations)
                if (CoinConfigurations.Count(c => c.CryptoCoin.Symbol.Equals(deviceConfiguration.CoinSymbol)) == 0)
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

        public void LoadMinerConfiguration()
        {
            XgminerConfiguration.LoadMinerConfiguration();
        }

        public void SaveMinerConfiguration()
        {
            XgminerConfiguration.SaveMinerConfiguration();
        }
    }
}
