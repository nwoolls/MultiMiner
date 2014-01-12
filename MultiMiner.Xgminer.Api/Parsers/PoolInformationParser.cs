using MultiMiner.Xgminer.Api.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using MultiMiner.Xgminer.Api.Extensions;

namespace MultiMiner.Xgminer.Api.Parsers
{
    class PoolInformationParser : ResponseTextParser
    {
        public static void ParseTextForDeviceDetails(string text, List<PoolInformationResponse> poolInformation)
        {
            List<string> deviceBlob = text.Split('|').ToList();
            deviceBlob.RemoveAt(0);

            foreach (string deviceText in deviceBlob)
            {
                if (deviceText == "\0")
                    continue;

                //bfgminer may have multiple entries for the same key, e.g. Hardware Errors
                //seen with customer data/hardware
                //remove dupes using Distinct()
                var deviceAttributes = deviceText.Split(',').ToList().Distinct();

                Dictionary<string, string> keyValuePairs = deviceAttributes
                  .Where(value => value.Contains('='))
                  .Select(value => value.Split('='))
                  .ToDictionary(pair => pair[0], pair => pair[1]);

                //seen Count == 0 with user API logs
                if (keyValuePairs.Count > 0)
                {
                    PoolInformationResponse newPool = new PoolInformationResponse();

                    newPool.Index = int.Parse(keyValuePairs["POOL"]);

                    //user bug reports indicate this key may not exist
                    if (keyValuePairs.ContainsKey("URL"))
                        newPool.Url = keyValuePairs["URL"];

                    //user bug reports indicate this key may not exist
                    if (keyValuePairs.ContainsKey("Status"))
                        newPool.Status = keyValuePairs["Status"];

                    newPool.Priority = TryToParseInt(keyValuePairs, "Priority", 0);
                    newPool.Quota = TryToParseInt(keyValuePairs, "Quota", 0);

                    if (keyValuePairs.ContainsKey("Long Pool"))
                        newPool.LongPoll = !keyValuePairs["Long Pool"].Equals("n", StringComparison.OrdinalIgnoreCase);

                    newPool.GetWorks = TryToParseInt(keyValuePairs, "Getworks", 0);
                    newPool.Accepted = TryToParseInt(keyValuePairs, "Accepted", 0);
                    newPool.Rejected = TryToParseInt(keyValuePairs, "Rejected", 0);
                    newPool.Works = TryToParseInt(keyValuePairs, "Works", 0);
                    newPool.Discarded = TryToParseInt(keyValuePairs, "Discarded", 0);
                    newPool.Stale = TryToParseInt(keyValuePairs, "Stale", 0);
                    newPool.GetFailures = TryToParseInt(keyValuePairs, "Get Failures", 0);
                    newPool.RemoteFailures = TryToParseInt(keyValuePairs, "Remote Failures", 0);
                    newPool.User = keyValuePairs["User"];
                    newPool.LastShareTime = TryToParseInt(keyValuePairs, "Last Share Time", 0).UnixTimeToDateTime();
                    newPool.Diff1Shares = TryToParseInt(keyValuePairs, "Diff1 Shares", 0);
                    newPool.Proxy = keyValuePairs["Proxy"];
                    newPool.DifficultyAccepted = TryToParseDouble(keyValuePairs, "Difficulty Accepted", 0.0);
                    newPool.DifficultyRejected = TryToParseDouble(keyValuePairs, "Difficulty Rejected", 0.0);
                    newPool.DifficultyStale = TryToParseDouble(keyValuePairs, "Difficulty Stale", 0.0);
                    newPool.LastShareDifficulty = TryToParseDouble(keyValuePairs, "Last Share Difficulty", 0.0);
                    newPool.HasStratum = keyValuePairs["Has Stratum"].Equals("true", StringComparison.OrdinalIgnoreCase);
                    newPool.StratumActive = keyValuePairs["Stratum Active"].Equals("true", StringComparison.OrdinalIgnoreCase);
                    newPool.StratumUrl = keyValuePairs["Stratum URL"];
                    newPool.BestShare = TryToParseInt(keyValuePairs, "Best Share", 0);
                    newPool.PoolRejectedPercent = TryToParseDouble(keyValuePairs, "Pool Rejected%", 0.0);
                    newPool.PoolStalePercent = TryToParseDouble(keyValuePairs, "Pool Stale%", 0.0);

                    poolInformation.Add(newPool);
                }
            }
        }
    }
}
