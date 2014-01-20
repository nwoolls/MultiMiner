using System;

namespace MultiMiner.Discovery
{
    public class InstanceChangedArgs : EventArgs
    {
        public Instance Instance { get; set; }
    }
}
