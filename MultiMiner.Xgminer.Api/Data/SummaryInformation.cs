namespace MultiMiner.Xgminer.Api.Data
{
    public class SummaryInformation
    {
        public long Elapsed { get; set; }
        public double AverageHashrate { get; set; }
        public long FoundBlocks { get; set; }
        public long GetWorks { get; set; }
        public long AcceptedShares { get; set; }
        public long RejectedShares { get; set; }
        public long HardwareErrors { get; set; }
        public double Utility { get; set; }
        public long DiscardedShares { get; set; }
        public long StaleShares { get; set; }
        public long GetFailures { get; set; }
        public long LocalWork { get; set; }
        public long RemoteFailures { get; set; }
        public long NetworkBlocks { get; set; }
        public double TotalHashrate { get; set; }
        public double WorkUtility { get; set; }
        public double DifficultyAccepted { get; set; }
        public double DifficultyRejected { get; set; }
        public double DifficultyStale { get; set; }
        public double BestShare { get; set; }
        public double DeviceHardwarePercent { get; set; }
        public double DeviceRejectedPercent { get; set; }
        public double PoolRejectedPercent { get; set; }
        public double PoolStatePercent { get; set; }
    }
}
