using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using System.Diagnostics;
using System.Net.Sockets;

namespace MultiMiner.Engine
{
    public class MinerProcess
    {
        public Process Process { get; set; }
        public int ApiPort { get; set; }
        public MinerConfiguration MinerConfiguration { get; set; }

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

        private ApiContext GetApiContext()
        {
            ApiContext result = null;
            try
            {
                result = new ApiContext(ApiPort);
            }
            catch (SocketException ex)
            {
                //won't be able to connect for the first 5s or so
                result = null;
            }
            return result;
        }
    }
}
