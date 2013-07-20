using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MultiMiner.Xgminer.Parsers
{
    class DeviceListParser
    {
        private const string DevicesHeaderPattern = @"\[.+ .+\] Devices detected:";
        private const string DevicesFooterPattern = @"\[.+ .+\] \d+ devices listed";
        private const string DeviceFullPattern = @"\[.+ .+\].*\d+\. (.*) \d+ ?: (.*) \(driver: (.*)\)";
        private const string DeviceBriefPattern = @"\[.+ .+\].*\d+\. (.*) \d+  ?\(driver: (.*)\)";

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
                        Device newDevice = new Device();

                        newDevice.Identifier = match.Groups[1].Value.TrimEnd();
                        newDevice.Name = match.Groups[2].Value.TrimEnd();
                        newDevice.Driver = match.Groups[3].Value.TrimEnd();
                        newDevice.Kind = DeviceIsGpu(newDevice) ? DeviceKind.GPU : DeviceKind.USB;

                        devices.Add(newDevice);
                    }
                    else
                    {
                        match = Regex.Match(line, DeviceBriefPattern);
                        if (match.Success)
                        {
                            Device newDevice = new Device();

                            newDevice.Identifier = match.Groups[1].Value.TrimEnd();
                            newDevice.Driver = match.Groups[2].Value.TrimEnd();
                            newDevice.Kind = DeviceIsGpu(newDevice) ? DeviceKind.GPU : DeviceKind.USB;

                            devices.Add(newDevice);
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
