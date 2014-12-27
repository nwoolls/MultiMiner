using MultiMiner.ExchangeApi;
using MultiMiner.ExchangeApi.Data;
using MultiMiner.Utility.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MultiMiner.Coinbase
{
    public class ApiContext : IApiContext
    {
        public IEnumerable<ExchangeInformation> GetExchangeInformation()
        {
            ApiWebClient webClient = new ApiWebClient();

            string response = webClient.DownloadFlakyString(new Uri(GetApiUrl()));

            Data.SellPrices sellPrices = JsonConvert.DeserializeObject<Data.SellPrices>(response);

            List<ExchangeInformation> results = new List<ExchangeInformation>();

            results.Add(new ExchangeInformation()
            {
                SourceCurrency = "BTC",
                TargetCurrency = "USD",
                TargetSymbol = "$",
                ExchangeRate = sellPrices.Subtotal.Amount
            });

            return results;
        }

        public string GetInfoUrl()
        {
            return "https://coinbase.com";
        }

        public string GetApiUrl()
        {
            return "https://coinbase.com/api/v1/prices/sell";
        }

        public string GetApiName()
        {
            return "Coinbase";
        }
    }
}
