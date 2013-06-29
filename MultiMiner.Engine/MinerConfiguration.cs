using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Engine
{
    class MinerConfiguration
    {
        public Dictionary<CoinAlgorithm, string> AlgorithmFlags { get; set; }
        public int Intensity { get; set; }
    }
}
