using System.Collections.Generic;

namespace MultiMiner.Engine.Configuration
{
    public class EngineConfiguration
    {
        public List<DeviceConfiguration> DeviceConfigurations { get; set; }
        public List<CoinConfiguration> CoinConfigurations { get; set; }
    }
}
