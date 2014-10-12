using MultiMiner.Engine.Data;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;

namespace MultiMiner.Remoting
{
    public sealed class ApplicationProxy
    {
        //events
        //delegate declarations
        public delegate void RemoteEventHandler(object sender, RemoteCommandEventArgs ea);
        public delegate void AllCoinEventHandler(object sender, string coinSymbol, bool disableStrategies, RemoteCommandEventArgs ea);
        public delegate void DevicesCoinEventHandler(object sender, IEnumerable<DeviceDescriptor> devices, string coinSymbol, RemoteCommandEventArgs ea);
        public delegate void ToggleDevicesEventHandler(object sender, IEnumerable<DeviceDescriptor> devices, bool enabled, RemoteCommandEventArgs ea);
        public delegate void ToggleEventHandler(object sender, bool enabled, RemoteCommandEventArgs ea);
        public delegate void ModelRequestEventHandler(object sender, ModelEventArgs ea);
        public delegate void ConfigurationEventHandler(object sender, ConfigurationEventArgs ea);
        public delegate void CoinConfigurationsEventHandler(object sender, Engine.Data.Configuration.Coin[] coinConfigurations, RemoteCommandEventArgs ea);

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
        public event ModelRequestEventHandler GetModelRequested;
        public event ConfigurationEventHandler GetConfigurationRequested;
        public event ConfigurationEventHandler SetConfigurationRequested;
        public event CoinConfigurationsEventHandler SetCoinConfigurationsRequested;
        public event RemoteEventHandler UpgradeMultiMinerRequested;
        public event RemoteEventHandler UpgradeBackendMinerRequested;

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

        public void SetAllDevicesToCoin(RemotingService sender, string clientAddress, string signature, string coinSymbol, bool disableStrategies)
        {
            if (SetAllDevicesToCoinRequested != null)
                SetAllDevicesToCoinRequested(sender, coinSymbol, disableStrategies, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
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

        public void ToggleDynamicIntensity(RemotingService sender, string clientAddress, string signature, bool enabled)
        {
            if (ToggleDynamicIntensityRequested != null)
                ToggleDynamicIntensityRequested(sender, enabled, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void GetApplicationModels(
            RemotingService sender,
            string clientAddress,
            string signature,
            out Data.Transfer.Device[] devices,
            out PoolGroup[] configurations,
            out bool mining,
            out bool hasChanges,
            out bool dynamicIntensity)
        {
            ModelEventArgs ea = new ModelEventArgs();
            ea.IpAddress = clientAddress;
            ea.Signature = signature;

            if (GetModelRequested != null)
                GetModelRequested(sender, ea);

            devices = ea.Devices.ToArray();
            configurations = ea.ConfiguredCoins.ToArray();
            mining = ea.Mining;
            hasChanges = ea.HasChanges;
            dynamicIntensity = ea.DynamicIntensity;
        }

        public void GetApplicationConfiguration(
            RemotingService sender, 
            string clientAddress, 
            string signature, 
            out Data.Transfer.Configuration.Application application, 
            out Data.Transfer.Configuration.Engine engine, 
            out Data.Transfer.Configuration.Path path, 
            out Data.Transfer.Configuration.Perks perks)
        {
            ConfigurationEventArgs ea = new ConfigurationEventArgs();
            ea.IpAddress = clientAddress;
            ea.Signature = signature;

            if (GetConfigurationRequested != null)
                GetConfigurationRequested(sender, ea);

            application = ea.Application;
            engine = ea.Engine;
            path = ea.Path;
            perks = ea.Perks;
        }

        public void SetApplicationConfiguration(
            RemotingService sender, 
            string clientAddress, 
            string signature, 
            Data.Transfer.Configuration.Application application, 
            Data.Transfer.Configuration.Engine engine, 
            Data.Transfer.Configuration.Path path, 
            Data.Transfer.Configuration.Perks perks)
        {
            ConfigurationEventArgs ea = new ConfigurationEventArgs() 
            { 
                IpAddress = clientAddress, 
                Signature = signature, 
                Application = application, 
                Engine = engine, 
                Path = path, 
                Perks = perks 
            };

            if (SetConfigurationRequested != null)
                SetConfigurationRequested(sender, ea);
        }

        public void SetCoinConfigurations(
            RemotingService sender,
            string clientAddress, 
            string signature,
            Engine.Data.Configuration.Coin[] coinConfigurations)
        {
            if (SetCoinConfigurationsRequested != null)
                SetCoinConfigurationsRequested(sender, coinConfigurations, 
                    new RemoteCommandEventArgs 
                    { 
                        IpAddress = clientAddress, 
                        Signature = signature 
                    });
        }

        public void UpgradeMultiMiner(RemotingService sender, string clientAddress, string signature)
        {
            if (UpgradeMultiMinerRequested != null)
                UpgradeMultiMinerRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }

        public void UpgradeBackendMiner(RemotingService sender, string clientAddress, string signature)
        {
            if (UpgradeBackendMinerRequested != null)
                UpgradeBackendMinerRequested(sender, new RemoteCommandEventArgs { IpAddress = clientAddress, Signature = signature });
        }
    }
}
