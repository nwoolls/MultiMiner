using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MultiMiner.Engine.Configuration
{
    public class EngineConfiguration
    {
        public List<DeviceConfiguration> DeviceConfigurations { get; set; }
        public List<CoinConfiguration> CoinConfigurations { get; set; }
        
        public void SaveDeviceConfigurations()
        {
            string fileName = DeviceConfigurationsFileName();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            XmlSerializer serializer = new XmlSerializer(typeof(List<DeviceConfiguration>));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, DeviceConfigurations);
            }
        }

        private string AppDataPath()
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }

        private string DeviceConfigurationsFileName()
        {
            return Path.Combine(AppDataPath(), "DeviceConfigurations.xml");
        }

        public void LoadDeviceConfigurations()
        {
            string fileName = DeviceConfigurationsFileName();
            if (File.Exists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<CoinConfiguration>));
                using (TextReader reader = new StreamReader(fileName))
                {
                    DeviceConfigurations = (List<DeviceConfiguration>)serializer.Deserialize(reader);
                }
            }
        }

        public void LoadCoinConfigurations()
        {
            string fileName = CoinConfigurationsFileName();
            if (File.Exists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<CoinConfiguration>));
                using (TextReader reader = new StreamReader(fileName))
                {
                    CoinConfigurations = (List<CoinConfiguration>)serializer.Deserialize(reader);
                }
            }
        }

        public void SaveCoinConfigurations()
        {
            string fileName = CoinConfigurationsFileName();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            XmlSerializer serializer = new XmlSerializer(typeof(List<CoinConfiguration>));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, CoinConfigurations);
            }
        }

        private string CoinConfigurationsFileName()
        {
            return Path.Combine(AppDataPath(), "CoinConfigurations.xml");
        }
    }
}
