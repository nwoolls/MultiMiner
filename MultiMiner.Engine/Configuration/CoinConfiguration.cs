using MultiMiner.Xgminer;
using System.Collections.Generic;

namespace MultiMiner.Engine.Configuration
{
    public class CoinConfiguration
    {
        public enum AdjustmentType
        {
            Addition = 0,
            Multiplication = 1
        }

        public CoinConfiguration()
        {
            this.Pools = new List<MiningPool>();
            this.Enabled = true;
        }

        public CryptoCoin Coin { get; set; }
        public List<MiningPool> Pools { get; set; }
        public string MinerFlags { get; set; }
        public double ProfitabilityAdjustment { get; set; }
        public AdjustmentType ProfitabilityAdjustmentType { get; set; }
        public bool Enabled { get; set; }

        public bool NotifyOnBlockFound { get; set; }
        public bool NotifyOnShareAccepted { get; set; }
    }
}
