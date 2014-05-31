using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Data.Transfer.Configuration
{
    [DataContract]
    public class Xgminer
    {
        public Xgminer()
        {
            AlgorithmFlags = new Hashtable();
            AlgorithmMiners = new Hashtable();
            Priority = ProcessPriorityClass.Normal;
        }

        [DataMember]
        public Hashtable AlgorithmFlags { get; set; }

        [DataMember]
        public string ScanArguments { get; set; }

        [DataMember]
        public bool DesktopMode { get; set; }

        [DataMember]
        public bool DisableGpu { get; set; }

        [DataMember]
        public bool DisableUsbProbe { get; set; }

        [DataMember]
        public ProcessPriorityClass Priority { get; set; }

        [DataMember]
        public int StartingApiPort { get; set; }

        [DataMember]
        public string AllowedApiIps { get; set; }

        [DataMember]
        public bool TerminateGpuMiners { get; set; }

        [DataMember]
        public bool StratumProxy { get; set; }

        [DataMember]
        public int StratumProxyPort { get; set; }

        [DataMember]
        public int StratumProxyStratumPort { get; set; }

        [DataMember]
        public MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor[] StratumProxies { get; set; }

        [DataMember]
        public Hashtable AlgorithmMiners { get; set; }
    }
}
