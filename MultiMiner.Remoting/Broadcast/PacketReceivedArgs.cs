using System;

namespace MultiMiner.Remoting.Broadcast
{
    public class PacketReceivedArgs : EventArgs
    {
        public string IpAddress { get; set; }
        public Packet Packet { get; set; }
    }
}
