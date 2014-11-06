namespace MultiMiner.Xgminer.Api.Data
{
    public class MinerStatistics
    {
        public string ID { get; set; }

        // AntMiner information
        public string[] ChainStatus = new string[16];
        public double Frequency { get; set; }
        public int Elapsed { get; set; }
    }
}
