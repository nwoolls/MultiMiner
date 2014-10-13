using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.CoinWarz.Extensions;
using MultiMiner.Utility.Net;
using MultiMiner.Xgminer.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.CoinWarz
{
    public class ApiContext : IApiContext
    {
        private readonly string apiKey;
        public ApiContext(string apiKey)
        {
            this.apiKey = apiKey;
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
            string apiUrl = String.Format(@"http://www.coinwarz.com/v1/api/profitability/?apikey={0}&algo=all", apiKey);

            //for some reason, the defaults for CoinWarz are not normalized for a consistent set of
            //hardware - we need to do that all via arguments
            const int Hashrate = 1000000;

            apiUrl = String.Format("{0}&{1}Power=0&{1}HashRate={2}", 
                apiUrl,
                AlgorithmNames.SHA256.ToLower(),
                AlgorithmMultipliers.SHA256 * Hashrate
                //API wants Gh/s
                / 1000 / 1000);

            apiUrl = String.Format("{0}&{1}Power=0&{1}HashRate={2}",
                apiUrl,
                AlgorithmNames.Scrypt.ToLower(),
                AlgorithmMultipliers.Scrypt * Hashrate);

            apiUrl = String.Format("{0}&{1}Power=0&{1}HashRate={2}",
                apiUrl,
                AlgorithmNames.ScryptN.ToLower(),
                AlgorithmMultipliers.ScryptN * Hashrate);

            apiUrl = String.Format("{0}&{1}Power=0&{1}HashRate={2}",
                apiUrl,
                AlgorithmNames.X11.ToLower(),
                AlgorithmMultipliers.X11 * Hashrate);

            apiUrl = String.Format("{0}&{1}Power=0&{1}HashRate={2}",
                apiUrl,
                AlgorithmNames.X13.ToLower(),
                AlgorithmMultipliers.X13 * Hashrate);

            apiUrl = String.Format("{0}&{1}Power=0&{1}HashRate={2}",
                apiUrl,
                AlgorithmNames.Keccak.ToLower(),
                AlgorithmMultipliers.Keccak * Hashrate
                //API wants Mh/s
                / 1000);

            apiUrl = String.Format("{0}&{1}Power=0&{1}HashRate={2}",
                apiUrl,
                AlgorithmNames.Quark.ToLower(),
                AlgorithmMultipliers.Quark * Hashrate);

            apiUrl = String.Format("{0}&{1}Power=0&{1}HashRate={2}",
                apiUrl,
                AlgorithmNames.Groestl.ToLower(),
                AlgorithmMultipliers.Groestl * Hashrate
                //API wants Mh/s
                / 1000);

            return apiUrl;
        }

        public string GetInfoUrl()
        {
            return String.Format(@"http://www.coinwarz.com/cryptocurrency", apiKey);
        }

        public string GetApiName()
        {
            return "CoinWarz.com";
        }
    }
}
