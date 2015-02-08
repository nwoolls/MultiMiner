using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultiMiner.Discovery
{
    class Sender
    {
        public static void Send(IPAddress ipAddress, string verb, int fingerprint)
        {
            //send from each local interface
            //reasoning: virtual network adapters may be the chosen interface otherwise
            List<string> localIPAddresses = Utility.Net.LocalNetwork.GetLocalIPAddresses();
            foreach (string localIPAddress in localIPAddresses)
                Send(IPAddress.Parse(localIPAddress), ipAddress, verb, fingerprint);
        }

        private static void Send(IPAddress source, IPAddress destination, string verb, int fingerprint)
        {
            using (UdpClient client = new UdpClient(new IPEndPoint(source, 0)))
            {
                Data.Packet packet = new Data.Packet();
                packet.MachineName = Environment.MachineName;
                packet.Fingerprint = fingerprint;
                packet.Verb = verb;

                string jsonData = JsonConvert.SerializeObject(packet);
                byte[] bytes = Encoding.ASCII.GetBytes(jsonData);

                IPEndPoint ip = new IPEndPoint(destination, Config.Port);
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
