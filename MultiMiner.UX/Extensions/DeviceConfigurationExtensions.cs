using MultiMiner.Utility.Serialization;
using System.Collections.Generic;
using MultiMiner.Remoting.Data.Transfer.Configuration;

namespace MultiMiner.UX.Extensions
{
    public static class DeviceConfigurationExtensions
    {
        public static List<Remoting.Data.Transfer.Configuration.Device> ToTransferObjects(this List<Engine.Data.Configuration.Device> modelObjects)
        {
            List<Remoting.Data.Transfer.Configuration.Device> transferObjects = new List<Remoting.Data.Transfer.Configuration.Device>();

            foreach (Engine.Data.Configuration.Device modelConfig in modelObjects)
            {
                Device transferObject = ObjectCopier.CloneObject<Engine.Data.Configuration.Device, Remoting.Data.Transfer.Configuration.Device>(modelConfig);
                transferObjects.Add(transferObject);
            }

            return transferObjects;
        }

        public static List<Engine.Data.Configuration.Device> ToModelObjects(this List<Remoting.Data.Transfer.Configuration.Device> transferObjects)
        {
            List<Engine.Data.Configuration.Device> modelObjects = new List<Engine.Data.Configuration.Device>();

            foreach (Remoting.Data.Transfer.Configuration.Device transferConfig in transferObjects)
            {
                Engine.Data.Configuration.Device modelObject = ObjectCopier.CloneObject<Remoting.Data.Transfer.Configuration.Device, Engine.Data.Configuration.Device>(transferConfig);
                modelObjects.Add(modelObject);
            }

            return modelObjects;
        }
    }
}
