using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    public class DeviceInformationParser
    {
        public static void ParseTextForDeviceInformation(string text, List<DeviceInformation> deviceInformation)
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
                    DeviceInformation newDevice = new DeviceInformation();

                    newDevice.Kind = keyValuePairs.ElementAt(0).Key;
                    newDevice.Index = int.Parse(keyValuePairs[newDevice.Kind]);

                    if (keyValuePairs.ContainsKey("Enabled")) //seen this needed with a user
                        newDevice.Enabled = keyValuePairs["Enabled"].Equals("Y");

                    if (keyValuePairs.ContainsKey("Status")) //check required for bfgminer
                        newDevice.Status = keyValuePairs["Status"];

                    //the RPC API returns numbers formatted en-US, e.g. 1,000.00
                    //specify CultureInfo.InvariantCulture for parsing or unhandled exceptions will
                    //occur on other locales
                    //can test for this with:
                    //Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                    //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

                    if (newDevice.Kind.Equals("GPU"))
                    {
                        if (keyValuePairs.ContainsKey("Temperature")) //check required for bfgminer
                            newDevice.Temperature = double.Parse(keyValuePairs["Temperature"], CultureInfo.InvariantCulture);
                        newDevice.FanSpeed = int.Parse(keyValuePairs["Fan Speed"], CultureInfo.InvariantCulture);
                        newDevice.FanPercent = int.Parse(keyValuePairs["Fan Percent"], CultureInfo.InvariantCulture);
                        newDevice.GpuClock = int.Parse(keyValuePairs["GPU Clock"], CultureInfo.InvariantCulture);
                        newDevice.MemoryClock = int.Parse(keyValuePairs["Memory Clock"], CultureInfo.InvariantCulture);
                        newDevice.GpuVoltage = double.Parse(keyValuePairs["GPU Voltage"], CultureInfo.InvariantCulture);
                        newDevice.GpuActivity = int.Parse(keyValuePairs["GPU Activity"], CultureInfo.InvariantCulture);
                        newDevice.PowerTune = int.Parse(keyValuePairs["Powertune"], CultureInfo.InvariantCulture);
                        newDevice.Intensity = keyValuePairs["Intensity"];
                    }

                    if (keyValuePairs.ContainsKey("MHS av")) //check required for bfgminer
                        newDevice.AverageHashrate = double.Parse(keyValuePairs["MHS av"], CultureInfo.InvariantCulture) * 1000;

                    //seen both MHS 5s and MHS 1s
                    if (keyValuePairs.ContainsKey("MHS 5s"))
                        newDevice.CurrentHashrate = double.Parse(keyValuePairs["MHS 5s"], CultureInfo.InvariantCulture) * 1000;
                    else if (keyValuePairs.ContainsKey("MHS 1s"))
                        newDevice.CurrentHashrate = double.Parse(keyValuePairs["MHS 1s"], CultureInfo.InvariantCulture) * 1000;

                    if (keyValuePairs.ContainsKey("Accepted")) //check required for bfgminer
                    {
                        //personally seen this need extra handling with a user
                        string stringValue = keyValuePairs["Accepted"];
                        int intValue = 0;
                        if (int.TryParse(stringValue, out intValue))
                            newDevice.AcceptedShares = intValue;
                    }

                    if (keyValuePairs.ContainsKey("Rejected")) //check required for bfgminer
                    {
                        //personally seen this need extra handling with a user
                        string stringValue = keyValuePairs["Rejected"];
                        int intValue = 0;
                        if (int.TryParse(stringValue, out intValue))
                            newDevice.RejectedShares = intValue;
                    }

                    if (keyValuePairs.ContainsKey("Hardware Errors")) //check required for bfgminer
                        newDevice.HardwareErrors = int.Parse(keyValuePairs["Hardware Errors"], CultureInfo.InvariantCulture);

                    if (keyValuePairs.ContainsKey("Utility")) //check required for bfgminer
                    {
                        //personally seen this need extra handling with a user
                        string stringValue = keyValuePairs["Utility"];
                        double doubleValue = 0.00;
                        if (double.TryParse(stringValue, out doubleValue))
                            newDevice.Utility = doubleValue;
                    }

                    deviceInformation.Add(newDevice);
                }
            }
        }
    }
}
