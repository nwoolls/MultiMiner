using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Engine
{
    public class CoinConfiguration
    {
        public CoinConfiguration()
        {
            this.Pools = new List<MiningPool>();
        }

        public CryptoCoin CryptoCoin { get; set; }
        public List<MiningPool> Pools { get; set; }
    }
}
