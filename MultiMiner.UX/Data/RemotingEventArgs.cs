using System;

namespace MultiMiner.UX.Data
{
    public class RemotingEventArgs : EventArgs
    {
        public string IpAddress;
        public Remoting.Data.Transfer.Machine Machine;
    }
}
