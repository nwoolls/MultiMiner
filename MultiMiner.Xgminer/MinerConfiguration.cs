using System.Collections.Generic;
using System.Diagnostics;

namespace MultiMiner.Xgminer
{
    public class MinerConfiguration
    {
        public MinerConfiguration()
        {
            this.DeviceIndexes = new List<int>();
            this.Priority = ProcessPriorityClass.Normal;
            this.Pools = new List<MiningPool>();
        }

        public MinerBackend MinerBackend { get; set; }
        public string ExecutablePath { get; set; }
        public List<MiningPool> Pools { get; set; }
        public CoinAlgorithm Algorithm { get; set; }
        public int ApiPort { get; set; }
        public bool ApiListen { get; set; }
        public List<int> DeviceIndexes { get; set; }
        public string Arguments { get; set; }
        public string CoinName { get; set; }
        public bool DisableGpu { get; set; }
        public ProcessPriorityClass Priority { get; set; }
        public bool ErupterDriver { get; set; }
    }
}
