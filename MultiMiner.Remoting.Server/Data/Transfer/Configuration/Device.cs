using MultiMiner.Xgminer;

namespace MultiMiner.Remoting.Server.Data.Transfer.Configuration
{
    public class Device
    {
        public DeviceKind Kind { get; set; }
        public int RelativeIndex { get; set; }
        public string Driver { get; set; }
        public string Path { get; set; }
        public string Serial { get; set; }
        public string CoinSymbol { get; set; }
        public bool Enabled { get; set; }
    }
}
