using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;

namespace MultiMiner.Remoting.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, UseSynchronizationContext = false)]
    class RemotingService : IRemotingService
    {
        public IEnumerable<Data.Transfer.Device> GetDevices()
        {
            List<Data.Transfer.Device> result = ApplicationProxy.Instance.Devices.ToList();
            return result;
        }
    }
}
