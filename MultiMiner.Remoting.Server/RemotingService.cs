using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.ServiceModel.Channels;
using System;
using MultiMiner.Engine;
using MultiMiner.Xgminer;

namespace MultiMiner.Remoting.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, UseSynchronizationContext = false)]
    public class RemotingService : IRemotingService
    {
        public void GetApplicationModels(
            string signature,
            out IEnumerable<Data.Transfer.Device> devices,
            out IEnumerable<CryptoCoin> configurations,
            out bool mining, 
            out bool hasChanges,
            out bool dynamicIntensity)
        {
            ApplicationProxy.Instance.GetApplicationModels(
                this, 
                GetClientIpAddress(), 
                signature, 
                out devices, 
                out configurations, 
                out mining, 
                out hasChanges, 
                out dynamicIntensity); 
        }

        public void GetApplicationConfiguration(
            string signature,
            out Data.Transfer.Configuration.Application application,
            out Data.Transfer.Configuration.Engine engine,
            out Data.Transfer.Configuration.Path path,
            out Data.Transfer.Configuration.Perks perks)
        {
            ApplicationProxy.Instance.GetApplicationConfiguration(
                this,
                GetClientIpAddress(),
                signature,
                out application,
                out engine,
                out path,
                out perks);
        }

        public void SetApplicationConfiguration(
            string signature,
            Data.Transfer.Configuration.Application application,
            Data.Transfer.Configuration.Engine engine,
            Data.Transfer.Configuration.Path path,
            Data.Transfer.Configuration.Perks perks)
        {
            ApplicationProxy.Instance.SetApplicationConfiguration(
                this,
                GetClientIpAddress(),
                signature,
                application,
                engine,
                path,
                perks);
        }

        private static string GetClientIpAddress()
        {
            OperationContext currentContext = OperationContext.Current;
            MessageProperties messageProperties = currentContext.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            return endpoint.Address;
        }

        public void StopMining(string signature)
        {
            ApplicationProxy.Instance.StopMining(this, GetClientIpAddress(), signature);       
        }

        public void StartMining(string signature)
        {
            ApplicationProxy.Instance.StartMining(this, GetClientIpAddress(), signature);
        }

        public void RestartMining(string signature)
        {
            ApplicationProxy.Instance.RestartMining(this, GetClientIpAddress(), signature);
        }

        public void ScanHardware(string signature)
        {
            ApplicationProxy.Instance.ScanHardware(this, GetClientIpAddress(), signature);
        }

        public void SaveChanges(string signature)
        {
            ApplicationProxy.Instance.SaveChanges(this, GetClientIpAddress(), signature);
        }

        public void CancelChanges(string signature)
        {
            ApplicationProxy.Instance.CancelChanges(this, GetClientIpAddress(), signature);
        }

        public void SetAllDevicesToCoin(string signature, string coinSymbol)
        {
            ApplicationProxy.Instance.SetAllDevicesToCoin(this, GetClientIpAddress(), signature, coinSymbol);
        }

        public void SetDevicesToCoin(string signature, IEnumerable<DeviceDescriptor> devices, string coinSymbol)
        {
            ApplicationProxy.Instance.SetDevicesToCoin(this, GetClientIpAddress(), signature, devices, coinSymbol);
        }

        public void ToggleDevices(string signature, IEnumerable<DeviceDescriptor> devices, bool enabled)
        {
            ApplicationProxy.Instance.ToggleDevices(this, GetClientIpAddress(), signature, devices, enabled);
        }

        public void ToggleDynamicIntensity(string signature, bool enabled)
        {
            ApplicationProxy.Instance.ToggleDynamicIntensity(this, GetClientIpAddress(), signature, enabled);
        }

        public void UpgradeMultiMiner(string signature)
        {
            ApplicationProxy.Instance.UpgradeMultiMiner(this, GetClientIpAddress(), signature);
        }

        public void UpgradeBackendMiner(string signature)
        {
            ApplicationProxy.Instance.UpgradeBackendMiner(this, GetClientIpAddress(), signature);
        }
    }
}
