using MultiMiner.Utility.Net;
using Newtonsoft.Json;
using System;
using System.Net;

namespace MultiMiner.Coinbase
{
    public static class ApiContext
    {
        public static Data.SellPrices GetSellPrices()
        {
            WebClient webClient = new ApiWebClient();

            string response = webClient.DownloadString(new Uri(GetApiUrl()));

            Data.SellPrices sellPrices = JsonConvert.DeserializeObject<Data.SellPrices>(response);
            return sellPrices;
        }

        public static string GetInfoUrl()
        {
            return "https://coinbase.com";
        }

        public static string GetApiUrl()
        {
            return "https://coinbase.com/api/v1/prices/sell";
        }

        public static string GetApiName()
        {
            return "Coinbase";
        }
    }
}
