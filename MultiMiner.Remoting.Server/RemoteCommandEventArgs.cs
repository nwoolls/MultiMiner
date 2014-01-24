using System;
using System.Collections.Generic;

namespace MultiMiner.Remoting.Server
{
    public class RemoteCommandEventArgs : EventArgs
    {
        public string IpAddress { get; set; }
        public string Signature { get; set; }
    }
}
