using MultiMiner.Xgminer;
using System.Collections.Generic;

namespace MultiMiner.Engine.Configuration
{
    public class XgminerConfiguration
    {
        public XgminerConfiguration()
        {
            AlgorithmFlags = new SerializableDictionary<CoinAlgorithm, string>();
        }

        public SerializableDictionary<CoinAlgorithm, string> AlgorithmFlags { get; set; }
    }
}
