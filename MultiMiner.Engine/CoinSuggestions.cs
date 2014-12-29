using MultiMiner.CoinApi.Data;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Engine
{
    public class CoinSuggestions
    {
        public static IEnumerable<CoinInformation> GetCoinsToMine(
            List<CoinInformation> coinApiInformation,
            CoinSuggestionsAlgorithm algorithm,
            Strategy strategyConfiguration,
            List<Coin> existingCoinConfigurations)
        {
            IEnumerable<CoinInformation> coinsToMine = coinApiInformation;

            //filter coins based on algorithm
            coinsToMine = FilterCoinsOnAlgorithm(coinsToMine, algorithm);

            //order coins based on mining strategies
            coinsToMine = OrderCoinsOnStrategy(coinsToMine, strategyConfiguration);

            //filter based on MinimumThresholdSymbol
            coinsToMine = FilterCoinsOnThresholdSymbol(coinsToMine, strategyConfiguration);

            //filter based on MinimumThresholdValue
            coinsToMine = FilterCoinsOnThresholdValue(coinsToMine, strategyConfiguration);

            //added checks for coin.Symbol and coin.Exchange
            //current CoinChoose.com feed for LTC profitability has a NULL exchange for Litecoin
            coinsToMine = FilterOutConfiguredCoins(coinsToMine, existingCoinConfigurations);

            //suggest the top 3 coins
            coinsToMine = coinsToMine.Take(3);

            return coinsToMine;
        }

        private static IEnumerable<CoinInformation> FilterOutConfiguredCoins(IEnumerable<CoinInformation> coins, List<Coin> existingConfigurations)
        {
            IEnumerable<CoinInformation> unconfiguredCoins = coins.Where(coin =>
                {
                    return !String.IsNullOrEmpty(coin.Symbol) &&
                        !existingConfigurations.Any(config => config.PoolGroup.Id.Equals(coin.Symbol, StringComparison.OrdinalIgnoreCase));
                });
            return unconfiguredCoins;
        }

        private static IEnumerable<CoinInformation> FilterCoinsOnThresholdValue(IEnumerable<CoinInformation> coins, Strategy strategyConfiguration)
        {
            if (strategyConfiguration.MinimumThresholdValue.HasValue)
            {
                List<CoinInformation> coinList = coins.ToList();

                switch (strategyConfiguration.MiningBasis)
                {
                    case Strategy.CoinMiningBasis.Profitability:
                        switch (strategyConfiguration.ProfitabilityKind)
                        {
                            case Strategy.CoinProfitabilityKind.AdjustedProfitability:
                                coinList = coinList.Where(c => c.AdjustedProfitability >= strategyConfiguration.MinimumThresholdValue).ToList();
                                break;
                            case Strategy.CoinProfitabilityKind.AverageProfitability:
                                coinList = coinList.Where(c => c.AverageProfitability >= strategyConfiguration.MinimumThresholdValue).ToList();
                                break;
                            case Strategy.CoinProfitabilityKind.StraightProfitability:
                                coinList = coinList.Where(c => c.Profitability >= strategyConfiguration.MinimumThresholdValue).ToList();
                                break;
                        }
                        break;
                    case Strategy.CoinMiningBasis.Difficulty:
                        coinList = coinList.Where(c => c.Difficulty >= strategyConfiguration.MinimumThresholdValue).ToList();
                        break;
                    case Strategy.CoinMiningBasis.Price:
                        coinList = coinList.Where(c => c.Price >= strategyConfiguration.MinimumThresholdValue).ToList();
                        break;
                }

                coins = coinList;
            }
            return coins;
        }

        private static IEnumerable<CoinInformation> FilterCoinsOnThresholdSymbol(IEnumerable<CoinInformation> coins, Strategy strategyConfiguration)
        {
            if (!String.IsNullOrEmpty(strategyConfiguration.MinimumThresholdSymbol))
            {
                List<CoinInformation> coinList = coins.ToList();

                int coinIndex = coinList.FindIndex(c => c.Symbol.Equals(strategyConfiguration.MinimumThresholdSymbol, StringComparison.OrdinalIgnoreCase));
                if (coinIndex >= 0)
                    coinList.RemoveRange(coinIndex + 1, coinList.Count - coinIndex - 1);

                coins = coinList;
            }
            return coins;
        }

        private static IEnumerable<CoinInformation> OrderCoinsOnStrategy(IEnumerable<CoinInformation> coins, Strategy strategyConfiguration)
        {
            IEnumerable<CoinInformation> orderedCoins = coins.OrderByDescending(c => c.AverageProfitability);

            switch (strategyConfiguration.MiningBasis)
            {
                case Strategy.CoinMiningBasis.Difficulty:
                    orderedCoins = coins.OrderBy(c => c.Difficulty);
                    break;
                case Strategy.CoinMiningBasis.Price:
                    orderedCoins = coins.OrderByDescending(c => c.Price);
                    break;
            }

            return orderedCoins;
        }

        private static IEnumerable<CoinInformation> FilterCoinsOnAlgorithm(IEnumerable<CoinInformation> coins, CoinSuggestionsAlgorithm algorithm)
        {
            if (algorithm == CoinSuggestionsAlgorithm.SHA256)
                coins = coins.Where(c => c.Algorithm.Equals(AlgorithmFullNames.SHA256, StringComparison.OrdinalIgnoreCase));
            else if (algorithm == CoinSuggestionsAlgorithm.Scrypt)
                coins = coins.Where(c => c.Algorithm.Equals(AlgorithmFullNames.Scrypt, StringComparison.OrdinalIgnoreCase));

            return coins;
        }
    }
}
