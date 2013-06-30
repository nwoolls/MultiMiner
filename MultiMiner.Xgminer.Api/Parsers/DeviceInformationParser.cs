using System.Collections.Generic;
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

                var deviceAttributes = deviceText.Split(',');

                Dictionary<string, string> keyValuePairs = deviceAttributes
                  .Where(value => value.Contains('='))
                  .Select(value => value.Split('='))
                  .ToDictionary(pair => pair[0], pair => pair[1]);

                DeviceInformation newDevice = new DeviceInformation();

                newDevice.Kind = keyValuePairs.ElementAt(0).Key;
                newDevice.Index = int.Parse(keyValuePairs[newDevice.Kind]);
                newDevice.Enabled = keyValuePairs["Enabled"].Equals("Y");
                newDevice.Status = keyValuePairs["Status"];

                if (newDevice.Kind.Equals("GPU"))
                {
                    newDevice.Temperature = double.Parse(keyValuePairs["Temperature"]);
                    newDevice.FanSpeed = int.Parse(keyValuePairs["Fan Speed"]);
                    newDevice.FanPercent = int.Parse(keyValuePairs["Fan Percent"]);
                    newDevice.GpuClock = int.Parse(keyValuePairs["GPU Clock"]);
                    newDevice.MemoryClock = int.Parse(keyValuePairs["Memory Clock"]);
                    newDevice.GpuVoltage = double.Parse(keyValuePairs["GPU Voltage"]);
                    newDevice.GpuActivity = int.Parse(keyValuePairs["GPU Activity"]);
                    newDevice.PowerTune = int.Parse(keyValuePairs["Powertune"]);
                    newDevice.Intensity = int.Parse(keyValuePairs["Intensity"]);
                }

                newDevice.AverageHashrate = double.Parse(keyValuePairs["MHS av"]) * 1000;
                newDevice.CurrentHashrate = double.Parse(keyValuePairs["MHS 5s"]) * 1000;

                newDevice.AcceptedShares = int.Parse(keyValuePairs["Accepted"]);
                newDevice.RejectedShares = int.Parse(keyValuePairs["Rejected"]);
                newDevice.HardwareErrors = int.Parse(keyValuePairs["Hardware Errors"]);
                newDevice.Utility = double.Parse(keyValuePairs["Utility"]);

                deviceInformation.Add(newDevice);
            }
        }
    }
}
