using MultiMiner.Utility.Serialization;
using System.Collections.Generic;

namespace MultiMiner.Win.Extensions
{
    public static class DeviceConfigurationExtensions
    {
        public static List<Remoting.Server.Data.Transfer.Configuration.Device> ToTransferObjects(this List<Engine.Configuration.DeviceConfiguration> modelObjects)
        {
            List<Remoting.Server.Data.Transfer.Configuration.Device> transferObjects = new List<Remoting.Server.Data.Transfer.Configuration.Device>();

            foreach (Engine.Configuration.DeviceConfiguration modelConfig in modelObjects)
            {
                Remoting.Server.Data.Transfer.Configuration.Device transferObject;
                transferObject = ObjectCopier.CloneObject<Engine.Configuration.DeviceConfiguration, Remoting.Server.Data.Transfer.Configuration.Device>(modelConfig);
                transferObjects.Add(transferObject);
            }

            return transferObjects;
        }

        public static List<Engine.Configuration.DeviceConfiguration> ToModelObjects(this List<Remoting.Server.Data.Transfer.Configuration.Device> transferObjects)
        {
            List<Engine.Configuration.DeviceConfiguration> modelObjects = new List<Engine.Configuration.DeviceConfiguration>();

            foreach (Remoting.Server.Data.Transfer.Configuration.Device transferConfig in transferObjects)
            {
                Engine.Configuration.DeviceConfiguration modelObject;
                modelObject = ObjectCopier.CloneObject<Remoting.Server.Data.Transfer.Configuration.Device, Engine.Configuration.DeviceConfiguration>(transferConfig);
                modelObjects.Add(modelObject);
            }

            return modelObjects;
        }
    }
}
