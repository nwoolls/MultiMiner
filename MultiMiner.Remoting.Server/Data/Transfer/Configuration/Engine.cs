using MultiMiner.Engine.Configuration;
using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Server.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    [DataContract]
    public class Engine
    {
        public Engine()
        {
            XgminerConfiguration = new Xgminer();
            StrategyConfiguration = new StrategyConfiguration();
        }

        [DataMember]
        public Device[] DeviceConfigurations { get; set; }

        [DataMember]
        public CoinConfiguration[] CoinConfigurations { get; set; }

        [DataMember]
        public Xgminer XgminerConfiguration { get; set; }

        [DataMember]
        public StrategyConfiguration StrategyConfiguration { get; set; }
    }
}
