using MultiMiner.Xgminer.Api.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiMiner.Xgminer.Api.Parsers
{
    class DeviceDetailsParser : ResponseTextParser
    {
        public static void ParseTextForDeviceDetails(string text, List<DeviceDetailsResponse> deviceDetails)
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
                    DeviceDetailsResponse newDevice = new DeviceDetailsResponse();

                    newDevice.Index = int.Parse(keyValuePairs["DEVDETAILS"]);
                    newDevice.Name = keyValuePairs["Name"];
                    newDevice.ID = int.Parse(keyValuePairs["ID"]);
                    newDevice.Driver = keyValuePairs["Driver"];
                    if (keyValuePairs.ContainsKey("Device Path"))
                        newDevice.DevicePath = keyValuePairs["Device Path"];
                    if (keyValuePairs.ContainsKey("Kernel"))
                        newDevice.Kernel = keyValuePairs["Kernel"];
                    if (keyValuePairs.ContainsKey("Model"))
                        newDevice.Model = keyValuePairs["Model"];

                    deviceDetails.Add(newDevice);
                }
            }
        }
    }
}
