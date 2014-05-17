using MultiMiner.Xgminer.Data;

namespace MultiMiner.Engine.Data
{
    public class MinerDescriptor : AvailableMiner
    {
        public CoinAlgorithm Algorithm { get; set; }
        public string FileName { get; set; }
        public bool LegacyApi { get; set; }
    }
}
