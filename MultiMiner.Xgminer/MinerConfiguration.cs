using System.Collections.Generic;

namespace MultiMiner.Xgminer
{
    public class MinerConfiguration
    {
        public MinerConfiguration()
        {
            this.DeviceIndexes = new List<int>();
        }

        public string ExecutablePath { get; set; }
        public List<MiningPool> Pools { get; set; }
        public CoinAlgorithm Algorithm { get; set; }
        public int ApiPort { get; set; }
        public bool ApiListen { get; set; }
        public List<int> DeviceIndexes { get; set; }
    }
}
