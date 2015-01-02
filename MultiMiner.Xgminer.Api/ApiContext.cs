using MultiMiner.Xgminer.Api.Parsers;
using MultiMiner.Xgminer.Api.Data;
using MultiMiner.Xgminer.Api.Extensions;
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
        public delegate void LogEventHandler(object sender, LogEventArgs ea);

        // event declaration 
        public event LogEventHandler LogEvent;
        
        public int Port { get; set; }
        public string IpAddress { get; set; }

        private const int ShortCommandTimeoutMs = 500;
        public const int LongCommandTimeoutMs = 5000;
        private const int ConnectTimeoutMS = 1000;

        public ApiContext(int port, string ipAddress = "127.0.0.1")
        {
            this.Port = port;
            this.IpAddress = ipAddress;
        }

        public List<DeviceInformation> GetDeviceInformation(int timeoutMs = ShortCommandTimeoutMs)
        {
            string textResponse = GetResponse(ApiVerb.Devs, timeoutMs);
            List<DeviceInformation> result = new List<DeviceInformation>();
            DeviceInformationParser.ParseTextForDeviceInformation(textResponse, result);
            return result;
        }

        public List<DeviceDetails> GetDeviceDetails()
        {
            string textResponse = GetResponse(ApiVerb.DevDetails);
            List<DeviceDetails> result = new List<DeviceDetails>();
            DeviceDetailsParser.ParseTextForDeviceDetails(textResponse, result);
            return result;
        }

        public SummaryInformation GetSummaryInformation()
        {
            string textResponse = GetResponse(ApiVerb.Summary);
            SummaryInformation result = new SummaryInformation();
            SummaryInformationParser.ParseTextForSummaryInformation(textResponse, result);
            return result;
        }

        public List<PoolInformation> GetPoolInformation()
        {
            string textResponse = GetResponse(ApiVerb.Pools);
            List<PoolInformation> result = new List<PoolInformation>();
            PoolInformationParser.ParseTextForDeviceDetails(textResponse, result);
            return result;
        }

        public VersionInformation GetVersionInformation()
        {
            string textResponse = GetResponse(ApiVerb.Version);
            VersionInformation result = new VersionInformation();
            VersionInformationParser.ParseTextForVersionInformation(textResponse, result);
            return result;
        }

        public NetworkCoinInformation GetCoinInformation()
        {
            string textResponse = GetResponse(ApiVerb.Coin);
            NetworkCoinInformation result = new NetworkCoinInformation();
            NetworkCoinInformationParser.ParseTextForCoinNetworkInformation(textResponse, result);
            return result;
        }

        public List<MinerStatistics> GetMinerStatistics()
        {
            //KnC titan returns OVER 9,000 results - give it more time
            string textResponse = GetResponse(ApiVerb.Stats, LongCommandTimeoutMs);
            List<MinerStatistics> result = new List<MinerStatistics>();
            MinerStatisticsParser.ParseTextForMinerStatistics(textResponse, result);
            return result;
        }

        public string QuitMining()
        {
            return GetResponse(ApiVerb.Quit);
        }

        public string RestartMining()
        {
            return GetResponse(ApiVerb.Restart);
        }

        public string SwitchPool(int poolIndex)
        {
            return GetResponse(String.Format("{0}|{1}", ApiVerb.SwitchPool, poolIndex));
        }

        public string GetResponse(string apiVerb, int timeoutMs = ShortCommandTimeoutMs)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(this.IpAddress, Port, ConnectTimeoutMS);

            NetworkStream tcpStream = tcpClient.GetStream();

            // set a read timeout, otherwise it is infinite and could lock the app
            // if a miner is locked up
            tcpStream.ReadTimeout = 5000; // ms

            Byte[] request = Encoding.ASCII.GetBytes(apiVerb);

            long timeoutTicks = timeoutMs * TimeSpan.TicksPerMillisecond;
            long ticks = DateTime.Now.Ticks;

            tcpStream.Write(request, 0, request.Length);

            Byte[] responseBuffer = new Byte[4096];
            string response = string.Empty;
            do
            {
                int bytesRead = tcpStream.Read(responseBuffer, 0, responseBuffer.Length);
                response = response + Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
            } while (
                //check timeout
                ((DateTime.Now.Ticks - ticks) <= timeoutTicks) &&
                //looking for a terminating NULL character from the RPC API
                (String.IsNullOrEmpty(response) ||
                                     (response[response.Length - 1] != '\0')));
            
            if (LogEvent != null)
            {
                LogEventArgs args = new LogEventArgs();

                args.DateTime = DateTime.Now;
                args.Request = apiVerb;
                args.Response = response;

                LogEvent(this, args);
            }

            tcpClient.Close();

            return response;
        } 
    }
}
