using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    class VersionInformationParser : ResponseTextParser
    {
        public static void ParseTextForVersionInformation(string text, MultiMiner.Xgminer.Api.Data.VersionInformation versionInformation)
        {
            List<string> responseParts = text.Split('|').ToList();

            Dictionary<string, string> keyValuePairs = GetDictionaryFromTextChunk(responseParts[0]);

            //check for key-value pairs, seen Count == 0 with user API logs
            if (keyValuePairs.Count > 0)
            {
                versionInformation.Name = keyValuePairs["Msg"].Replace(" versions", String.Empty);
                versionInformation.Description = keyValuePairs["Description"];

                keyValuePairs = GetDictionaryFromTextChunk(responseParts[1]);

                string key = "CGMiner";
                if (keyValuePairs.ContainsKey(key))
                    versionInformation.MinerVersion = keyValuePairs[key];
                else
                {
                    // SGMiner 4.0 broke compatibility with the CGMiner RPC API
                    // Version 5.0 fixed the issue - this work-around is for 4.0
                    // Request :"version"
                    // Response:"STATUS=S,When=1415068731,Code=22,Msg=SGMiner versions,Description=sgminer 4.1.0|VERSION,SGMiner=4.1.0,API=3.1|\u0000"
                    key = "SGMiner";
                    if (keyValuePairs.ContainsKey(key))
                        versionInformation.MinerVersion = keyValuePairs[key];
                }

                versionInformation.ApiVersion = keyValuePairs["API"];
            }
        }
    }
}
