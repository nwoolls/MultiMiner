using MultiMiner.Utility;
using System.Collections.Generic;
using System.IO;

namespace MultiMiner.Win.Configuration
{
    public class NetworkDevicesConfiguration
    {
        public class NetworkDevice
        {
            public string IPAddress { get; set; }
            public int Port { get; set; }
        }

        public List<NetworkDevice> NetworkDevices { get; set; }

        private static string NetworkDevicesConfigurationFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "NetworkDevicesConfiguration.xml");
        }

        public void SaveNetworkDevicesConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(NetworkDevices, NetworkDevicesConfigurationFileName());
        }

        public void LoadNetworkDevicesConfiguration()
        {
            NetworkDevices = ConfigurationReaderWriter.ReadConfiguration<List<NetworkDevice>>(NetworkDevicesConfigurationFileName());
        }
    }
}
