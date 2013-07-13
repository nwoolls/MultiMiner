using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace MultiMiner.Engine
{
    public class MinerProcess
    {
        public Process Process { get; set; }
        public int ApiPort { get; set; }
        public MinerConfiguration MinerConfiguration { get; set; } //for relaunching crashed miners

        private ApiContext apiContext;
        public ApiContext ApiContext 
        { 
            get 
            {
                if (apiContext == null)
                    apiContext = GetApiContext();
                return apiContext;
            } 
        }

        public void StopMining()
        {
            if (!Process.HasExited)
            {
                try
                {
                    ApiContext.QuitMining();
                    Thread.Sleep(250);
                }
                catch (SocketException ex)
                {
                    //won't be able to connect for the first 5s or so
                }

                Process.Kill();
            }
        }

        private ApiContext GetApiContext()
        {
            return new ApiContext(ApiPort);
        }
    }
}
