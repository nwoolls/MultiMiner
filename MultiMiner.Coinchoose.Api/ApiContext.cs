using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.Coinchoose.Api
{
    public static class ApiContext
    {
        public static List<CoinInformation> GetCoinInformation(string userAgent = "")
        {
            WebClient client = new WebClient();
            if (!string.IsNullOrEmpty(userAgent))
                client.Headers.Add("user-agent", userAgent);

            string jsonString = client.DownloadString("http://www.coinchoose.com/api.php");
            JArray jsonArray = JArray.Parse(jsonString);

            List<CoinInformation> result = new List<CoinInformation>();

            foreach (JToken jToken in jsonArray)
            {
                CoinInformation coinInformation = new CoinInformation();
                coinInformation.PopulateFromJson(jToken);
                result.Add(coinInformation);
            }

            return result;
        }
    }
}
