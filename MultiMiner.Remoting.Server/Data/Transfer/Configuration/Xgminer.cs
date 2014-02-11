using System.Collections;
using System.Diagnostics;

namespace MultiMiner.Remoting.Server.Data.Transfer.Configuration
{
    public class Xgminer
    {
        public Xgminer()
        {
            AlgorithmFlags = new Hashtable();
            Priority = ProcessPriorityClass.Normal;
        }

        public Hashtable AlgorithmFlags { get; set; }
        public string ScanArguments { get; set; }
        public bool DesktopMode { get; set; }
        public bool DisableGpu { get; set; }
        public bool DisableUsbProbe { get; set; }
        public ProcessPriorityClass Priority { get; set; }
        public int StartingApiPort { get; set; }
        public string AllowedApiIps { get; set; }
        public bool TerminateGpuMiners { get; set; }
        public bool StratumProxy { get; set; }
        public int StratumProxyPort { get; set; }
        public int StratumProxyStratumPort { get; set; }
    }
}
