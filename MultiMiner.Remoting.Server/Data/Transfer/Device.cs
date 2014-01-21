using MultiMiner.Engine;
using MultiMiner.Xgminer;

namespace MultiMiner.Remoting.Server.Data.Transfer
{
    public class Device
    {
        //device info
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public DevicePlatform Platform { get; set; }
        public int ProcessorCount { get; set; }

        //coin info
        public CryptoCoin Coin { get; set; }
        public double Difficulty { get; set; }
        public double Price { get; set; }
        public double Profitability { get; set; }
        public double AdjustedProfitability { get; set; }
        public double AverageProfitability { get; set; }

        //calculated
        public string Pool { get; set; }
        public double Exchange { get; set; }
        public double EffectiveHashRate { get; set; }

        //device stats
        public double Temperature { get; set; }
        public int FanPercent { get; set; }
        public double AverageHashrate { get; set; }
        public double CurrentHashrate { get; set; }
        public double DailyIncome { get; set; }
        public int AcceptedShares { get; set; }
        public int RejectedShares { get; set; }
        public int HardwareErrors { get; set; }
        public double Utility { get; set; }
        public double WorkUtility { get; set; }
        public string Intensity { get; set; } //string, might be D
        public int PoolIndex { get; set; }
        public double RejectedSharesPercent { get; set; }
        public double HardwareErrorsPercent { get; set; }
    }
}
