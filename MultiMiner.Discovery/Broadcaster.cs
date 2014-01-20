using System.Net;

namespace MultiMiner.Discovery
{
    public class Broadcaster
    {
        public static void Broadcast()
        {
            Sender.Send(IPAddress.Broadcast);
        }
    }
}
