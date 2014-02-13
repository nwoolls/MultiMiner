using MultiMiner.Engine.Data;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MultiMiner.Engine.Data.Configuration
{
    //marked Serializable to allow deep cloning of CoinConfiguration
    [Serializable]
    [XmlType(TypeName = "CoinConfiguration")]
    public class Coin
    {
        public enum AdjustmentType
        {
            Addition = 0,
            Multiplication = 1
        }

        public Coin()
        {
            this.Pools = new List<MiningPool>();
            this.Enabled = true;
            this.CryptoCoin = new CryptoCoin();
        }

        [XmlElement(ElementName = "Coin")]
        public CryptoCoin CryptoCoin { get; set; }
        public List<MiningPool> Pools { get; set; }
        public string MinerFlags { get; set; }
        public double ProfitabilityAdjustment { get; set; }
        public AdjustmentType ProfitabilityAdjustmentType { get; set; }
        public bool Enabled { get; set; }
        public bool PoolsDown { get; set; }

        public bool NotifyOnBlockFound { get; set; }
        public bool NotifyOnShareAccepted { get; set; }
    }
}
