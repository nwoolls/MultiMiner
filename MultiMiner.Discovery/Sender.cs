using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiMiner.Discovery
{
    class Sender
    {
        public static void Send(IPAddress ipAddress, string verb)
        {
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint ip = new IPEndPoint(ipAddress, Config.Port);
                byte[] bytes = Encoding.ASCII.GetBytes(verb);
                client.Send(bytes, bytes.Length, ip);
                client.Close();
            }
        }
    }
}
