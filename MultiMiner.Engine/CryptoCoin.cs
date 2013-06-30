using MultiMiner.Xgminer;

namespace MultiMiner.Engine
{
    public class CryptoCoin
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public CoinAlgorithm Algorithm { get; set; }
    }
}
