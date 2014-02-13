using MultiMiner.Xgminer.Data;
using System;

namespace MultiMiner.Engine.Data
{
    //marked Serializable to allow deep cloning of CoinConfiguration
    [Serializable]
    public class CryptoCoin
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public CoinAlgorithm Algorithm { get; set; }
    }
}
