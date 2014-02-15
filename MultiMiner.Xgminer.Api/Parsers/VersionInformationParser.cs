using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    class VersionInformationParser
    {
        public static void ParseTextForVersionInformation(string text, MultiMiner.Xgminer.Api.Data.VersionInformation versionInformation)
        {
            List<string> textChunks = text.Split('|').ToList();

            Dictionary<string, string> keyValuePairs = GetDictionaryFromTextChunk(textChunks[0]);

            versionInformation.Name = keyValuePairs["Msg"].Replace(" versions", String.Empty);
            versionInformation.Description = keyValuePairs["Description"];

            keyValuePairs = GetDictionaryFromTextChunk(textChunks[1]);

            versionInformation.MinerVersion = keyValuePairs["CGMiner"];
            versionInformation.ApiVersion = keyValuePairs["API"];
        }

        private static Dictionary<string, string> GetDictionaryFromTextChunk(string textChunk)
        {
            IEnumerable<string> deviceAttributes = textChunk.Split(',').ToList().Distinct();
            Dictionary<string, string> keyValuePairs = deviceAttributes
              .Where(value => value.Contains('='))
              .Select(value => value.Split('='))
              .ToDictionary(pair => pair[0], pair => pair[1]);
            return keyValuePairs;
        }
    }
}
