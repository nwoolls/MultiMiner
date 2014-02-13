using Newtonsoft.Json;
using System;
using System.Net;

namespace MultiMiner.Coinbase
{
    public static class ApiContext
    {
        private class ApiWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                //default is 100s - far too long for our API calls
                //if API is being flakey we don't want calls taking 100s to timeout
                //lets go with 10s
                w.Timeout = 10 * 1000;
                return w;
            }
        }

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
