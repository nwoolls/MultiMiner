using MultiMiner.CoinApi.Data;
using Newtonsoft.Json.Linq;
using System;

namespace MultiMiner.CoinChoose.Extensions
{
    public static class CoinInformationExtensions
    {
        public static void PopulateFromJson(this CoinInformation coinInformation, JToken jToken)
        {
            coinInformation.Symbol = jToken.Value<string>("symbol");
            coinInformation.Name = jToken.Value<string>("name");
            coinInformation.Algorithm = jToken.Value<string>("algo");

            try
            {
                coinInformation.CurrentBlocks = jToken.Value<int>("currentBlocks");
            }
            catch (InvalidCastException)
            {
                //handle System.InvalidCastException: Null object cannot be converted to a value type.
                //tried this but didn't work: if (jToken["currentBlocks"] != null)
                coinInformation.CurrentBlocks = 0;
            }

            //difficulty may be NULL, e.g. for Terracoin when the block explorer is down
            try
            {
                coinInformation.Difficulty = jToken.Value<double>("difficulty");
            }
            catch (SystemException ex)
            {
                //handle System.InvalidCastException: Null object cannot be converted to a value type.
                //tried this but didn't work: if (jToken["difficulty"] != null)
                if ((ex is InvalidCastException) || (ex is FormatException))
                    coinInformation.Difficulty = 0;
                else
                    throw;
            }

            coinInformation.Reward = jToken.Value<double>("reward");

            try
            {
                coinInformation.MinimumBlockTime = jToken.Value<double>("minBlockTime"); //this one can be null too (?)
            }
            catch (InvalidCastException)
            {
                //handle System.InvalidCastException: Null object cannot be converted to a value type.
                //tried this but didn't work: if (jToken["minBlockTime"] != null)
                coinInformation.MinimumBlockTime = 0;
            }

            try
            {
                coinInformation.NetworkHashRate = jToken.Value<Int64>("networkhashrate"); //this one can be null too (?)
            }
            catch (InvalidCastException)
            {
                //handle System.InvalidCastException: Null object cannot be converted to a value type.
                //tried this but didn't work: if (jToken["networkhashrate"] != null)
                coinInformation.NetworkHashRate = 0;
            }

            coinInformation.Price = jToken.Value<double>("price");
            coinInformation.Exchange = jToken.Value<string>("exchange");
            coinInformation.Profitability = jToken.Value<double>("ratio");
            coinInformation.AdjustedProfitability = jToken.Value<double>("adjustedratio");
            
            try
            {
                coinInformation.AverageProfitability = jToken.Value<double>("avgProfit"); //this one can be null too (?)
            }
            catch (InvalidCastException)
            {
                //handle System.InvalidCastException: Null object cannot be converted to a value type.
                //tried this but didn't work: if (jToken["avgProfit"] != null)
                coinInformation.AverageProfitability = 0;
            }

            coinInformation.AverageHashRate = jToken.Value<double>("avgHash");
        }
    }
}
