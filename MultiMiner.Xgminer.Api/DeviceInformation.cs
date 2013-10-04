using System;
namespace MultiMiner.Xgminer.Api
{
    public class DeviceInformation
    {
        public DeviceInformation()
        {
            Status = String.Empty;
        }

        public string Kind { get; set; }
        public string Name { get; set; }
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
        public int PoolIndex { get; set; }
    }
}
