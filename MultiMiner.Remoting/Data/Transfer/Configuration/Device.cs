using MultiMiner.Xgminer.Data;
using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Data.Transfer.Configuration
{
    [DataContract]
    public class Device
    {
        [DataMember]
        public DeviceKind Kind { get; set; }

        [DataMember]
        public int RelativeIndex { get; set; }

        [DataMember]
        public string Driver { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Serial { get; set; }

        [DataMember]
        public string CoinSymbol { get; set; }

        [DataMember]
        public bool Enabled { get; set; }
    }
}
