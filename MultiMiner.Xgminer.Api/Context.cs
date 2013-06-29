using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Xgminer.Api
{
    public class Context
    {
        public string Config()
        {
            string apiVerb = Verb.Config;

            string response = GetResponse(apiVerb);

            return response;
        }

        public string Devs()
        {
            string apiVerb = Verb.Devs;

            string response = GetResponse(apiVerb);

            return response;
        }

        private static string GetResponse(string apiVerb)
        {
            TcpClient tcpClient = new TcpClient("127.0.0.1", 4028);
            NetworkStream stream = tcpClient.GetStream();
            Byte[] request = System.Text.Encoding.ASCII.GetBytes(apiVerb);
            stream.Write(request, 0, request.Length);

            Byte[] responseBuffer = new Byte[1024];
            string response = string.Empty;
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            response = System.Text.Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
            return response;
        } 
    }
}
