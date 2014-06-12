using MultiMiner.Engine;
using MultiMiner.Utility.Serialization;
using System.Collections.Generic;
using System.IO;

namespace MultiMiner.UX.Data.Configuration
{
    public class Metadata
    {
        public class DeviceMetadata : Xgminer.Data.DeviceDescriptor
        {
            public string FriendlyName { get; set; }
        }

        public List<DeviceMetadata> Devices { get; set; }

        public Metadata()
        {
            Devices = new List<DeviceMetadata>();
        }

        private static string DeviceMetadataConfigurationFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "DeviceMetadata.xml");
        }

        public void SaveDeviceMetadataConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(Devices, DeviceMetadataConfigurationFileName());
        }

        public void LoadDeviceMetadataConfiguration()
        {
            Devices = ConfigurationReaderWriter.ReadConfiguration<List<DeviceMetadata>>(DeviceMetadataConfigurationFileName());
        }
    }
}
