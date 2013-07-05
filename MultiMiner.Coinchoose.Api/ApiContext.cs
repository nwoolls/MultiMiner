using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace MultiMiner.Coinchoose.Api
{
    public static class ApiContext
    {
        public static List<CoinInformation> GetCoinInformation()
        {
            WebClient client = new WebClient();
            const string userAgent = "MultiMiner/V1";
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
