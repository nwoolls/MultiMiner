using MultiMiner.MobileMiner.Data;
using MultiMiner.Xgminer.Api.Data;

namespace MultiMiner.UX.Extensions
{
    static class MiningStatisticsExtensions
    {
        public static void PopulateFrom(this MiningStatistics miningStatistics, DeviceInformation deviceInformation)
        {
            miningStatistics.AcceptedShares = deviceInformation.AcceptedShares;
            miningStatistics.AverageHashrate = deviceInformation.AverageHashrate;
            miningStatistics.CurrentHashrate = deviceInformation.CurrentHashrate;
            miningStatistics.Enabled = deviceInformation.Enabled;
            miningStatistics.FanPercent = deviceInformation.FanPercent;
            miningStatistics.FanSpeed = deviceInformation.FanSpeed;
            miningStatistics.GpuActivity = deviceInformation.GpuActivity;
            miningStatistics.GpuClock = deviceInformation.GpuClock;
            miningStatistics.GpuVoltage = deviceInformation.GpuVoltage;
            miningStatistics.HardwareErrors = deviceInformation.HardwareErrors;
            miningStatistics.Index = deviceInformation.Index;
            miningStatistics.Intensity = deviceInformation.Intensity;
            miningStatistics.Kind = deviceInformation.Kind;
            miningStatistics.MemoryClock = deviceInformation.MemoryClock;
            miningStatistics.PowerTune = deviceInformation.PowerTune;
            miningStatistics.RejectedShares = deviceInformation.RejectedShares;
            miningStatistics.Status = deviceInformation.Status;
            miningStatistics.Temperature = deviceInformation.Temperature;
            miningStatistics.Utility = deviceInformation.Utility;
            //new properties from bfgminer
            miningStatistics.Name = deviceInformation.Name;
            miningStatistics.DeviceID = deviceInformation.ID;
            miningStatistics.PoolIndex = deviceInformation.PoolIndex;
            //miningStatistics.PoolName = deviceInformation.PoolName;
            miningStatistics.RejectedSharesPercent = deviceInformation.RejectedSharesPercent;
            miningStatistics.HardwareErrorsPercent = deviceInformation.HardwareErrorsPercent;
        }
    }
}
