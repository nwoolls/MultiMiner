using MultiMiner.Utility;
using MultiMiner.Xgminer;
using System.Diagnostics;
using System.IO;

namespace MultiMiner.Engine.Configuration
{
    public class XgminerConfiguration
    {
        public XgminerConfiguration()
        {
            AlgorithmFlags = new SerializableDictionary<CoinAlgorithm, string>();
            Priority = ProcessPriorityClass.Normal;
            StartingApiPort = 4028;
            StratumProxyPort = 8332;
            StratumProxyStratumPort = 3333;
        }

        public SerializableDictionary<CoinAlgorithm, string> AlgorithmFlags { get; set; }
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
        public int StratumProxyPort { get; set; }
        public int StratumProxyStratumPort { get; set; }

        private static string XgminerConfigurationFileName()
        {
            return Path.Combine(ApplicationPaths.AppDataPath(), "XgminerConfiguration.xml");
        }

        public void LoadMinerConfiguration()
        {
            XgminerConfiguration minerConfiguration = ConfigurationReaderWriter.ReadConfiguration<XgminerConfiguration>(XgminerConfigurationFileName());
            ObjectCopier.CopyObject(minerConfiguration, this);
        }

        public void SaveMinerConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(this, XgminerConfigurationFileName());
        }
    }
}
