namespace MultiMiner.Engine.Configuration
{
    public class StrategyConfiguration
    {
        public enum CoinSwitchStrategy
        {
            SingleMostProfitable = 0,
            AllMostProfitable = 1
        }

        public bool MineProfitableCoins { get; set; }
        public CoinSwitchStrategy SwitchStrategy { get; set; }
        public string MinimumProfitabilitySymbol { get; set; }
        public double? MinimumProfitabilityPercentage { get; set; }
        public double? MineMostProfitableOverridePercentage { get; set; }
    }
}
