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
            StrategyConfiguration = ConfigurationReaderWriter.ReadConfiguration<StrategyConfiguration>(StrategyConfigurationsFileName());
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
        }

        private static string CoinConfigurationsFileName()
        {
            return Path.Combine(AppDataPath(), "CoinConfigurations.xml");
        }

        public void LoadCoinConfigurations()
        {
            CoinConfigurations = ConfigurationReaderWriter.ReadConfiguration<List<CoinConfiguration>>(CoinConfigurationsFileName());
            RemoveIvalidCoinsFromDeviceConfigurations();
        }

        private void RemoveDisabledCoinsFromDeviceConfigurations()
        {
            foreach (CoinConfiguration coinConfiguration in CoinConfigurations.Where(c => !c.Enabled))
            {
                IEnumerable<DeviceConfiguration> coinDeviceConfigurations = DeviceConfigurations.Where(c => c.CoinSymbol.Equals(coinConfiguration.Coin.Symbol));
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
