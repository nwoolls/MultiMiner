using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    [DataContract]
    public class Path
    {
        [DataMember]
        public string SharedConfigPath { get; set; }
    }
}
