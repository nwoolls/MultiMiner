using System;

namespace MultiMiner.Discovery
{
    public class InstanceChangedArgs : EventArgs
    {
        public Data.Instance Instance { get; set; }
    }
}
