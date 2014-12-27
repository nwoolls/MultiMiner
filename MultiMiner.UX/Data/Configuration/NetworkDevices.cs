using MultiMiner.Engine;
using MultiMiner.Utility.Serialization;
using MultiMiner.UX.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace MultiMiner.UX.Data.Configuration
{
    public class NetworkDevices
    {
        public class NetworkDevice
        {
            public NetworkDevice()
            {
                //sticky by default - require user intervention to remove devices
                Sticky = true;
                RecentCommands = new List<string>();
            }

            public string IPAddress { get; set; }
            public int Port { get; set; }
            public bool Sticky { get; set; }
            public bool Hidden { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public List<string> RecentCommands { get; set; }
        }

        public List<NetworkDevice> Devices { get; set; }

        public NetworkDevices()
        {
            //set a default - null ref errors if submitting MobileMiner stats before scan is completed
            Devices = new List<NetworkDevice>();
        }

        private static string NetworkDevicesConfigurationFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "NetworkDevicesConfiguration.xml");
        }

        public void SaveNetworkDevicesConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(Devices, NetworkDevicesConfigurationFileName());
        }

        public void LoadNetworkDevicesConfiguration()
        {
            Devices = ConfigurationReaderWriter.ReadConfiguration<List<NetworkDevice>>(NetworkDevicesConfigurationFileName());
        }
    }
}
