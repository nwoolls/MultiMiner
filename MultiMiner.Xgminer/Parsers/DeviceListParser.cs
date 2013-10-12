using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MultiMiner.Xgminer.Parsers
{
    public class DeviceListParser
    {
        private const string DevicesHeaderPattern = @"\[.+ .+\] Devices detected:";
        private const string DevicesFooterPattern = @"\[.+ .+\] \d+ devices listed";
        private const string DeviceFullPattern = @"\[.+ .+\].*\d+\. (.*) ?: (.*) \(driver: (.*)\)";
        private const string DeviceBriefPattern = @"\[.+ .+\].*\d+\. (.*)  ?\(driver: (.*)\)";

        public static void ParseTextForDevices(List<string> text, List<Device> devices)
        {
            bool inDevices = false;
            
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

                        device.Identifier = match.Groups[1].Value.TrimEnd().Substring(0, 3);
                        device.Name = match.Groups[2].Value.TrimEnd();
                        device.Driver = match.Groups[3].Value.TrimEnd();
                        device.Kind = DeviceIsGpu(device) ? DeviceKind.GPU : DeviceKind.USB;
                        device.DeviceIndex = devices.Count;

                        devices.Add(device);
                    }
                    else
                    {
                        match = Regex.Match(line, DeviceBriefPattern);
                        if (match.Success)
                        {
                            Device device = new Device();

                            device.Identifier = match.Groups[1].Value.TrimEnd().Substring(0, 3);
                            device.Driver = match.Groups[2].Value.TrimEnd();
                            device.Kind = DeviceIsGpu(device) ? DeviceKind.GPU : DeviceKind.USB;
                            device.DeviceIndex = devices.Count;

                            devices.Add(device);
                        }
                    }
                }

                if (Regex.Match(line, DevicesHeaderPattern).Success)
                {
                    inDevices = true;
                }
            }
        }

        private static bool DeviceIsGpu(Device device)
        {
            return device.Identifier.Equals("GPU") || device.Identifier.Equals("OCL");
        }
    }
}
