using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.WhatMine.Extensions;
using MultiMiner.Utility.Net;
using MultiMiner.Xgminer.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace MultiMiner.WhatMine
{
    public class ApiContext : IApiContext
    {
        public static string ScryptNFactor = "SCRYPTNFACTOR";

        private readonly string apiKey;
        public ApiContext(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public IEnumerable<CoinInformation> GetCoinInformation(string userAgent = "")
        {
            WebClient client = new ApiWebClient();
            if (!string.IsNullOrEmpty(userAgent))
                client.Headers.Add("user-agent", userAgent);

            string apiUrl = GetApiUrl();
            string jsonString = client.DownloadString(apiUrl);
            JObject jsonObject = JObject.Parse(jsonString);

            string resultStatus = jsonObject.Value<string>("status");
            if (!resultStatus.Equals("success", StringComparison.OrdinalIgnoreCase))
                throw new CoinApiException(resultStatus);

            List<CoinInformation> result = new List<CoinInformation>();

            JArray jsonArray = jsonObject.Value<JArray>("Data");
            foreach (JToken jToken in jsonArray)
            {
                CoinInformation coinInformation = new CoinInformation();
                coinInformation.PopulateFromJson(jToken);
                if (coinInformation.Difficulty > 0)
                    //only add coins with valid info since the user may be basing
                    //strategies on Difficulty
                    result.Add(coinInformation);
            }

            CalculateProfitability(result);

            return result;
        }

        private static void CalculateProfitability(List<CoinInformation> coinInformation)
        {
            CoinInformation btcInformation = coinInformation.Single(mpi => mpi.Symbol.Equals("BTC", StringComparison.OrdinalIgnoreCase));

            foreach (CoinInformation otherInformation in coinInformation)
            {
                otherInformation.Profitability = (otherInformation.Income / btcInformation.Income) * 100;
                otherInformation.AdjustedProfitability = otherInformation.Profitability;
                otherInformation.AverageProfitability = otherInformation.Profitability;
            }
        }

        public string GetApiUrl()
        {
            string apiUrl = String.Format(@"http://whatmine.com/v1/api/profitability?key={0}&currency=USD", apiKey);

            //for some reason, the defaults for CoinWarz are not normalized for a consistent set of
            //hardware - we need to do that all via arguments
            const int Hashrate = 1000000;

            apiUrl = String.Format("{0}&{1}_speed={2}", 
                apiUrl,
                AlgorithmNames.SHA256.ToUpper(),
                AlgorithmMultipliers.SHA256 * Hashrate
                //API wants Gh/s
                / 1000 / 1000);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.Scrypt.ToUpper(),
                AlgorithmMultipliers.Scrypt * Hashrate);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                ScryptNFactor,
                AlgorithmMultipliers.ScryptN * Hashrate);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.X11.ToUpper(),
                AlgorithmMultipliers.X11 * Hashrate);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.X13.ToUpper(),
                AlgorithmMultipliers.X13 * Hashrate);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.Keccak.ToUpper(),
                AlgorithmMultipliers.Keccak * Hashrate
                //API wants Mh/s
                / 1000);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.Quark.ToUpper(),
                AlgorithmMultipliers.Quark * Hashrate);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.Groestl.ToUpper(),
                AlgorithmMultipliers.Groestl * Hashrate
                //API wants Mh/s
                / 1000);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.X13.ToUpper(),
                AlgorithmMultipliers.X13 * Hashrate
                //API wants Mh/s
                / 1000);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.X15.ToUpper(),
                AlgorithmMultipliers.X15 * Hashrate
                //API wants Mh/s
                / 1000);

            apiUrl = String.Format("{0}&{1}_speed={2}",
                apiUrl,
                AlgorithmNames.Nist5.ToUpper(),
                AlgorithmMultipliers.Nist5 * Hashrate
                //API wants Mh/s
                / 1000);

            return apiUrl;
        }

        public string GetInfoUrl()
        {
            return @"http://whatmine.com";
        }

        public string GetApiName()
        {
            return "WhatMine.com";
        }
    }
}
