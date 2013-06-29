using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultiMiner.Xgminer
{
    public static class DeviceParser
    {
        public static void ParseOutputForDevices(List<string> output, List<Device> result)
        {
            ParseOutputForDevicePlatforms(output, result);
        }

        private static void ParseOutputForDevicePlatforms(List<string> output, List<Device> result)
        {
            bool inPlatform = false;

            string currentPlatformVendor = string.Empty;
            string currentPlatformName = string.Empty;
            string currentPlatformVersion = string.Empty;
            string currentDeviceName = string.Empty;
            string currentDeviceDescription = string.Empty;

            const string platformVendorPattern = @"\[.+ .+\] \w* Platform \d+ vendor: (.*)";
            const string platformNamePattern = @"\[.+ .+\] \w* Platform \d+ name: (.*)";
            const string platformVersionPattern = @"\[.+ .+\] \w* Platform \d+ version: (.*)";

            const string platformDevicesHeaderPattern = @"\[.+ .+\] Platform \d+ devices: \d+";
            const string platformDevicesFooterPattern = @"\[.+ .+\] \d+ \w+ devices max detected";

            const string deviceNamePattern = @"\[.+ .+\] \t\d+\t(.*)";
            const string deviceDescriptionPattern = @"\[.+ .+\] \w* \d+ (.*)";

            List<string> names = new List<string>();
            List<string> descriptions = new List<string>();

            foreach (string line in output)
            {
                Match match = Regex.Match(line, platformVendorPattern);
                if (match.Success)
                    currentPlatformVendor = match.Groups[1].Value.TrimEnd();

                match = Regex.Match(line, platformNamePattern);
                if (match.Success)
                    currentPlatformName = match.Groups[1].Value.TrimEnd();

                match = Regex.Match(line, platformVersionPattern);
                if (match.Success)
                    currentPlatformVersion = match.Groups[1].Value.TrimEnd();

                if (Regex.Match(line, platformDevicesFooterPattern).Success)
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        Device device = new Device();
                        device.Platform.Name = currentPlatformName;
                        device.Platform.Vendor = currentPlatformVendor;
                        device.Platform.Version = currentPlatformVersion;
                        device.Name = names[i];
                        device.Description = descriptions[i];
                        device.Kind = DeviceKind.GPU;

                        result.Add(device);
                    }

                    inPlatform = false;
                    names.Clear();
                    descriptions.Clear();
                }

                if (inPlatform)
                {
                    match = Regex.Match(line, deviceNamePattern);
                    if (match.Success)
                    {
                        currentDeviceName = match.Groups[1].Value.TrimEnd();
                        names.Add(currentDeviceName);
                    }


                    match = Regex.Match(line, deviceDescriptionPattern);
                    if (match.Success)
                    {
                        currentDeviceDescription = match.Groups[1].Value.TrimEnd();
                        descriptions.Add(currentDeviceDescription);
                    }

                }

                if (Regex.Match(line, platformDevicesHeaderPattern).Success)
                {
                    inPlatform = true;
                    names.Clear();
                    descriptions.Clear();
                }

                // do something with line
            }
        }
    }
}
