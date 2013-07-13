using MultiMiner.Xgminer;

namespace MultiMiner.Engine.Configuration
{
    public class XgminerConfiguration
    {
        public const string CgminerName = "cgminer";
        public const string BfgminerName = "bfgminer";

        public XgminerConfiguration()
        {
            AlgorithmFlags = new SerializableDictionary<CoinAlgorithm, string>();
        }

        public SerializableDictionary<CoinAlgorithm, string> AlgorithmFlags { get; set; }
        public MinerBackend MinerBackend { get; set; }
        public bool DesktopMode { get; set; }
        public bool DisableGpu { get; set; }

        public string MinerName
        {
            get
            {
                string minerName = CgminerName;
                if (MinerBackend == MinerBackend.Bfgminer)
                    minerName = BfgminerName;
                return minerName;
            }
        }
    }
}
