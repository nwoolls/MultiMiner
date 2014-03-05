using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    class VersionInformationParser : ResponseTextParser
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
    }
}
