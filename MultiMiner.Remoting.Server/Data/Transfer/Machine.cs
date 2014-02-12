using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Server.Data.Transfer
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    [DataContract]
    public class Machine
    {
        [DataMember]
        public double TotalScryptHashrate { get; set; }

        [DataMember]
        public double TotalSha256Hashrate { get; set; }
    }
}
