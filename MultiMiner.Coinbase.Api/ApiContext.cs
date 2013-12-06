using Newtonsoft.Json;
using System;
using System.Net;

namespace MultiMiner.Coinbase.Api
{
    public static class ApiContext
    {
        public static SellPrices GetSellPrices()
        {
            WebClient webClient = new WebClient();

            string response = webClient.DownloadString(new Uri("https://coinbase.com/api/v1/prices/sell"));

            SellPrices sellPrices = JsonConvert.DeserializeObject<SellPrices>(response);
            return sellPrices;
        }
    }
}
