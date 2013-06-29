namespace MultiMiner.Xgminer
{
    public static class DevicePatterns
    {
        public const string PlatformVendor = @"\[.+ .+\] \w* Platform \d+ vendor: (.*)";
        public const string PlatformName = @"\[.+ .+\] \w* Platform \d+ name: (.*)";
        public const string PlatformVersion = @"\[.+ .+\] \w* Platform \d+ version: (.*)";
        public const string PlatformDevicesHeader = @"\[.+ .+\] Platform \d+ devices: \d+";
        public const string PlatformDevicesFooter = @"\[.+ .+\] \d+ \w+ devices max detected";
        public const string DeviceName = @"\[.+ .+\] \t\d+\t(.*)";
        public const string DeviceDescription = @"\[.+ .+\] \w* \d+ (.*)";
        public const string UsbDevicesHeader = @"\[.+ .+\] USB all: found .*";
        public const string UsbDevicesFooter = @"\[.+ .+\] \d+ known USB devices";
        public const string UsbManufacturer = @" Manufacturer: '(.*)'";
        public const string UsbProduct = @" Product: '(.*)'";
    }
}
