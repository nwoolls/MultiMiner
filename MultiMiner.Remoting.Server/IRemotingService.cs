using MultiMiner.Engine;
using MultiMiner.Xgminer;
using System.Collections.Generic;
using System.ServiceModel;

namespace MultiMiner.Remoting.Server
{
    [ServiceContract]
    public interface IRemotingService
    {
        [OperationContract]
        void GetDevices(out IEnumerable<Data.Transfer.Device> devices, out bool mining, out bool hasChanges);
        [OperationContract]
        void GetConfiguredCoins(out IEnumerable<CryptoCoin> configurations);
        [OperationContract]
        void StopMining(string signature);
        [OperationContract]
        void StartMining(string signature);
        [OperationContract]
        void RestartMining(string signature);
        [OperationContract]
        void ScanHardware(string signature);
        [OperationContract]
        void SetAllDevicesToCoin(string signature, string coinSymbol);
        [OperationContract]
        void SetDevicesToCoin(string signature, IEnumerable<DeviceDescriptor> devices, string coinSymbol);
        [OperationContract]
        void SaveChanges(string signature);
        [OperationContract]
        void CancelChanges(string signature);
        [OperationContract]
        void ToggleDevices(string signature, IEnumerable<DeviceDescriptor> devices, bool enabled);
    }
}
