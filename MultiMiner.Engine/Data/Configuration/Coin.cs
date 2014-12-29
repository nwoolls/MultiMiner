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
            Pools = new List<MiningPool>();
            Enabled = true;
            PoolGroup = new PoolGroup();
        }

        [XmlElement(ElementName = "Coin")]
        public PoolGroup PoolGroup { get; set; }
        public List<MiningPool> Pools { get; set; }
        public string MinerFlags { get; set; }
        public double ProfitabilityAdjustment { get; set; }
        public AdjustmentType ProfitabilityAdjustmentType { get; set; }
        public bool Enabled { get; set; }
        public bool PoolsDown { get; set; }
        
        //rolled over so we can do a 1-time disable of these
        public bool NotifyOnBlockFound2 { get; set; }
        public bool NotifyOnShareAccepted2 { get; set; }
    }
}
