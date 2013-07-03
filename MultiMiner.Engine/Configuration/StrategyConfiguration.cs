using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public int PermanentDeviceCount { get; set; }
        public string PermanentCoinSymbol { get; set; }
    }
}
