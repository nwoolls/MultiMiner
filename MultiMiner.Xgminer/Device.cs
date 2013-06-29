using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Xgminer
{
    public class Device
    {
        public DeviceKind Kind { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
    }
}
