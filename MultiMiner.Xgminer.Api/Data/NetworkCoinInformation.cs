using MultiMiner.Xgminer.Data;

namespace MultiMiner.Xgminer.Api.Data
{
    public class NetworkCoinInformation
    {
        public const string UnknownAlgorithm = "unknown";

        public string Algorithm { get; set; }
        public long CurrentBlockTime { get; set; }
        public string CurrentBlockHash { get; set; }
        public bool LongPoll { get; set; }
        public double NetworkDifficulty { get; set; }
    }
}
