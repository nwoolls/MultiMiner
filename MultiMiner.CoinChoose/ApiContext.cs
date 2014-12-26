using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.CoinChoose.Extensions;
using MultiMiner.Utility.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MultiMiner.CoinChoose
{
    public class ApiContext : IApiContext
    {
        public IEnumerable<CoinInformation> GetCoinInformation(string userAgent = "")
        {
            ApiWebClient client = new ApiWebClient();
            if (!string.IsNullOrEmpty(userAgent))
                client.Headers.Add("user-agent", userAgent);

            string apiUrl = GetApiUrl();

            string jsonString = client.DownloadFlakyString(apiUrl);

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
