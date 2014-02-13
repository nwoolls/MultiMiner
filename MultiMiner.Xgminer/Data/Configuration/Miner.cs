using System.Collections.Generic;
using System.Diagnostics;

namespace MultiMiner.Xgminer.Data.Configuration
{
    public class MinerConfiguration
    {
        public MinerConfiguration()
        {
            this.DeviceDescriptors = new List<DeviceDescriptor>();
            this.Priority = ProcessPriorityClass.Normal;
            this.Pools = new List<MiningPool>();
            this.StratumProxyPort = 8332;
            this.StratumProxyStratumPort = 3333;
            this.LogInterval = 5;
        }

        public string ExecutablePath { get; set; }
        public List<MiningPool> Pools { get; set; }
        public CoinAlgorithm Algorithm { get; set; }
        public int ApiPort { get; set; }
        public bool ApiListen { get; set; }
        public string AllowedApiIps { get; set; }
        public List<DeviceDescriptor> DeviceDescriptors { get; set; }
        public string LaunchArguments { get; set; }
        public string ScanArguments { get; set; }
        public string CoinName { get; set; }
        public bool DisableGpu { get; set; }
        public bool DisableUsbProbe { get; set; }
        public ProcessPriorityClass Priority { get; set; }
        public int LogInterval { get; set; }

        //bfgminer-specific
        public bool StratumProxy { get; set; }
        public int StratumProxyPort { get; set; }
        public int StratumProxyStratumPort { get; set; }
    }
}
