using MultiMiner.Xgminer;

namespace MultiMiner.Engine.Configuration
{
    public class DeviceConfiguration
    {
        public DeviceKind DeviceKind { get; set; }
        public int DeviceIndex { get; set; }
        public string CoinSymbol { get; set; }
    }
}
