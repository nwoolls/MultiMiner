using System;
using System.Collections.Generic;

namespace MultiMiner.Engine
{
    public class LogProcessCloseArgs : EventArgs
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CoinName { get; set; }
        public string CoinSymbol { get; set; }
        public double StartPrice { get; set; }
        public double EndPrice { get; set; }
        public int AcceptedShares { get; set; }
        public List<int> DeviceIndexes { get; set; }
    }
}
