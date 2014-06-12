using MultiMiner.CoinApi.Data;
using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Services;
using MultiMiner.Utility.Serialization;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MultiMiner.UX.ViewModels
{
    public class ApplicationViewModel
    {
        //configuration
        private readonly Engine.Data.Configuration.Engine engineConfiguration = new Engine.Data.Configuration.Engine();
        private readonly UX.Data.Configuration.Application applicationConfiguration = new UX.Data.Configuration.Application();
        private readonly UX.Data.Configuration.Paths pathConfiguration = new UX.Data.Configuration.Paths();
        private readonly UX.Data.Configuration.Perks perksConfiguration = new UX.Data.Configuration.Perks();
        private readonly UX.Data.Configuration.NetworkDevices networkDevicesConfiguration = new UX.Data.Configuration.NetworkDevices();
        private readonly UX.Data.Configuration.Metadata metadataConfiguration = new UX.Data.Configuration.Metadata();

        public Engine.Data.Configuration.Engine EngineConfiguration { get { return engineConfiguration; } }
        public UX.Data.Configuration.Application ApplicationConfiguration { get { return applicationConfiguration; } }
        public UX.Data.Configuration.Paths PathConfiguration { get { return pathConfiguration; } }
        public UX.Data.Configuration.Perks PerksConfiguration { get { return perksConfiguration; } }
        public UX.Data.Configuration.NetworkDevices NetworkDevicesConfiguration { get { return networkDevicesConfiguration; } }
        public UX.Data.Configuration.Metadata MetadataConfiguration { get { return metadataConfiguration; } }

        //view models
        private readonly MinerFormViewModel localViewModel = new MinerFormViewModel();
        private readonly MinerFormViewModel remoteViewModel = new MinerFormViewModel();

        public MinerFormViewModel LocalViewModel { get { return localViewModel; } }
        public MinerFormViewModel RemoteViewModel { get { return remoteViewModel; } }

        //hardware information
        private List<Xgminer.Data.Device> devices = new List<Xgminer.Data.Device>();

        public List<Xgminer.Data.Device> Devices { get { return devices; } }
        
        //Coin API information

        public List<CoinInformation> CoinApiInformation { get; set; }

        #region Primary mining logic
        public void ScanHardwareLocally()
        {
            try
            {
                DevicesService devicesService = new DevicesService(EngineConfiguration.XgminerConfiguration);
                MinerDescriptor defaultMiner = MinerFactory.Instance.GetDefaultMiner();
                devices = devicesService.GetDevices(MinerPath.GetPathToInstalledMiner(defaultMiner));

                //safe to do here as we are Scanning Hardware - we are not mining
                //no data to lose in the ViewModel
                //clearing means our sort order within the ListView is preserved
                //and things like selecting the first item work better
                //http://social.msdn.microsoft.com/Forums/windows/en-US/8a81c5a6-251c-4bf9-91c5-a937b5cfe9f3/possible-bug-in-listview-control-topitem-property-doesnt-work-with-groups
                LocalViewModel.Devices.Clear();

                ApplyModelsToViewModel();
            }
            catch (Win32Exception)
            {
                //miner not installed/not launched
                devices = new List<Xgminer.Data.Device>(); //dummy empty device list
            }

            if ((devices.Count > 0) && (EngineConfiguration.DeviceConfigurations.Count == 0) &&
                (EngineConfiguration.CoinConfigurations.Count == 1))
            {
                //setup devices for a brand new user
                ConfigureDevicesForNewUser();
            }

            //first try to match up devices without configurations with configurations without devices
            //could happen if, for instance, a COM port changes for a device
            FixOrphanedDeviceConfigurations();

            //there needs to be a device config for each device
            AddMissingDeviceConfigurations();
            //but no configurations for devices that have gone missing
            RemoveExcessDeviceConfigurations();
            //remove any duplicate configurations
            EngineConfiguration.RemoveDuplicateDeviceConfigurations();
                            
            //cache devices
            SaveKnownDevicesToFile();
        }

        private void ConfigureDevicesForNewUser()
        {
            Engine.Data.Configuration.Coin coinConfiguration = EngineConfiguration.CoinConfigurations.Single();

            for (int i = 0; i < devices.Count; i++)
            {
                Engine.Data.Configuration.Device deviceConfiguration = new Engine.Data.Configuration.Device()
                {
                    CoinSymbol = coinConfiguration.CryptoCoin.Symbol,
                    Enabled = true
                };

                deviceConfiguration.Assign(devices[i]);
                EngineConfiguration.DeviceConfigurations.Add(deviceConfiguration);
            }

            EngineConfiguration.SaveDeviceConfigurations();
        }

        //try to match up devices without configurations with configurations without devices
        //could happen if, for instance, a COM port changes for a device
        private void FixOrphanedDeviceConfigurations()
        {
            foreach (Xgminer.Data.Device device in devices)
            {
                Engine.Data.Configuration.Device existingConfiguration = EngineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));

                //if there is no configuration specifically for the device
                if (existingConfiguration == null)
                {
                    //find a configuration that uses the same driver and that, itself, has no specifically matching device
                    Engine.Data.Configuration.Device orphanedConfiguration = EngineConfiguration.DeviceConfigurations.FirstOrDefault(
                        c => c.Driver.Equals(device.Driver, StringComparison.OrdinalIgnoreCase) &&
                                !devices.Exists(d => d.Equals(c)));

                    if (orphanedConfiguration != null)
                        orphanedConfiguration.Assign(device);
                }
            }
        }

        //each device needs to have a DeviceConfiguration
        //this will add any missing ones after populating devices
        //for instance if the user starts up the app with a new device
        public void AddMissingDeviceConfigurations()
        {
            bool hasBtcConfigured = EngineConfiguration.CoinConfigurations.Exists(c => c.Enabled && c.CryptoCoin.Symbol.Equals(KnownCoins.BitcoinSymbol, StringComparison.OrdinalIgnoreCase));
            bool hasLtcConfigured = EngineConfiguration.CoinConfigurations.Exists(c => c.Enabled && c.CryptoCoin.Symbol.Equals(KnownCoins.LitecoinSymbol, StringComparison.OrdinalIgnoreCase));

            foreach (Xgminer.Data.Device device in devices)
            {
                Engine.Data.Configuration.Device existingConfiguration = EngineConfiguration.DeviceConfigurations.FirstOrDefault(
                    c => (c.Equals(device)));
                if (existingConfiguration == null)
                {
                    Engine.Data.Configuration.Device newConfiguration = new Engine.Data.Configuration.Device();

                    newConfiguration.Assign(device);

                    if (device.SupportsAlgorithm(CoinAlgorithm.Scrypt) && hasLtcConfigured)
                        newConfiguration.CoinSymbol = KnownCoins.LitecoinSymbol;
                    else if (device.SupportsAlgorithm(CoinAlgorithm.SHA256) && hasBtcConfigured)
                        newConfiguration.CoinSymbol = KnownCoins.BitcoinSymbol;

                    newConfiguration.Enabled = true;
                    EngineConfiguration.DeviceConfigurations.Add(newConfiguration);
                }
            }
        }

        //each device needs to have a DeviceConfiguration
        //this will remove any access ones after populating devices
        //for instance if the user starts up the app with missing devices
        private void RemoveExcessDeviceConfigurations()
        {
            EngineConfiguration.DeviceConfigurations.RemoveAll(c => !devices.Exists(d => d.Equals(c)));
        }
        #endregion

        #region Settings logic
        private static string KnownDevicesFileName()
        {
            string filePath = ApplicationPaths.AppDataPath();
            return Path.Combine(filePath, "KnownDevicesCache.xml");
        }

        public void LoadKnownDevicesFromFile()
        {
            string knownDevicesFileName = KnownDevicesFileName();
            if (File.Exists(knownDevicesFileName))
            {
                devices = ConfigurationReaderWriter.ReadConfiguration<List<Xgminer.Data.Device>>(knownDevicesFileName);
                ApplyModelsToViewModel();
            }
        }

        private void SaveKnownDevicesToFile()
        {
            ConfigurationReaderWriter.WriteConfiguration(devices, KnownDevicesFileName());
        }
        #endregion

        #region Model / ViewModel behavior
        public void ApplyModelsToViewModel()
        {
            ApplyDevicesToViewModel();
            LocalViewModel.ApplyDeviceConfigurationModels(EngineConfiguration.DeviceConfigurations,
                EngineConfiguration.CoinConfigurations);
            ApplyCoinInformationToViewModel();
            LocalViewModel.ApplyCoinConfigurationModels(EngineConfiguration.CoinConfigurations);
        }

        private void ApplyDevicesToViewModel()
        {
            //ApplyDeviceModels() ensures we have a 1-to-1 with listview items
            LocalViewModel.ApplyDeviceModels(devices, NetworkDevicesConfiguration.Devices, MetadataConfiguration.Devices);
        }

        public void ApplyCoinInformationToViewModel()
        {
            if (CoinApiInformation != null)
                LocalViewModel.ApplyCoinInformationModels(CoinApiInformation);
        }
        #endregion

    }
}
