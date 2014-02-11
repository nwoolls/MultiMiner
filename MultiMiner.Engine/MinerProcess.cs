using MultiMiner.Xgminer;
using MultiMiner.Xgminer.Api;
using System;
using System.ComponentModel;
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
        public Coin.Api.Data.CoinInformation CoinInformation { get; set; }
        //store separately from CoinInformation as CoinInformation depends on Coin API
        public string CoinSymbol { get; set; }

        public bool HasSickDevice { get; set; }
        public bool HasDeadDevice { get; set; }
        public bool HasZeroHashrateDevice { get; set; }
        public bool MinerIsFrozen { get; set; }
        public bool HasPoorPerformingDevice { get; set; }
        public long FoundBlocks { get; set; }
        public long AcceptedShares { get; set; }
        //set TerminateProcess to True to skip using the QUIT RPC API command
        public bool TerminateProcess { get; set; }

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
                if (!TerminateProcess)
                {
                    try
                    {
                        ApiContext.QuitMining();

                        //try to give the miner time to close
                        //killing it could leave (Win)USB devices tied up
                        int count = 0;
                        const int max = 10;
                        while (!Process.HasExited && (count < max))
                        {
                            Thread.Sleep(500);
                            count++;
                        }
                    }
                    catch (SocketException ex)
                    {
                        //won't be able to connect for the first 5s or so
                    }
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
                //process.WaitForExit(); causes potential lockups closing bfgminer
                process.Close();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is Win32Exception)
                {
                    //already closed
                    return;
                }
                throw;
            }
        }

        private ApiContext GetApiContext()
        {
            return new ApiContext(ApiPort);
        }
    }
}
