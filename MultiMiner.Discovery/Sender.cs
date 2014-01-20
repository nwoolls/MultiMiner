using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiMiner.Discovery
{
    class Sender
    {
        public static void Send(IPAddress ipAddress)
        {
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint ip = new IPEndPoint(ipAddress, Keys.Port);
                byte[] bytes = Encoding.ASCII.GetBytes(Keys.Identifier);
                client.Send(bytes, bytes.Length, ip);
                client.Close();
            }
        }
    }
}
