using System;

namespace MultiMiner.Xgminer
{
    public class LaunchFailedArgs : EventArgs
    {
        public string Reason { get; set; }
        public string CoinName { get; set; }
    }
}
