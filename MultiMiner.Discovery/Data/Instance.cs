using System;

namespace MultiMiner.Discovery.Data
{
    [Serializable]
    public class Instance
    {
        public string IpAddress { get; set; }
        public string MachineName { get; set; }
        public int Fingerprint { get; set; }
    }
}
