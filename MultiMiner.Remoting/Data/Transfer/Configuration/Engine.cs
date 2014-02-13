using MultiMiner.Engine.Data.Configuration;
using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    [DataContract]
    public class Engine
    {
        public Engine()
        {
            XgminerConfiguration = new Xgminer();
            StrategyConfiguration = new Strategy();
        }

        [DataMember]
        public Device[] DeviceConfigurations { get; set; }

        [DataMember]
        public Coin[] CoinConfigurations { get; set; }

        [DataMember]
        public Xgminer XgminerConfiguration { get; set; }

        [DataMember]
        public Strategy StrategyConfiguration { get; set; }
    }
}
