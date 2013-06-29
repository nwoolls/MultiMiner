using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Engine
{
    class CryptoCoin
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public CoinAlgorithm Algorithm { get; set; }
    }
}
