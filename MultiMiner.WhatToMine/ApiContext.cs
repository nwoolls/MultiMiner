using MultiMiner.CoinApi;
using MultiMiner.CoinApi.Data;
using MultiMiner.Utility.Net;
using MultiMiner.WhatToMine.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MultiMiner.WhatToMine
{
    public class ApiContext : IApiContext
    {
        public static string ScryptNFactor = "Scrypt-N";

        public IEnumerable<CoinInformation> GetCoinInformation(string userAgent = "")
        {
            ApiWebClient client = new ApiWebClient();
            if (!string.IsNullOrEmpty(userAgent))
                client.Headers.Add("user-agent", userAgent);

            //get GPU coin info (scrypt, X11, etc)
            string jsonString = client.DownloadString(GetApiUrl());            
            Data.ApiResponse apiResponse = JsonConvert.DeserializeObject<Data.ApiResponse>(jsonString);

            //merge in ASIC coin info (sha-256, scrypt, etc)
            jsonString = client.DownloadString(GetAsicApiUrl());            
            Data.ApiResponse asicApiResponse = JsonConvert.DeserializeObject<Data.ApiResponse>(jsonString);
            foreach (string coinName in asicApiResponse.Coins.Keys)
                apiResponse.Coins[coinName] = asicApiResponse.Coins[coinName];

            List<CoinInformation> result = new List<CoinInformation>();

            foreach (string coinName in apiResponse.Coins.Keys)
            {
                CoinInformation coinInformation = new CoinInformation();
                coinInformation.Name = coinName;
                coinInformation.PopulateFromJson(apiResponse.Coins[coinName]);
                if (coinInformation.Difficulty > 0)
                    //only add coins with valid info since the user may be basing
                    //strategies on Difficulty
                    result.Add(coinInformation);
            }

            return result;
        }

        public string GetApiUrl()
        {
            return "https://www.whattomine.com/coins.json";
        }

        public string GetAsicApiUrl()
        {
            return "https://www.whattomine.com/asic.json";
        }

        public string GetInfoUrl()
        {
            return "https://www.whattomine.com/";
        }
        
        public string GetApiName()
        {
            return "WhatToMine.com";
        }
    }
}
