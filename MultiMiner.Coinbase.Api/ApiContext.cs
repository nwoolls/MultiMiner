using Newtonsoft.Json;
using System;
using System.Net;

namespace MultiMiner.Coinbase.Api
{
    public static class ApiContext
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

        public static SellPrices GetSellPrices()
        {
            WebClient webClient = new ApiWebClient();

            string response = webClient.DownloadString(new Uri("https://coinbase.com/api/v1/prices/sell"));

            SellPrices sellPrices = JsonConvert.DeserializeObject<SellPrices>(response);
            return sellPrices;
        }
    }
}
