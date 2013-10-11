using MultiMiner.Xgminer.Api.Responses;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    public class DeviceInformationParser
    {
        public static void ParseTextForDeviceInformation(string text, List<DeviceInformationResponse> deviceInformation)
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
                    DeviceInformationResponse newDevice = new DeviceInformationResponse();

                    newDevice.Kind = keyValuePairs.ElementAt(0).Key;

                    newDevice.Index = TryToParseInt(keyValuePairs, newDevice.Kind, -1);
                    if (newDevice.Index == -1)
                        continue;
                    
                    if (keyValuePairs.ContainsKey("Enabled")) //seen this needed with a user
                        newDevice.Enabled = keyValuePairs["Enabled"].Equals("Y");

                    if (keyValuePairs.ContainsKey("Status")) //check required for bfgminer
                        newDevice.Status = keyValuePairs["Status"];

                    if (keyValuePairs.ContainsKey("Name"))
                        newDevice.Name = keyValuePairs["Name"];

                    if (newDevice.Kind.Equals("GPU"))
                    {
                        newDevice.Temperature = TryToParseDouble(keyValuePairs, "Temperature", 0.00);
                        newDevice.FanSpeed = TryToParseInt(keyValuePairs, "Fan Speed", 0);
                        newDevice.FanPercent = TryToParseInt(keyValuePairs, "Fan Percent", 0);
                        newDevice.GpuClock = TryToParseInt(keyValuePairs, "GPU Clock", 0);
                        newDevice.MemoryClock = TryToParseInt(keyValuePairs, "Memory Clock", 0);
                        newDevice.GpuVoltage = TryToParseDouble(keyValuePairs, "GPU Voltage", 0.00);
                        newDevice.GpuActivity = TryToParseInt(keyValuePairs, "GPU Activity", 0);
                        newDevice.PowerTune = TryToParseInt(keyValuePairs, "Powertune", 0);
                        newDevice.Intensity = keyValuePairs["Intensity"];
                    }

                    newDevice.AverageHashrate = TryToParseDouble(keyValuePairs, "MHS av", 0.00) * 1000;

                    //seen both MHS 5s and MHS 1s
                    if (keyValuePairs.ContainsKey("MHS 5s"))
                        newDevice.CurrentHashrate = TryToParseDouble(keyValuePairs, "MHS 5s", 0.00) * 1000;
                    else if (keyValuePairs.ContainsKey("MHS 1s"))
                        newDevice.CurrentHashrate = TryToParseDouble(keyValuePairs, "MHS 1s", 0.00) * 1000;
                    
                    newDevice.AcceptedShares = TryToParseInt(keyValuePairs, "Accepted", 0);                    
                    newDevice.RejectedShares = TryToParseInt(keyValuePairs, "Rejected", 0);                    
                    newDevice.HardwareErrors = TryToParseInt(keyValuePairs, "Hardware Errors", 0);                    
                    newDevice.Utility = TryToParseDouble(keyValuePairs, "Utility", 0.00);                    
                    newDevice.PoolIndex = TryToParseInt(keyValuePairs, "Last Share Pool", -1);
                    
                    deviceInformation.Add(newDevice);
                }
            }
        }

        //the RPC API returns numbers formatted en-US, e.g. 1,000.00
        //specify CultureInfo.InvariantCulture for parsing or unhandled exceptions will
        //occur on other locales
        //can test for this with:
        //Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
        //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
        private static int TryToParseInt(Dictionary<string, string> keyValuePairs, string key, int defaultValue)
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

        private static double TryToParseDouble(Dictionary<string, string> keyValuePairs, string key, double defaultValue)
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
    }
}
