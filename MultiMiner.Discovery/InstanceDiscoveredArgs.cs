using System;

namespace MultiMiner.Discovery
{
    public class InstanceDiscoveredArgs : EventArgs
    {
        public string IpAddress { get; set; }
    }
}
