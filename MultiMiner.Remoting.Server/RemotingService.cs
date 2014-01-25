using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.ServiceModel.Channels;
using System;
using MultiMiner.Engine;

namespace MultiMiner.Remoting.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, UseSynchronizationContext = false)]
    public class RemotingService : IRemotingService
    {
        public void GetDevices(out IEnumerable<Data.Transfer.Device> devices, out bool mining)
        {
            devices = ApplicationProxy.Instance.Devices.ToList();
            mining = ApplicationProxy.Instance.Mining;
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

        public void GetConfiguredCoins(out IEnumerable<CryptoCoin> configurations)
        {
            configurations = ApplicationProxy.Instance.ConfiguredCoins.ToList();
        }

        public void SetAllDevicesToCoin(string signature, string coinSymbol)
        {
            ApplicationProxy.Instance.SetAllDevicesToCoin(this, GetClientIpAddress(), signature, coinSymbol);
        }
    }
}
