using System;

namespace MultiMiner.Xgminer.Data
{
    [Serializable]
    public class Device : DeviceDescriptor
    {
        public Device()
        {
            this.Platform = new DevicePlatform();
        }

        public string Name { get; set; }
        public DevicePlatform Platform { get; set; }
        public int ProcessorCount { get; set; }
    }
}
