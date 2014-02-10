namespace MultiMiner.Remoting.Server.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    public class Perks
    {
        public bool PerksEnabled { get; set; }
        public bool ShowExchangeRates { get; set; }
        public bool ShowIncomeRates { get; set; }
        public bool ShowIncomeInUsd { get; set; }
        public bool EnableRemoting { get; set; }
        public string RemotingPassword { get; set; }
    }
}
