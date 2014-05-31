using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    [DataContract]
    public class Perks
    {
        [DataMember]
        public bool PerksEnabled { get; set; }

        [DataMember]
        public bool ShowExchangeRates { get; set; }

        [DataMember]
        public bool ShowIncomeRates { get; set; }

        [DataMember]
        public bool ShowIncomeInUsd { get; set; }

        [DataMember]
        public bool EnableRemoting { get; set; }

        [DataMember]
        public string RemotingPassword { get; set; }

        [DataMember]
        public bool AdvancedProxying { get; set; }
    }
}
