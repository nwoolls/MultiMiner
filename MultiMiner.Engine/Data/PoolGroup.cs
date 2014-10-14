using System;
using System.Xml.Serialization;

namespace MultiMiner.Engine.Data
{
    //marked Serializable to allow deep cloning of CoinConfiguration
    [Serializable]
    [XmlType("CryptoCoin")]
    public class PoolGroup
    {
        public enum PoolGroupKind
        {
            SingleCoin,
            MultiCoin
        }

        //serialized as Symbol for legacy support
        [XmlElement(ElementName = "Symbol")]
        //for SingleCoin, Id is the exchange symbol
        //for MultiCoin, Id is the algorithm + optional multipool API
        public string Id { get; set; }

        public PoolGroupKind Kind { get; set; }
        public string Name { get; set; }
        public string Algorithm { get; set; }
    }
}
