using MultiMiner.Engine;
using MultiMiner.Xgminer;
using System;
using System.Collections.Generic;

namespace MultiMiner.Remoting.Server
{
    public sealed class ApplicationProxy
    {
        //events
        //delegate declarations
        public delegate void RemoteEventHandler(object sender, RemoteCommandEventArgs ea);
        public delegate void AllCoinEventHandler(object sender, string coinSymbol, RemoteCommandEventArgs ea);
        public delegate void DevicesCoinEventHandler(object sender, IEnumerable<DeviceDescriptor> devices, string coinSymbol, RemoteCommandEventArgs ea);
        public delegate void ToggleDevicesEventHandler(object sender, IEnumerable<DeviceDescriptor> devices, bool enabled, RemoteCommandEventArgs ea);
        public delegate void ToggleEventHandler(object sender, bool enabled, RemoteCommandEventArgs ea);

        //event declarations        
        public event RemoteEventHandler StartMiningRequested;
        public event RemoteEventHandler StopMiningRequested;
        public event RemoteEventHandler RestartMiningRequested;
        public event RemoteEventHandler ScanHardwareRequested;
        public event AllCoinEventHandler SetAllDevicesToCoinRequested;
        public event DevicesCoinEventHandler SetDeviceToCoinRequested;
        public event RemoteEventHandler SaveChangesRequested;
        public event RemoteEventHandler CancelChangesRequested;
        public event ToggleDevicesEventHandler ToggleDevicesRequested;
        public event ToggleEventHandler ToggleDynamicIntensityRequested;

        private bool dynamicIntensity;
        private bool mining;
        private bool hasChanges;
        private List<Data.Transfer.Device> devices;
        private List<CryptoCoin> configuredCoins;
        private static volatile ApplicationProxy instance;
        private static object syncRoot = new Object();

        private ApplicationProxy() { }

        public static ApplicationProxy Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ApplicationProxy();
                    }
                }

                return instance;
            }
        }

        public List<Data.Transfer.Device> Devices
        {
            get
            {
                lock (syncRoot)
                {
                    return devices;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    devices = value;
                }
            }
        }

        public List<CryptoCoin> ConfiguredCoins
        {
            get
            {
                lock (syncRoot)
                {
                    return configuredCoins;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    configuredCoins = value;
                }
            }
        }

        public bool Mining
        {
            get
            {
                lock (syncRoot)
                {
                    return mining;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    mining = value;
                }
            }
        }

        public bool HasChanges
        {
            get
            {
                lock (syncRoot)
                {
                    return hasChanges;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    hasChanges = value;
                }
            }
        }

        public bool DynamicIntensity
        {
            get
            {
                lock (syncRoot)
                {
                    return dynamicIntensity;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    dynamicIntensity = value;
                }
            }
        }

        public void StopMining(RemotingService sender, string clientAddress, string signature)
        {
            if (StopMiningRequested != null)
                StopMiningRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void StartMining(RemotingService sender, string clientAddress, string signature)
        {
            if (StartMiningRequested != null)
                StartMiningRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void RestartMining(RemotingService sender, string clientAddress, string signature)
        {
            if (RestartMiningRequested != null)
                RestartMiningRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void ScanHardware(RemotingService sender, string clientAddress, string signature)
        {
            if (ScanHardwareRequested != null)
                ScanHardwareRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void SetAllDevicesToCoin(RemotingService sender, string clientAddress, string signature, string coinSymbol)
        {
            if (SetAllDevicesToCoinRequested != null)
                SetAllDevicesToCoinRequested(sender, coinSymbol, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void SetDevicesToCoin(RemotingService sender, string clientAddress, string signature, IEnumerable<DeviceDescriptor> devices, string coinSymbol)
        {
            if (SetDeviceToCoinRequested != null)
                SetDeviceToCoinRequested(sender, devices, coinSymbol, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void SaveChanges(RemotingService sender, string clientAddress, string signature)
        {
            if (SaveChangesRequested != null)
                SaveChangesRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void CancelChanges(RemotingService sender, string clientAddress, string signature)
        {
            if (CancelChangesRequested != null)
                CancelChangesRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void ToggleDevices(RemotingService sender, string clientAddress, string signature, IEnumerable<DeviceDescriptor> devices, bool enabled)
        {
            if (ToggleDevicesRequested != null)
                ToggleDevicesRequested(sender, devices, enabled, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void ToggleDynamicIntensity(MultiMiner.Remoting.Server.RemotingService sender, string clientAddress, string signature, bool enabled)
        {
            if (ToggleDynamicIntensityRequested != null)
                ToggleDynamicIntensityRequested(sender, enabled, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }
    }
}
