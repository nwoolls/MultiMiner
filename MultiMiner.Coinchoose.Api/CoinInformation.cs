using Newtonsoft.Json.Linq;
using System;

namespace MultiMiner.Coinchoose.Api
{
    public class CoinInformation
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Algorithm { get; set; }
        public int CurrentBlocks { get; set; }
        public double Difficulty { get; set; }
        public double Reward { get; set; }
        public double MinimumBlockTime { get; set; }
        public Int64 NetworkHashRate { get; set; }
        public double Price { get; set; }
        public string Exchange { get; set; }
        public double Profitability { get; set; }
        public double AdjustedProfitability { get; set; }
        public double AverageProfitability { get; set; }
        public double AverageHashRate { get; set; }

        public void PopulateFromJson(JToken jToken)
        {
            Symbol = jToken.Value<string>("symbol");
            Name = jToken.Value<string>("name");
            Algorithm = jToken.Value<string>("algo");

            try
            {
                CurrentBlocks = jToken.Value<int>("currentBlocks");
            }
            catch (InvalidCastException)
            {
                //handle System.InvalidCastException: Null object cannot be converted to a value type.
                //tried this but didn't work: if (jToken["currentBlocks"] != null)
                CurrentBlocks = 0;
            }
            
            Difficulty = jToken.Value<double>("difficulty");
            Reward = jToken.Value<double>("reward");
            MinimumBlockTime = jToken.Value<double>("minBlockTime"); //this one can be null too (?)
            NetworkHashRate = jToken.Value<Int64>("networkhashrate");
            Price = jToken.Value<double>("price");
            Exchange = jToken.Value<string>("exchange");
            Profitability = jToken.Value<double>("ratio");
            AdjustedProfitability = jToken.Value<double>("adjustedratio");
            AverageProfitability = jToken.Value<double>("avgProfit");
            AverageHashRate = jToken.Value<double>("avgHash");
        }
    }
}
