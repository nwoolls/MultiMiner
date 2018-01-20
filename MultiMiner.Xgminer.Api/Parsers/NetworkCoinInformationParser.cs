using MultiMiner.Xgminer.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    public class NetworkCoinInformationParser : ResponseTextParser
    {
        public static void ParseTextForCoinNetworkInformation(string text, NetworkCoinInformation coinInformation)
        {
            List<string> responseParts = ParseResponseText(text);
            if (responseParts.Count == 0)
                return;

            foreach (string responsePart in responseParts)
            {
                List<string> textChunks = text.Split('|').ToList();

                Dictionary<string, string> keyValuePairs = GetDictionaryFromTextChunk(textChunks[0]);
                
                keyValuePairs = GetDictionaryFromTextChunk(textChunks[1]);

                // user reported a network device that does / may not return "Hash Method"
                // see: https://github.com/nwoolls/MultiMiner/issues/336
                const string hashMethodKey = "Hash Method";
                if (keyValuePairs.ContainsKey(hashMethodKey))
                {
                    coinInformation.Algorithm = keyValuePairs[hashMethodKey];
                }
                else
                {
                    coinInformation.Algorithm = NetworkCoinInformation.UnknownAlgorithm;
                }

                coinInformation.CurrentBlockTime = TryToParseInt(keyValuePairs, "Current Block Time", 0);
                coinInformation.CurrentBlockHash = keyValuePairs["Current Block Hash"];
                coinInformation.LongPoll = keyValuePairs["LP"].Equals("true", StringComparison.OrdinalIgnoreCase);
                coinInformation.NetworkDifficulty = TryToParseDouble(keyValuePairs, "Network Difficulty", 0.0);
            }
        }
    }
}
