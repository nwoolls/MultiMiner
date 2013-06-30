using MultiMiner.Xgminer;
using System.Collections.Generic;

namespace MultiMiner.Engine.Configuration
{
    public class CoinConfiguration
    {
        public CoinConfiguration()
        {
            this.Pools = new List<MiningPool>();
        }

        public CryptoCoin Coin { get; set; }
        public List<MiningPool> Pools { get; set; }
    }
}
