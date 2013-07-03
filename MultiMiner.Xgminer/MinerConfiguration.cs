using System.Collections.Generic;

namespace MultiMiner.Xgminer
{
    public class MinerConfiguration
    {
        public MinerConfiguration()
        {
            this.DeviceIndexes = new List<int>();
        }

        public MinerBackend MinerBackend { get; set; }
        public string ExecutablePath { get; set; }
        public List<MiningPool> Pools { get; set; }
        public CoinAlgorithm Algorithm { get; set; }
        public int ApiPort { get; set; }
        public bool ApiListen { get; set; }
        public List<int> DeviceIndexes { get; set; }
        public string Arguments { get; set; }

        public MinerBackend ActualMinerBackend
        {
            get
            {
                MinerBackend result = this.MinerBackend;
                if (result == Xgminer.MinerBackend.Automatic)
                {
                    result = Xgminer.MinerBackend.Cgminer;
                    if (ExecutablePath.ToLower().Contains("bfgminer"))
                        result = Xgminer.MinerBackend.Bfgminer;
                }
                return result;
            }
        }

    }
}
