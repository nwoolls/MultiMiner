using MultiMiner.Xgminer.Data;
using System.ServiceModel;

namespace MultiMiner.Remoting
{
    //do not use collections - arrays only for Mono compatibility
    [ServiceContract]
    [ServiceKnownType(typeof(CoinAlgorithm))]
    public interface IRemotingService
    {
        [OperationContract]
        void GetApplicationModels(
            string signature,
            out Data.Transfer.Device[] devices,
            out Engine.Data.PoolGroup[] configurations, 
            out bool mining, 
            out bool hasChanges,
            out bool dynamicIntensity);
        [OperationContract]
        void GetApplicationConfiguration(
            string signature,
            out Data.Transfer.Configuration.Application application,
            out Data.Transfer.Configuration.Engine engine,
            out Data.Transfer.Configuration.Path path,
            out Data.Transfer.Configuration.Perks perks);
        [OperationContract]
        void SetApplicationConfiguration(
            string signature,
            Data.Transfer.Configuration.Application application,
            Data.Transfer.Configuration.Engine engine,
            Data.Transfer.Configuration.Path path,
            Data.Transfer.Configuration.Perks perks);
        [OperationContract]
        void SetCoinConfigurations(
            string signature,
            Engine.Data.Configuration.Coin[] coinConfigurations);
        [OperationContract]
        void StopMining(string signature);
        [OperationContract]
        void StartMining(string signature);
        [OperationContract]
        void RestartMining(string signature);
        [OperationContract]
        void ScanHardware(string signature);
        [OperationContract]
        void SetAllDevicesToCoin(string signature, string coinSymbol, bool disableStrategies);
        [OperationContract]
        void SetDevicesToCoin(string signature, DeviceDescriptor[] devices, string coinSymbol);
        [OperationContract]
        void SaveChanges(string signature);
        [OperationContract]
        void CancelChanges(string signature);
        [OperationContract]
        void ToggleDevices(string signature, DeviceDescriptor[] devices, bool enabled);
        [OperationContract]
        void ToggleDynamicIntensity(string signature, bool enabled);
        [OperationContract]
        void UpgradeMultiMiner(string signature);
        [OperationContract]
        void UpgradeBackendMiner(string signature);
    }
}
