using MultiMiner.Xgminer.Data;
using System;
using System.Xml.Serialization;

namespace MultiMiner.Engine.Data.Configuration
{
    [Serializable]
    [XmlType(TypeName = "DeviceConfiguration")]
    public class Device : DeviceDescriptor
    {
        public Device()
        {
            Enabled = true;
            CoinSymbol = String.Empty;
        }

        public string CoinSymbol { get; set; }
        public bool Enabled { get; set; }
    }
}
