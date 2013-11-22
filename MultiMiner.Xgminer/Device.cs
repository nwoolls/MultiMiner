namespace MultiMiner.Xgminer
{
    public class Device
    {
        public Device()
        {
            this.Platform = new DevicePlatform();
        }

        public DeviceKind Kind { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DevicePlatform Platform { get; set; }
        public string Driver { get; set; }
        public int DeviceIndex { get; set; }
        public int RelativeIndex { get; set; }
        public int ProcessorCount { get; set; }

        //USB devices have a Serial and Path
        public string Serial { get; set; }
        public string Path { get; set; }
    }
}
