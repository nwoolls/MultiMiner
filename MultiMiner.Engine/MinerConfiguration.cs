using System.Collections.Generic;

namespace MultiMiner.Engine
{
    class MinerConfiguration
    {
        public Dictionary<CoinAlgorithm, string> AlgorithmFlags { get; set; }
        public int Intensity { get; set; }
    }
}
