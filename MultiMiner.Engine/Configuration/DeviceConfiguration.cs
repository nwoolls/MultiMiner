using System;

namespace MultiMiner.Engine.Configuration
{
    public class DeviceConfiguration
    {
        public DeviceConfiguration()
        {
            this.Enabled = true;
            this.CoinSymbol = String.Empty;
        }

        public int DeviceIndex { get; set; }
        public string CoinSymbol { get; set; }
        public bool Enabled { get; set; }
    }
}
