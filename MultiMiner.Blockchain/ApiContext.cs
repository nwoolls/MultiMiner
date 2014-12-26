using MultiMiner.Blockchain.Data;
using MultiMiner.ExchangeApi;
using MultiMiner.ExchangeApi.Data;
using MultiMiner.Utility.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiMiner.Blockchain
{
    public class ApiContext : IApiContext
    {
        public IEnumerable<ExchangeInformation> GetExchangeInformation()
        {
            ApiWebClient webClient = new ApiWebClient();
            webClient.Encoding = Encoding.UTF8;

            string response = webClient.DownloadFlakyString(new Uri(GetApiUrl()));

            Dictionary<string, TickerEntry> tickerEntries = JsonConvert.DeserializeObject<Dictionary<string, TickerEntry>>(response);

            List<ExchangeInformation> results = new List<ExchangeInformation>();

            foreach (KeyValuePair<string, TickerEntry> keyValuePair in tickerEntries)
            {
                results.Add(new ExchangeInformation()
                {
                    SourceCurrency = "BTC",
                    TargetCurrency = keyValuePair.Key,
                    TargetSymbol = keyValuePair.Value.Symbol,
                    ExchangeRate = keyValuePair.Value.Last
                });
            }

            return results;
        }

        public string GetInfoUrl()
        {
            return "https://blockchain.info";
        }

        public string GetApiUrl()
        {
            //use HTTP as HTTPS is down at times and returns:
            //The request was aborted: Could not create SSL/TLS secure channel.
            return "https://blockchain.info/ticker";
        }

        public string GetApiName()
        {
            return "Blockchain";
        }
    }
}
