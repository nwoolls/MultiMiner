using MultiMiner.Utility;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiMiner.Engine.Configuration
{
    public class EngineConfiguration
    {
        public EngineConfiguration()
        {
            DeviceConfigurations = new List<DeviceConfiguration>();
            CoinConfigurations = new List<CoinConfiguration>();
            XgminerConfiguration = new XgminerConfiguration();
            StrategyConfiguration = new StrategyConfiguration();
        }

        public List<DeviceConfiguration> DeviceConfigurations { get; set; }
        public List<CoinConfiguration> CoinConfigurations { get; set; }
        public XgminerConfiguration XgminerConfiguration { get; set; }
        public StrategyConfiguration StrategyConfiguration { get; set; }

        public void RemoveBlankPoolConfigurations()
        {
            foreach (CoinConfiguration coinConfiguration in CoinConfigurations)
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

        private static string AppDataPath()
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }

        private static string StrategyConfigurationsFileName()
        {
            return Path.Combine(AppDataPath(), "StrategyConfiguration.xml");
        }

        public void SaveStrategyConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(StrategyConfiguration, StrategyConfigurationsFileName());
        }

        public void LoadStrategyConfiguration()
        {
            try
            {
                StrategyConfiguration = ConfigurationReaderWriter.ReadConfiguration<StrategyConfiguration>(StrategyConfigurationsFileName());
            }
            catch (InvalidOperationException ex)
            {
                //legacy settings
                Obsolete.StrategyConfiguration obsoleteSettings = ConfigurationReaderWriter.ReadConfiguration<Obsolete.StrategyConfiguration>(StrategyConfigurationsFileName());
                StrategyConfiguration = new StrategyConfiguration();
                obsoleteSettings.StoreTo(StrategyConfiguration);
                SaveStrategyConfiguration();
            }
        }

        private static string DeviceConfigurationsFileName()
        {
            return Path.Combine(AppDataPath(), "DeviceConfigurations.xml");
        }
        
        public void SaveDeviceConfigurations()
        {
            ConfigurationReaderWriter.WriteConfiguration(DeviceConfigurations, DeviceConfigurationsFileName());
        }

        public void LoadDeviceConfigurations()
        {
            DeviceConfigurations = ConfigurationReaderWriter.ReadConfiguration<List<DeviceConfiguration>>(DeviceConfigurationsFileName());
            RemoveIvalidCoinsFromDeviceConfigurations();
            RemoveDuplicateDeviceConfigurations();
        }

        //this is necessary due to large changes to the class definition and streaming in / deserializing
        //older legacy XML
        private void RemoveDuplicateDeviceConfigurations()
        {
            DeviceConfigurations = DeviceConfigurations
                .GroupBy(c => new { c.Kind, c.RelativeIndex, c.Driver, c.Path, c.Serial })
                .Select(c => c.First())
                .ToList();
        }

        public static string CoinConfigurationsFileName()
        {
            return Path.Combine(AppDataPath(), "CoinConfigurations.xml");
        }

        public void LoadCoinConfigurations()
        {
            CoinConfigurations = ConfigurationReaderWriter.ReadConfiguration<List<CoinConfiguration>>(CoinConfigurationsFileName());
            RemoveIvalidCoinsFromDeviceConfigurations();
            RemoveBlankPoolConfigurations();
        }

        private void RemoveDisabledCoinsFromDeviceConfigurations()
        {
            foreach (CoinConfiguration coinConfiguration in CoinConfigurations.Where(c => !c.Enabled))
            {
                IEnumerable<DeviceConfiguration> coinDeviceConfigurations = DeviceConfigurations.Where(c => !String.IsNullOrEmpty(c.CoinSymbol) && c.CoinSymbol.Equals(coinConfiguration.Coin.Symbol));
                foreach (DeviceConfiguration coinDeviceConfiguration in coinDeviceConfigurations)
                    coinDeviceConfiguration.CoinSymbol = string.Empty;
            }
        }

        private void RemoveDeletedCoinsFromDeviceConfigurations()
        {
            foreach (DeviceConfiguration deviceConfiguration in DeviceConfigurations)
                if (CoinConfigurations.Count(c => c.Coin.Symbol.Equals(deviceConfiguration.CoinSymbol)) == 0)
                    deviceConfiguration.CoinSymbol = string.Empty;
        }

        private void RemoveIvalidCoinsFromDeviceConfigurations()
        {
            RemoveDisabledCoinsFromDeviceConfigurations();
            RemoveDeletedCoinsFromDeviceConfigurations();
        }

        public void SaveCoinConfigurations()
        {
            RemoveBlankPoolConfigurations();
            ConfigurationReaderWriter.WriteConfiguration(CoinConfigurations, CoinConfigurationsFileName());
            RemoveIvalidCoinsFromDeviceConfigurations();
        }

        private static string XgminerConfigurationFileName()
        {
            return Path.Combine(AppDataPath(), "XgminerConfiguration.xml");
        }

        public void LoadMinerConfiguration()
        {
            XgminerConfiguration = ConfigurationReaderWriter.ReadConfiguration<XgminerConfiguration>(XgminerConfigurationFileName());
        }

        public void SaveMinerConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(XgminerConfiguration, XgminerConfigurationFileName());
        }
    }
}
