using MultiMiner.Xgminer;

namespace MultiMiner.Engine.Configuration
{
    public class XgminerConfiguration
    {
        public XgminerConfiguration()
        {
            AlgorithmFlags = new SerializableDictionary<CoinAlgorithm, string>();
        }

        public SerializableDictionary<CoinAlgorithm, string> AlgorithmFlags { get; set; }
        public MinerBackend MinerBackend { get; set; }
        public bool DesktopMode { get; set; }
        public bool DisableGpu { get; set; }
    }
}
