using MultiMiner.Xgminer;
using System;

namespace MultiMiner.Engine.Configuration
{
    [Serializable]
    public class DeviceConfiguration : DeviceDescriptor
    {
        public DeviceConfiguration()
        {
            this.Enabled = true;
            this.CoinSymbol = String.Empty;
        }

        public string CoinSymbol { get; set; }
        public bool Enabled { get; set; }
    }
}
