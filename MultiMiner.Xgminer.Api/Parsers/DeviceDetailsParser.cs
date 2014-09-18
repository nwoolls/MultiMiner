using MultiMiner.Xgminer.Api.Data;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Xgminer.Api.Parsers
{
    class DeviceDetailsParser : ResponseTextParser
    {
        public static void ParseTextForDeviceDetails(string text, List<DeviceDetails> deviceDetails)
        {
            List<string> responseParts = ParseResponseText(text);
            if (responseParts.Count == 0)
                return;

            foreach (string responsePart in responseParts)
            {
                Dictionary<string, string> keyValuePairs = ParseResponsePart(responsePart);

                //check for key-value pairs, seen Count == 0 with user API logs
                if (keyValuePairs.Count > 0)
                {
                    DeviceDetails newDevice = new DeviceDetails();

                    newDevice.Index = int.Parse(keyValuePairs["DEVDETAILS"]);
                    newDevice.Name = keyValuePairs["Name"];
                    newDevice.ID = int.Parse(keyValuePairs["ID"]);
                    newDevice.Driver = keyValuePairs["Driver"];
                    if (keyValuePairs.ContainsKey("Device Path"))
                        newDevice.DevicePath = keyValuePairs["Device Path"];
                    if (keyValuePairs.ContainsKey("Serial"))
                        newDevice.Serial = keyValuePairs["Serial"];
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
