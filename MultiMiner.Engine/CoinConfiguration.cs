using System.Collections.Generic;

namespace MultiMiner.Engine
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
