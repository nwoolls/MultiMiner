using MultiMiner.Utility.Serialization;

namespace MultiMiner.Xgminer.Data
{
    public class CoinAlgorithm
    {
        //used for determining work utility / effective hashrate
        public enum AlgorithmFamily
        {
            Unknown,
            SHA2,
            SHA3,
            Scrypt,
            Lyra2
        }

        //general info
        public string Name { get; set; }
        public AlgorithmFamily Family { get; set; }
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
