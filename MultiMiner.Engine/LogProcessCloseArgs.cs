using MultiMiner.Xgminer.Data;
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
        public long AcceptedShares { get; set; }
        public List<DeviceDescriptor> DeviceDescriptors { get; set; }
        public Xgminer.Data.Configuration.Miner MinerConfiguration { get; set; }
    }
}
