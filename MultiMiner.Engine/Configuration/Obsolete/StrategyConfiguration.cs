namespace MultiMiner.Engine.Configuration.Obsolete
{
    [System.Obsolete("Kept for streaming old settings")]
    public class StrategyConfiguration
    {
        public enum CoinSwitchStrategy
        {
            SingleMostProfitable = 0,
            AllMostProfitable = 1
        }

        public enum CoinProfitabilityBasis
        {
            AdjustedProfitability = 0,
            AverageProfitability = 1,
            StraightProfitability = 2
        }

        public bool MineProfitableCoins { get; set; }
        public CoinSwitchStrategy SwitchStrategy { get; set; }
        public string MinimumProfitabilitySymbol { get; set; }
        public double? MinimumProfitabilityPercentage { get; set; }
        public double? MineMostProfitableOverridePercentage { get; set; }
        public CoinProfitabilityBasis ProfitabilityBasis { get; set; }
        public Coinchoose.Api.BaseCoin BaseCoin { get; set; }
    }
}
