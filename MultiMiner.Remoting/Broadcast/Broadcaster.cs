using System.Net;

namespace MultiMiner.Remoting.Broadcast
{
    public class Broadcaster
    {
        public static void Broadcast(object payload)
        {
            Sender.Send(IPAddress.Broadcast, payload);
        }
    }
}
