using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace MultiMiner.Remoting.Broadcast
{
    public class Sender
    {
        public static void Send(IPAddress ipAddress, object payload)
        {
            //send from each local interface
            //reasoning: virtual network adapters may be the chosen interface otherwise
            List<string> localIPAddresses = Utility.Net.LocalNetwork.GetLocalIPAddresses();
            foreach (string localIPAddress in localIPAddresses)
                Send(IPAddress.Parse(localIPAddress), ipAddress, payload);
        }

        private static void Send(IPAddress source, IPAddress destination, object payload)
        {
            using (UdpClient client = new UdpClient(new IPEndPoint(source, 0)))
            {
                string jsonPayload = JsonConvert.SerializeObject(payload);

                Packet packet = new Packet();
                packet.Descriptor = payload.GetType().FullName;
                packet.Payload = jsonPayload;

                string jsonPacket = JsonConvert.SerializeObject(packet);

                byte[] bytes = Encoding.ASCII.GetBytes(jsonPacket);

                IPEndPoint ip = new IPEndPoint(destination, Config.BroadcastPort);
                try
                {
                    client.Send(bytes, bytes.Length, ip);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.HostUnreachable)
                        //reasoning: we broadcast on all interfaces
                        //on OS X this may result in No route to host
                        Console.WriteLine(String.Format("{0}: {1}", source, ex.Message));
                    else
                        throw;
                }
                finally
                {
                    client.Close();
                }
            }
        }
    }
}
