using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using MultiMiner.Xgminer.Data;

namespace MultiMiner.Xgminer.Parsers
{
    public class DeviceListParser
    {
        private const string DevicesHeaderPattern = @"\[.+ .+\] Devices detected:";
        private const string DevicesFooterPattern = @"\[.+ .+\] \d+ devices listed";
        private const string DeviceFullPattern = @"\[.+ .+\] (.*) \((.*)\)";

        public static void ParseTextForDevices(List<string> text, List<Device> devices)
        {
            bool inDevices = false;
            int relativeIndex = 0;
            
            foreach (string line in text)
            {
                if (Regex.Match(line, DevicesFooterPattern).Success)
                {
                    inDevices = false;
                }

                if (inDevices)
                {
                    Match match = Regex.Match(line, DeviceFullPattern);
                    if (match.Success)
                    {
                        Device device = new Device();

                        device.Name = match.Groups[1].Value.Trim();

                        string value = match.Groups[2].Value.TrimEnd();
                        Dictionary<string, string> details =
                            value.Split(';')
                                .Select(x => x.Split('='))
                                .ToDictionary(y => y[0].Trim(), y => y[1].Trim());

                        device.Driver = details["driver"];
                        device.ProcessorCount = int.Parse(details["procs"]);
                        if (details.ContainsKey("path"))
                            device.Path = details["path"];
                        if (details.ContainsKey("serial"))
                            device.Serial = details["serial"];

                        device.Kind = device.Driver.Equals("opencl") ? DeviceKind.GPU :
                            device.Driver.Equals("cpu") ? DeviceKind.CPU : DeviceKind.USB;

                        if ((devices.Count > 0) && (devices.Last()).Kind != device.Kind)
                            //relativeIndex is relative to device Kind
                            relativeIndex = 0;

                        device.RelativeIndex = relativeIndex;

                        devices.Add(device);

                        relativeIndex++;
                    }
                }

                if (Regex.Match(line, DevicesHeaderPattern).Success)
                {
                    inDevices = true;
                }
            }
        }
    }
}
