using MultiMiner.Coin.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.CoinChoose.Api
{
    public class ApiContext : IApiContext
    {
        private class ApiWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                //default is 100s - far too long for our API calls
                //if Azure is being flakey we don't want calls taking 100s to timeout
                //lets go with 10s
                w.Timeout = 10 * 1000;
                return w;
            }
        }

        public IEnumerable<CoinInformation> GetCoinInformation(string userAgent = "")
        {
            WebClient client = new ApiWebClient();
            if (!string.IsNullOrEmpty(userAgent))
                client.Headers.Add("user-agent", userAgent);

            string apiUrl = GetApiUrl();

            string jsonString = client.DownloadString(apiUrl);
            JArray jsonArray = JArray.Parse(jsonString);

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
            string apiUrl = "http://www.coinchoose.com/api.php";
            return apiUrl;
        }

        public string GetInfoUrl()
        {
            return "http://coinchoose.com/index.php";
        }
        
        public string GetApiName()
        {
            return "CoinChoose.com";
        }
    }
}
