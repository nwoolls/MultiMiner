using MultiMiner.Utility.Serialization;
using System.Linq;

namespace MultiMiner.UX.Extensions
{
    public static class EngineConfigurationExtensions
    {
        public static Remoting.Data.Transfer.Configuration.Engine ToTransferObject(this Engine.Data.Configuration.Engine modelObject)
        {
            Remoting.Data.Transfer.Configuration.Engine transferObject = new Remoting.Data.Transfer.Configuration.Engine();

            ObjectCopier.CopyObject(modelObject, transferObject, "DeviceConfigurations", "XgminerConfiguration", "CoinConfigurations");
            
            transferObject.DeviceConfigurations = modelObject.DeviceConfigurations.ToTransferObjects().ToArray();

            transferObject.XgminerConfiguration = modelObject.XgminerConfiguration.ToTransferObject();

            transferObject.CoinConfigurations = modelObject.CoinConfigurations.ToArray();
            
            return transferObject;
        }

        public static Engine.Data.Configuration.Engine ToModelObject(this Remoting.Data.Transfer.Configuration.Engine transferObject)
        {
            Engine.Data.Configuration.Engine modelObject = new Engine.Data.Configuration.Engine();

            ObjectCopier.CopyObject(transferObject, modelObject, "DeviceConfigurations", "XgminerConfiguration", "CoinConfigurations");

            modelObject.DeviceConfigurations = transferObject.DeviceConfigurations.ToList().ToModelObjects();

            modelObject.XgminerConfiguration = transferObject.XgminerConfiguration.ToModelObject();

            modelObject.CoinConfigurations = transferObject.CoinConfigurations.ToList();

            return modelObject;
        }
    }
}
