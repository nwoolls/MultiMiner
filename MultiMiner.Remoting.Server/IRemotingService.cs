using System.Collections.Generic;
using System.ServiceModel;

namespace MultiMiner.Remoting.Server
{
    [ServiceContract]
    public interface IRemotingService
    {
        [OperationContract]
        void GetDevices(out IEnumerable<Data.Transfer.Device> devices, out bool mining);
        [OperationContract]
        void StopMining(string signature);
        [OperationContract]
        void StartMining(string signature);
        [OperationContract]
        void RestartMining(string signature);
    }
}
