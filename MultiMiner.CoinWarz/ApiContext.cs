using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.CoinWarz.Extensions;
using MultiMiner.Utility.Net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.CoinWarz
{
    public class ApiContext : IApiContext
    {
        private readonly string apiKey;
        private readonly string urlParms;
        public ApiContext(string apiKey, string urlParms = "")
        {
            this.apiKey = apiKey;
            this.urlParms = urlParms;
        }

        public IEnumerable<CoinInformation> GetCoinInformation(string userAgent = "")
        {
            WebClient client = new ApiWebClient();
            if (!string.IsNullOrEmpty(userAgent))
                client.Headers.Add("user-agent", userAgent);

            string apiUrl = GetApiUrl();

            string jsonString = client.DownloadString(apiUrl);

            JObject jsonObject = JObject.Parse(jsonString);
            
            if (!jsonObject.Value<bool>("Success"))
            {
                throw new CoinApiException(jsonObject.Value<string>("Message"));
            }

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

        public string GetApiUrl()
        {            
            return String.Format(@"http://www.coinwarz.com/v1/api/profitability/?apikey={0}&algo=all&{1}", apiKey, urlParms);
        }

        public string GetInfoUrl()
        {
            return @"http://www.coinwarz.com/cryptocurrency";
        }

        public string GetApiName()
        {
            return "CoinWarz.com";
        }
    }
}
