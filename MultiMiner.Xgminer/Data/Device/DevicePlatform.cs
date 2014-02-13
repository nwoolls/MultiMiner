using System;

namespace MultiMiner.Xgminer.Data
{
    [Serializable]
    public class DevicePlatform
    {
        public string Vendor { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
