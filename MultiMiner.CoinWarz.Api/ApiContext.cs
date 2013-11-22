using MultiMiner.Coin.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.CoinWarz.Api
{
    public class ApiContext : IApiContext
    {
        private readonly string apiKey;
        public ApiContext(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public IEnumerable<CoinInformation> GetCoinInformation(string userAgent = "",
            BaseCoin profitabilityBasis = BaseCoin.Bitcoin)
        {
            WebClient client = new WebClient();
            if (!string.IsNullOrEmpty(userAgent))
                client.Headers.Add("user-agent", userAgent);

            string apiUrl = GetApiUrl(profitabilityBasis);

            string jsonString = client.DownloadString(apiUrl);

            JObject jsonObject = JObject.Parse(jsonString);
            
            JArray jsonArray = jsonObject.Value<JArray>("Data");

            List<CoinInformation> result = new List<CoinInformation>();

            foreach (JToken jToken in jsonArray)
            {
                CoinInformation coinInformation = new CoinInformation();
                coinInformation.PopulateFromJson(jToken);
                if (coinInformation.Difficulty > 0)
                    //only add coins with valid info since the user may be basing
                    //strategies on Difficulty
                    result.Add(coinInformation);
            }

            return result;
        }

        public string GetApiUrl(BaseCoin profitabilityBasis)
        {
            return String.Format(@"http://www.coinwarz.com/v1/api/profitability/?apikey={0}&algo=all", apiKey);
        }
    }
}
