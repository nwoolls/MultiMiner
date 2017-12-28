using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    public class ResponseTextParser
    {
        //the RPC API returns numbers formatted en-US, e.g. 1,000.00
        //specify CultureInfo.InvariantCulture for parsing or unhandled exceptions will
        //occur on other locales
        //can test for this with:
        //Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
        //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
        protected static int TryToParseInt(Dictionary<string, string> keyValuePairs, string key, int defaultValue)
        {
            int result = defaultValue;

            if (keyValuePairs.ContainsKey(key))
            {
                string stringValue = keyValuePairs[key];
                int intValue;
                if (int.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out intValue))
                    result = intValue;
            }

            return result;
        }

        protected static double TryToParseDouble(Dictionary<string, string> keyValuePairs, string key, double defaultValue)
        {
            double result = defaultValue;

            if (keyValuePairs.ContainsKey(key))
            {
                string stringValue = keyValuePairs[key];
                double doubleValue;
                if (double.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out doubleValue))
                    result = doubleValue;
            }

            return result;
        }

        protected static Dictionary<string, string> GetDictionaryFromTextChunk(string textChunk)
        {
            IEnumerable<string> deviceAttributes = textChunk.Split(',').ToList().Distinct();
            Dictionary<string, string> keyValuePairs = deviceAttributes
              .Where(value => value.Contains('='))
              .Select(value => value.Split('='))
              .ToDictionary(pair => pair[0], pair => pair[1]);
            return keyValuePairs;
        }

        protected static Dictionary<string, string> ParseResponsePart(string responsePart)
        {
            if (responsePart == "\0")
                return new Dictionary<string,string>();

            IEnumerable<string> partAttributes = responsePart.Split(',');

            Dictionary<string, string> keyValuePairs = partAttributes
              .Where(value => value.Contains('='))
              .Select(value => value.Split('='))
              //remove dupe keys (with diff values) using GroupBy(), seen dupes with user stack traces
              .GroupBy(pair => pair[0])
              .ToDictionary(grp => grp.Key, grp => grp.First()[1]);

            return keyValuePairs;
        }

        protected static List<string> ParseResponseText(string text)
        {
            List<string> responseParts = text.Split('|').ToList();
            responseParts.RemoveAt(0);
            return responseParts;
        }
    }
}
