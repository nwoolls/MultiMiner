namespace MultiMiner.Xgminer
{
    public class Device : DeviceDescriptor
    {
        public Device()
        {
            this.Platform = new DevicePlatform();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public DevicePlatform Platform { get; set; }
        public int DeviceIndex { get; set; }
        public int ProcessorCount { get; set; }
    }
}
