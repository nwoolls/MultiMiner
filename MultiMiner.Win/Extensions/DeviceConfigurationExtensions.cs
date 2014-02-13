using MultiMiner.Utility.Serialization;
using System.Collections.Generic;

namespace MultiMiner.Win.Extensions
{
    public static class DeviceConfigurationExtensions
    {
        public static List<Remoting.Server.Data.Transfer.Configuration.Device> ToTransferObjects(this List<Engine.Data.Configuration.Device> modelObjects)
        {
            List<Remoting.Server.Data.Transfer.Configuration.Device> transferObjects = new List<Remoting.Server.Data.Transfer.Configuration.Device>();

            foreach (Engine.Data.Configuration.Device modelConfig in modelObjects)
            {
                Remoting.Server.Data.Transfer.Configuration.Device transferObject;
                transferObject = ObjectCopier.CloneObject<Engine.Data.Configuration.Device, Remoting.Server.Data.Transfer.Configuration.Device>(modelConfig);
                transferObjects.Add(transferObject);
            }

            return transferObjects;
        }

        public static List<Engine.Data.Configuration.Device> ToModelObjects(this List<Remoting.Server.Data.Transfer.Configuration.Device> transferObjects)
        {
            List<Engine.Data.Configuration.Device> modelObjects = new List<Engine.Data.Configuration.Device>();

            foreach (Remoting.Server.Data.Transfer.Configuration.Device transferConfig in transferObjects)
            {
                Engine.Data.Configuration.Device modelObject;
                modelObject = ObjectCopier.CloneObject<Remoting.Server.Data.Transfer.Configuration.Device, Engine.Data.Configuration.Device>(transferConfig);
                modelObjects.Add(modelObject);
            }

            return modelObjects;
        }
    }
}
