using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using System.Diagnostics;

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
            return new ApiContext(ApiPort);
        }
    }
}
