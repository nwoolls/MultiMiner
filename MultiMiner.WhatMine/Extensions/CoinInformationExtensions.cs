using MultiMiner.CoinApi.Data;
using MultiMiner.Xgminer.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace MultiMiner.WhatMine.Extensions
{
    public static class CoinInformationExtensions
    {
        public static void PopulateFromJson(this CoinInformation coinInformation, JToken jToken)
        {
            coinInformation.Symbol = jToken.Value<string>("market_code");
            coinInformation.Name = jToken.Value<string>("name");

            string algorithm = jToken.Value<string>("algorithm");
            coinInformation.Algorithm = FixAlgorithmName(algorithm);

            coinInformation.CurrentBlocks = jToken.Value<int>("height");
            coinInformation.Difficulty = jToken.Value<double>("difficulty");
            coinInformation.Reward = jToken.Value<double>("reward");
            coinInformation.Exchange = jToken.Value<string>("exchange_name");
            coinInformation.Income = jToken.Value<double>("btc_per_day");

            if (coinInformation.Symbol.Equals("BTC", System.StringComparison.OrdinalIgnoreCase))
                coinInformation.Price = 1;
            else
                coinInformation.Price = jToken.Value<double>("exchange_rate");
        }

        private static string FixAlgorithmName(string algorithm)
        {
            string result = algorithm;
            if (algorithm.Equals(ApiContext.ScryptNFactor, StringComparison.OrdinalIgnoreCase))
                result = AlgorithmFullNames.ScryptN;
            else
            {
                KnownAlgorithm knownAlgorithm = KnownAlgorithms.Algorithms.SingleOrDefault(a => a.Name.Equals(algorithm, StringComparison.OrdinalIgnoreCase));
                if (knownAlgorithm != null) result = knownAlgorithm.FullName;
            }
            return result;
        }
    }
}
