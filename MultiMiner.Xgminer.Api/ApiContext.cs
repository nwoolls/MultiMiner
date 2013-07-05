using MultiMiner.Xgminer.Api.Parsers;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MultiMiner.Xgminer.Api
{
    public class ApiContext
    {
        //events
        // delegate declaration 
        public delegate void LogEventHandler(object sender, LogEventArgs ca);

        // event declaration 
        public event LogEventHandler LogEvent;

        private readonly int port;
        public ApiContext(int port)
        {
            this.port = port;
        }

        public List<DeviceInformation> GetDeviceInformation()
        {
            string textResponse = GetResponse(ApiVerb.Devs);
            List<DeviceInformation> result = new List<DeviceInformation>();
            DeviceInformationParser.ParseTextForDeviceInformation(textResponse, result);
            return result;
        }

        public void QuitMining()
        {
            GetResponse(ApiVerb.Quit);
        }

        private string GetResponse(string apiVerb)
        {
            TcpClient tcpClient = new TcpClient("127.0.0.1", port);
            NetworkStream tcpStream = tcpClient.GetStream();

            Byte[] request = Encoding.ASCII.GetBytes(apiVerb);
            tcpStream.Write(request, 0, request.Length);

            Byte[] responseBuffer = new Byte[4096];
            string response = string.Empty;
            do
            {
                int bytesRead = tcpStream.Read(responseBuffer, 0, responseBuffer.Length);
                response = response + Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
            } while (tcpStream.DataAvailable);


            LogEventArgs args = new LogEventArgs();

            args.DateTime = DateTime.Now;
            args.Request = apiVerb;
            args.Response = response;

            if (LogEvent != null)
                LogEvent(this, args); 

            return response;
        } 
    }
}
