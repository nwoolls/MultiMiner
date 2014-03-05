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
    }
}
