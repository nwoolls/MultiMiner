using System.Net;

namespace MultiMiner.Remoting.Server.Broadcast
{
    public class Broadcaster
    {
        public static void Broadcast(object payload)
        {
            Sender.Send(IPAddress.Broadcast, payload);
        }
    }
}
