using MultiMiner.Utility;
using MultiMiner.Xgminer;
using System.Diagnostics;

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
        }

        public SerializableDictionary<CoinAlgorithm, string> AlgorithmFlags { get; set; }
        public MinerBackend MinerBackend { get; set; }
        public bool DesktopMode { get; set; }
        public bool DisableGpu { get; set; }
        public ProcessPriorityClass Priority { get; set; }
        public bool ErupterDriver { get; set; }
        public int StartingApiPort { get; set; }
        public string AllowedApiIps { get; set; }
        public bool StratumProxy { get; set; }
        public int StratumProxyPort { get; set; }
    }
}
