namespace MultiMiner.Remoting.Server.Data.Transfer
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    public class Machine
    {
        public double TotalScryptHashrate { get; set; }
        public double TotalSha256Hashrate { get; set; }
    }
}
