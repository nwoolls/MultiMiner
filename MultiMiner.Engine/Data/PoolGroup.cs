using System;
using System.Xml.Serialization;

namespace MultiMiner.Engine.Data
{
    //marked Serializable to allow deep cloning of CoinConfiguration
    [Serializable]
    [XmlType("CryptoCoin")]
    public class PoolGroup
    {
        [XmlElement(ElementName = "Symbol")]
        //for Crypto Coins, Id is the exchange symbol
        //for Multi Pools, Id is the algorithm + optional multipool API
        public string Id { get; set; }
        public string Name { get; set; }
        public string Algorithm { get; set; }
    }
}
