using MultiMiner.Coin.Api;
using Newtonsoft.Json.Linq;

namespace MultiMiner.CoinWarz.Api
{
    public static class CoinInformationExtensions
    {
        public static void PopulateFromJson(this CoinInformation coinInformation, JToken jToken)
        {
            coinInformation.Symbol = jToken.Value<string>("CoinTag");
            coinInformation.Name = jToken.Value<string>("CoinName");
            coinInformation.Algorithm = jToken.Value<string>("Algorithm");
            coinInformation.CurrentBlocks = jToken.Value<int>("BlockCount");
            coinInformation.Difficulty = jToken.Value<double>("Difficulty");
            coinInformation.Reward = jToken.Value<double>("BlockReward");
            coinInformation.Price = jToken.Value<double>("ExchangeRate");
            coinInformation.Exchange = jToken.Value<string>("Exchange");
            coinInformation.Profitability = jToken.Value<double>("ProfitRatio");
            coinInformation.AdjustedProfitability = jToken.Value<double>("ProfitRatio");
            coinInformation.AverageProfitability = jToken.Value<double>("AvgProfitRatio");
        }
    }
}
