using System;

namespace MultiMiner.CoinApi.Data
{
    public class CoinInformation
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Algorithm { get; set; }
        public long CurrentBlocks { get; set; }
        public double Difficulty { get; set; }
        public double Reward { get; set; }
        public double MinimumBlockTime { get; set; }
        public double NetworkHashRate { get; set; }
        public double Price { get; set; }
        public double Income { get; set; }
        public string Exchange { get; set; }
        public double Profitability { get; set; }
        public double AdjustedProfitability { get; set; }
        public double AverageProfitability { get; set; }
        public double AverageHashRate { get; set; }
        public bool IsMultiCoin { get; set; }
    }
}
