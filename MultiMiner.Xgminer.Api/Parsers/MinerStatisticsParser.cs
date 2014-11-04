using MultiMiner.Xgminer.Api.Data;
using System;
using System.Collections.Generic;

namespace MultiMiner.Xgminer.Api.Parsers
{
    class MinerStatisticsParser : ResponseTextParser
    {
        public static void ParseTextForMinerStatistics(string text, List<MinerStatistics> minerStatistics)
        {
            List<string> responseParts = ParseResponseText(text);
            if (responseParts.Count == 0)
                return;

            foreach (string responsePart in responseParts)
            {
                Dictionary<string, string> keyValuePairs = ParseResponsePart(responsePart);

                //check for key-value pairs, seen Count == 0 with user API logs
                if (keyValuePairs.Count <= 0)
                    continue;

                //ignore response parts without a STATS key
                //allows for custom responses (e.g. newer S3 firmware)
                if (!keyValuePairs.ContainsKey("STATS"))
                    continue;

                string id = String.Empty;

                //user bug reports indicate this key may not exist
                if (keyValuePairs.ContainsKey("ID"))
                    id = keyValuePairs["ID"];

                if (id.StartsWith("pool", StringComparison.OrdinalIgnoreCase))
                    //not concerned with pool information (for now)
                    continue;

                MinerStatistics newStatistics = new MinerStatistics();
                newStatistics.ID = id;

                string key = String.Empty;
                const int MaxChainCount = 16;
                for (int i = 1; i < MaxChainCount; i++)
                {
                    key = "chain_acs" + i;
                    if (keyValuePairs.ContainsKey(key))
                        newStatistics.ChainStatus[i - 1] = keyValuePairs[key];
                }

                key = "frequency";
                if (keyValuePairs.ContainsKey(key))
                    newStatistics.Frequency = TryToParseDouble(keyValuePairs, key, 0.0);

                key = "Elapsed";
                if (keyValuePairs.ContainsKey(key))
                    newStatistics.Elapsed = TryToParseInt(keyValuePairs, key, 0);

                minerStatistics.Add(newStatistics);
            }
        }
    }
}
