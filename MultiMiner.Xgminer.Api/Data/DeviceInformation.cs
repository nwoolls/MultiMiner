using System;

namespace MultiMiner.Xgminer.Api.Data
{
    public class DeviceInformation
    {
        public DeviceInformation()
        {
            Status = String.Empty;
            Name = String.Empty;
        }

        public string Kind { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public int ID { get; set; }
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
        public double WorkUtility { get; set; }
        public string Intensity { get; set; } //string, might be D
        public int PoolIndex { get; set; }
        public double RejectedSharesPercent { get; set; }
        public double HardwareErrorsPercent { get; set; }
        public double LastShareDifficulty { get; set; }
        public double DifficultyAccepted { get; set; }
        public int DeviceElapsed { get; set; }
    }
}
