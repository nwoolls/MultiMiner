using System.Collections.Generic;
using System.ServiceModel;

namespace MultiMiner.Remoting.Server
{
    [ServiceContract]
    public interface IRemotingService
    {
        [OperationContract]
        IEnumerable<Data.Transfer.Device> GetDevices();
    }
}
