using MultiMiner.Engine.Configuration;
using System.Collections.Generic;

namespace MultiMiner.Remoting.Server.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    public class Engine
    {
        public Engine()
        {
            XgminerConfiguration = new Xgminer();
            StrategyConfiguration = new StrategyConfiguration();
        }

        public Configuration.Device[] DeviceConfigurations { get; set; }
        public CoinConfiguration[] CoinConfigurations { get; set; }
        public Configuration.Xgminer XgminerConfiguration { get; set; }
        public StrategyConfiguration StrategyConfiguration { get; set; }
    }
}
