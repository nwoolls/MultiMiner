using System.Collections.Generic;

namespace MultiMiner.Engine.Configuration
{
    public class MinerConfiguration
    {
        public MinerConfiguration()
        {
            AlgorithmFlags = new SerializableDictionary<CoinAlgorithm, string>();
        }

        public SerializableDictionary<CoinAlgorithm, string> AlgorithmFlags { get; set; }
    }
}
