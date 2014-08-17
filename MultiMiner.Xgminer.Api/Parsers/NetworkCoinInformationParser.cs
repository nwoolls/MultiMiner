using MultiMiner.Xgminer.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    class NetworkCoinInformationParser : ResponseTextParser
    {
        public static void ParseTextForCoinNetworkInformation(string text, NetworkCoinInformation coinInformation)
        {
            List<string> deviceBlob = text.Split('|').ToList();
            deviceBlob.RemoveAt(0);

            foreach (string deviceText in deviceBlob)
            {
                List<string> textChunks = text.Split('|').ToList();

                Dictionary<string, string> keyValuePairs = GetDictionaryFromTextChunk(textChunks[0]);
                
                keyValuePairs = GetDictionaryFromTextChunk(textChunks[1]);

                coinInformation.Algorithm = keyValuePairs["Hash Method"];
                coinInformation.CurrentBlockTime = TryToParseInt(keyValuePairs, "Current Block Time", 0);
                coinInformation.CurrentBlockHash = keyValuePairs["Current Block Hash"];
                coinInformation.LongPoll = keyValuePairs["LP"].Equals("true", StringComparison.OrdinalIgnoreCase);
                coinInformation.NetworkDifficulty = TryToParseDouble(keyValuePairs, "Network Difficulty", 0.0);
            }
        }
    }
}
