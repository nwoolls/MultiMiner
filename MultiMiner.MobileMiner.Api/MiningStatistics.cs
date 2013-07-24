namespace MultiMiner.MobileMiner.Api
{
    public class MiningStatistics
    {
        //User
        public virtual string EmailAddress { get; set; }
        public virtual string ApplicationKey { get; set; }

        //Machine
        public virtual string MachineName { get; set; }

        //MiningStatistics
        public string MinerName { get; set; } //e.g. MultiMiner, Asteroid, etc.
        public string CoinSymbol { get; set; }
        public string CoinName { get; set; }
        public string Algorithm { get; set; } //e.g. scrypt, SHA-256

        //rest is taken from MultiMiner.Xgminer.Api.DeviceInformation
        public string Kind { get; set; }
        public int Index { get; set; }
        public bool Enabled { get; set; }
        public string Status { get; set; }
        public double Temperature { get; set; }
        public int FanSpeed { get; set; }
        public int FanPercent { get; set; }
        public int GpuClock { get; set; }
        public int MemoryClock { get; set; }
        public double GpuVoltage { get; set; }
        public int GpuActivity { get; set; }
        public int PowerTune { get; set; }
        public double AverageHashrate { get; set; }
        public double CurrentHashrate { get; set; }
        public int AcceptedShares { get; set; }
        public int RejectedShares { get; set; }
        public int HardwareErrors { get; set; }
        public double Utility { get; set; }
        public string Intensity { get; set; } //string, might be D
    }
}
