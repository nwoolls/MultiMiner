using System;

namespace MultiMiner.Remoting.Server.Broadcast
{
    public class PacketReceivedArgs : EventArgs
    {
        public string IpAddress { get; set; }
        public Packet Packet { get; set; }
    }
}
