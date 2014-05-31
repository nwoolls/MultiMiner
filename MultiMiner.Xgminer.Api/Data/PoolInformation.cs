using System;

namespace MultiMiner.Xgminer.Api.Data
{
    public class PoolInformation
    {
        public int Index { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        public int Quota { get; set; }
        public bool LongPoll { get; set; }
        public int GetWorks { get; set; }
        public int Accepted { get; set; }
        public int Rejected { get; set; }
        public int Works { get; set; }
        public int Discarded { get; set; }
        public int Stale { get; set; }
        public int GetFailures { get; set; }
        public int RemoteFailures { get; set; }
        public string User { get; set; }
        public DateTime LastShareTime { get; set; }
        public int Diff1Shares { get; set; }
        public string Proxy { get; set; }
        public double DifficultyAccepted { get; set; }
        public double DifficultyRejected { get; set; }
        public double DifficultyStale { get; set; }
        public double LastShareDifficulty { get; set; }
        public bool HasStratum { get; set; }
        public bool StratumActive { get; set; }
        public string StratumUrl { get; set; }
        public double BestShare { get; set; }
        public double PoolRejectedPercent { get; set; }
        public double PoolStalePercent { get; set; }
    }
}
