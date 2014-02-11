using MultiMiner.Utility.Serialization;
using MultiMiner.Xgminer;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Win.Extensions
{
    public static class EngineConfigurationExtensions
    {
        public static Remoting.Server.Data.Transfer.Configuration.Engine ToTransferObject(this Engine.Configuration.EngineConfiguration modelObject)
        {
            Remoting.Server.Data.Transfer.Configuration.Engine transferObject = new Remoting.Server.Data.Transfer.Configuration.Engine();

            ObjectCopier.CopyObject(modelObject, transferObject, "DeviceConfigurations", "XgminerConfiguration", "CoinConfigurations");
            
            transferObject.DeviceConfigurations = modelObject.DeviceConfigurations.ToTransferObjects().ToArray();

            ObjectCopier.CopyObject(modelObject.XgminerConfiguration, transferObject.XgminerConfiguration, "AlgorithmFlags");
            foreach (KeyValuePair<CoinAlgorithm, string> pair in modelObject.XgminerConfiguration.AlgorithmFlags)
                transferObject.XgminerConfiguration.AlgorithmFlags.Add(pair.Key, pair.Value);

            transferObject.CoinConfigurations = modelObject.CoinConfigurations.ToArray();

            return transferObject;
        }

        public static Engine.Configuration.EngineConfiguration ToModelObject(this Remoting.Server.Data.Transfer.Configuration.Engine transferObject)
        {
            Engine.Configuration.EngineConfiguration modelObject = new Engine.Configuration.EngineConfiguration();

            ObjectCopier.CopyObject(transferObject, modelObject, "DeviceConfigurations", "XgminerConfiguration", "CoinConfigurations");

            modelObject.DeviceConfigurations = transferObject.DeviceConfigurations.ToList().ToModelObjects();

            ObjectCopier.CopyObject(transferObject.XgminerConfiguration, modelObject.XgminerConfiguration, "AlgorithmFlags");

            foreach (CoinAlgorithm key in transferObject.XgminerConfiguration.AlgorithmFlags.Keys)
                modelObject.XgminerConfiguration.AlgorithmFlags.Add(key, (string)transferObject.XgminerConfiguration.AlgorithmFlags[key]);

            modelObject.CoinConfigurations = transferObject.CoinConfigurations.ToList();

            return modelObject;
        }
    }
}
