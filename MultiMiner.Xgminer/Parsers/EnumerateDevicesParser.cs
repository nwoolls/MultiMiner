using MultiMiner.Xgminer.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MultiMiner.Xgminer.Parsers
{
    public static class EnumerateDevicesParser
    {
        private const string PlatformVendorPattern = @"\[.+ .+\] \w* Platform \d+ vendor: (.*)";
        private const string PlatformNamePattern = @"\[.+ .+\] \w* Platform \d+ name: (.*)";
        private const string PlatformVersionPattern = @"\[.+ .+\] \w* Platform \d+ version: (.*)";
        private const string PlatformDevicesHeaderPattern = @"\[.+ .+\] Platform \d+ devices: \d+";
        private const string PlatformDevicesFooterPattern = @"\[.+ .+\] \d+ \w+ devices max detected";
        private const string DeviceNamePattern = @"\[.+ .+\] \t\d+\t(.*)";
        private const string DeviceDescriptionPattern = @"\[.+ .+\] \w* \d+ (.*)";
        private const string UsbDevicesHeaderPattern = @"\[.+ .+\] USB all: found .*";
        private const string UsbDevicesFooterPattern = @"\[.+ .+\] \d+ known USB devices";
        private const string UsbManufacturerPattern = @" Manufacturer: '(.*)'";
        private const string UsbProductPattern = @" Product: '(.*)'";

        public static void ParseTextForDevices(List<string> text, List<Device> devices)
        {
            ParseTextForGpuDevices(text, devices);
            ParseTextForUsbDevices(text, devices);
        }

        private static void ParseTextForUsbDevices(List<string> text, List<Device> devices)
        {
            bool inUsbList = false;

            string currentUsbManufacturer = string.Empty;
            string currentUsbProduct = string.Empty;

            foreach (string line in text)
            {
                if (Regex.Match(line, UsbDevicesFooterPattern).Success)
                {
                    inUsbList = false;
                }

                if (inUsbList)
                {
                    Match match = Regex.Match(line, UsbManufacturerPattern);
                    if (match.Success)
                        currentUsbManufacturer = match.Groups[1].Value.TrimEnd();

                    match = Regex.Match(line, UsbProductPattern);
                    if (match.Success)
                    {
                        currentUsbProduct = match.Groups[1].Value.TrimEnd();

                        Device device = new Device();
                        device.Platform.Name = string.Empty;
                        device.Platform.Vendor = currentUsbManufacturer;
                        device.Platform.Version = string.Empty;
                        device.Name = currentUsbProduct;
                        device.Kind = DeviceKind.USB;

                        devices.Add(device);
                    }
                }

                if (Regex.Match(line, UsbDevicesHeaderPattern).Success)
                {
                    inUsbList = true;
                }
            }
        }

        private static void ParseTextForGpuDevices(List<string> text, List<Device> devices)
        {
            bool inPlatform = false;

            string currentPlatformVendor = string.Empty;
            string currentPlatformName = string.Empty;
            string currentPlatformVersion = string.Empty;
            string currentDeviceName = string.Empty;
            string currentDeviceDescription = string.Empty;

            List<string> names = new List<string>();
            List<string> descriptions = new List<string>();

            foreach (string line in text)
            {
                Match match = Regex.Match(line, PlatformVendorPattern);
                if (match.Success)
                    currentPlatformVendor = match.Groups[1].Value.TrimEnd();

                match = Regex.Match(line, PlatformNamePattern);
                if (match.Success)
                    currentPlatformName = match.Groups[1].Value.TrimEnd();

                match = Regex.Match(line, PlatformVersionPattern);
                if (match.Success)
                    currentPlatformVersion = match.Groups[1].Value.TrimEnd();
                
                if (Regex.Match(line, PlatformDevicesFooterPattern).Success)
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        Device device = new Device();
                        device.Platform.Name = currentPlatformName;
                        device.Platform.Vendor = currentPlatformVendor;
                        device.Platform.Version = currentPlatformVersion;

                        device.Name = names[i];

                        device.Kind = DeviceKind.GPU;

                        devices.Add(device);
                    }

                    inPlatform = false;
                    names.Clear();
                    descriptions.Clear();
                }

                if (inPlatform)
                {
                    match = Regex.Match(line, DeviceNamePattern);
                    if (match.Success)
                    {
                        currentDeviceName = match.Groups[1].Value.TrimEnd();
                        names.Add(currentDeviceName);
                    }

                    match = Regex.Match(line, DeviceDescriptionPattern);
                    if (match.Success)
                    {
                        currentDeviceDescription = match.Groups[1].Value.TrimEnd();
                        descriptions.Add(currentDeviceDescription);
                    }

                }

                if (Regex.Match(line, PlatformDevicesHeaderPattern).Success)
                {
                    inPlatform = true;
                    names.Clear();
                    descriptions.Clear();
                }
            }
        }
    }
}
