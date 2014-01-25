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
        public void GetDevices(out IEnumerable<Data.Transfer.Device> devices, out bool mining, out bool hasChanges)
        {
            devices = ApplicationProxy.Instance.Devices.ToList();
            mining = ApplicationProxy.Instance.Mining;
            hasChanges = ApplicationProxy.Instance.HasChanges;
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

        public void GetConfiguredCoins(out IEnumerable<CryptoCoin> configurations)
        {
            configurations = ApplicationProxy.Instance.ConfiguredCoins.ToList();
        }

        public void SetAllDevicesToCoin(string signature, string coinSymbol)
        {
            ApplicationProxy.Instance.SetAllDevicesToCoin(this, GetClientIpAddress(), signature, coinSymbol);
        }

        public void SetDevicesToCoin(string signature, IEnumerable<DeviceDescriptor> devices, string coinSymbol)
        {
            ApplicationProxy.Instance.SetDevicesToCoin(this, GetClientIpAddress(), signature, devices, coinSymbol);
        }
    }
}
