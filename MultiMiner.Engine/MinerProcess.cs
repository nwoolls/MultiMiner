using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using System;
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
        public DateTime StartDate { get; set; }
        public Coinchoose.Api.CoinInformation CoinInformation { get; set; }

        public bool HasSickDevice { get; set; }
        public bool HasDeadDevice { get; set; }
        public bool HasZeroHashrateDevice { get; set; }
        public bool HasFrozenDevice { get; set; }
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

                KillProcess(Process);
            }
        }

        public static void KillProcess(Process process)
        {
            //do NOT call process.CloseMainWindow();
            //can leave zombie cgminer processes
            try
            {
                process.Kill();
                process.WaitForExit();
                process.Close();
            }
            catch (System.InvalidOperationException ex)
            {
                //already closed
            }
        }

        private ApiContext GetApiContext()
        {
            return new ApiContext(ApiPort);
        }
    }
}
