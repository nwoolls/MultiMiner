using MultiMiner.Xgminer.Data;
using MultiMiner.Xgminer.Installer;

namespace MultiMiner.Engine
{
    public class MinerDescriptor
    {
        public CoinAlgorithm Algorithm { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public IMinerInstaller Installer { get; set; }
    }
}
