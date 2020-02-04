using MultiMiner.CoinApi.Data;
using MultiMiner.Xgminer.Data;
using System;
using System.Linq;

namespace MultiMiner.WhatToMine.Extensions
{
    static class CoinInformationExtensions
    {
        public static void PopulateFromJson(this CoinInformation coinInformation, Data.ApiCoinInformation apiCoinInformation)
        {
            coinInformation.Symbol = FixSymbolName(apiCoinInformation.Tag, apiCoinInformation.Algorithm);
            coinInformation.Algorithm = FixAlgorithmName(apiCoinInformation.Algorithm);
            coinInformation.CurrentBlocks = apiCoinInformation.Last_Block;
            coinInformation.Difficulty = apiCoinInformation.Difficulty;
            coinInformation.Reward = apiCoinInformation.Block_Reward;
            coinInformation.NetworkHashRate = apiCoinInformation.Nethash;
            coinInformation.Profitability = apiCoinInformation.Profitability;
            coinInformation.AdjustedProfitability = apiCoinInformation.Profitability;
            coinInformation.AverageProfitability = apiCoinInformation.Profitability24;
            coinInformation.NetworkHashRate = apiCoinInformation.Nethash;

            if (coinInformation.Symbol.Equals("BTC", StringComparison.OrdinalIgnoreCase))
                coinInformation.Price = 1;
            else
                coinInformation.Price = apiCoinInformation.Exchange_Rate;
        }

        private static string FixSymbolName(string symbol, string algorithm)
        {
            string result = symbol;

            if ("NICEHASH".Equals(result))
            {
                result = "NiceHash:" + algorithm.Replace(AlgorithmFullNames.SHA256, AlgorithmNames.SHA256);
            }

            return result;
        }

        private static string FixAlgorithmName(string algorithm)
        {
            string result = algorithm;
            if (algorithm.Equals(ApiContext.ScryptNFactor, StringComparison.OrdinalIgnoreCase))
            {
                result = AlgorithmFullNames.ScryptN;
            }
            else
            {
                KnownAlgorithm knownAlgorithm = KnownAlgorithms.Algorithms.SingleOrDefault(a => a.Name.Equals(algorithm, StringComparison.OrdinalIgnoreCase));
                if (knownAlgorithm != null) result = knownAlgorithm.FullName;
            }
            return result;
        }
    }
}
