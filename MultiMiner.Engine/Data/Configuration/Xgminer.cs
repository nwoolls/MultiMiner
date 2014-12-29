using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using MultiMiner.Utility.Serialization;

namespace MultiMiner.Engine.Data.Configuration
{
    [XmlType(TypeName = "XgminerConfiguration")]
    public class Xgminer
    {
        public class ProxyDescriptor
        {
            public int GetworkPort { get; set; }
            public int StratumPort { get; set; }
        }

        public Xgminer()
        {
            AlgorithmFlags = new SerializableDictionary<string, string>();
            AlgorithmMiners = new SerializableDictionary<string, string>();
            Priority = ProcessPriorityClass.Normal;
            StartingApiPort = 4028;

            StratumProxies = new List<ProxyDescriptor>();
        }

        public SerializableDictionary<string, string> AlgorithmFlags { get; set; }
        public SerializableDictionary<string, string> AlgorithmMiners { get; set; }
        public string ScanArguments { get; set; }
        public bool DesktopMode { get; set; }
        public bool DisableGpu { get; set; }
        public bool DisableUsbProbe { get; set; }
        public ProcessPriorityClass Priority { get; set; }
        public int StartingApiPort { get; set; }
        public string AllowedApiIps { get; set; }
        public bool TerminateGpuMiners { get; set; }

        //bfgminer-specific
        public bool StratumProxy { get; set; }
        public int StratumProxyPort { get; set; } //deprecated
        public int StratumProxyStratumPort { get; set; } //deprecated
        public List<ProxyDescriptor> StratumProxies { get; set; }

        private static string XgminerConfigurationFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "XgminerConfiguration.xml");
        }

        public void LoadMinerConfiguration()
        {
            string fileName = XgminerConfigurationFileName();
            UpgradeMinerConfigurationFile(fileName);
            Xgminer minerConfiguration = ConfigurationReaderWriter.ReadConfiguration<Xgminer>(fileName);
            ObjectCopier.CopyObject(minerConfiguration, this);

            UpgradeConfiguration();

            if (StratumProxies.Count == 0)
                AddDefaultProxy();
        }

        private static void UpgradeMinerConfigurationFile(string fileName)
        {
            //we no longer serialize CoinAlgorithm - it is now a class, not an enum
            if (File.Exists(fileName))
            {
                string configurationText = File.ReadAllText(fileName);
                configurationText = configurationText.Replace("CoinAlgorithm>", "string>");
                File.WriteAllText(fileName, configurationText);
            }
        }

        private void AddDefaultProxy()
        {
            ProxyDescriptor proxy = new ProxyDescriptor()
            {
                GetworkPort = 8332,
                StratumPort = 3333
            };
            StratumProxies.Add(proxy);
        }

        private void UpgradeConfiguration()
        {
            UpgradeProxyConfiguration();
        }

        private void UpgradeProxyConfiguration()
        {
            if ((StratumProxyPort > 0) || (StratumProxyStratumPort > 0))
            {
                ProxyDescriptor proxy = new ProxyDescriptor()
                {
                    GetworkPort = StratumProxyPort,
                    StratumPort = StratumProxyStratumPort
                };
                StratumProxies.Add(proxy);

                StratumProxyPort = -1;
                StratumProxyStratumPort = -1;

                SaveMinerConfiguration();
            }
        }

        public void SaveMinerConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(this, XgminerConfigurationFileName());
        }
    }
}
