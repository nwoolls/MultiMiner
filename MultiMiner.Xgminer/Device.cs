using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Xgminer
{
    public class Device
    {
        public Device()
        {
            this.Platform = new DevicePlatform();
        }

        public DeviceKind Kind { get; set; }
        public DevicePlatform Platform { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
