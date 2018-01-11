using MultiMiner.Utility.Serialization;

namespace MultiMiner.Xgminer.Data
{
    public class CoinAlgorithm
    {
        //used for determining work utility / effective hashrate
        public double PoolMultiplier { get; set; }

        //used for determining profitability from hashrate
        public double DifficultyMultiplier { get; set; }

        //general info
        public string Name { get; set; }
        public bool BuiltIn { get; set; }
        
        //coin API info
        public string FullName { get; set; }

        //miner info
        public string DefaultMiner { get; set; }
        //per-miner arguments, e.g. kernel args
        public SerializableDictionary<string, string> MinerArguments { get; set; }

        public CoinAlgorithm()
        {
            MinerArguments = new SerializableDictionary<string, string>();
        }
    }
}
