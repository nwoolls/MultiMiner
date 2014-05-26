using MultiMiner.Xgminer.Data;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Data.Transfer
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    [DataContract]
    public class Machine
    {
        public Machine()
        {
            TotalHashrates = new Dictionary<string, double>();
        }

        [DataMember]
        public double TotalScryptHashrate { get; set; }

        [DataMember]
        public double TotalSha256Hashrate { get; set; }

        [DataMember]
        public Dictionary<string, double> TotalHashrates { get; set; }
    }
}
