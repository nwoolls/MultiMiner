using MultiMiner.Engine;
using MultiMiner.Engine.Data;
using MultiMiner.Engine.Data.Configuration;
using MultiMiner.Utility.OS;
using MultiMiner.Utility.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace MultiMiner.UX.Data.Configuration
{
    [XmlType(TypeName = "ApplicationConfiguration")]
    public class Application
    {
        public enum TimerInterval
        {
            FiveMinutes = 0,
            FifteenMinutes = 1,
            ThirtyMinutes = 2,
            OneHour = 3,
            TwoHours = 4,
            ThreeHours = 5,
            SixHours = 6,
            TwelveHours = 7,
            TwentyFourHours = 8,
            FortyEightHours = 9
        }

        public Application()
        {
            this.StartupMiningDelay = 45;
            this.StrategyCheckInterval = TimerInterval.FifteenMinutes;
            this.RollOverLogFiles = true;
            this.OldLogFileSets = 1;
            this.SuggestCoinsToMine = false;
            this.MobileMinerUsesHttps = true;
            this.SuggestionsAlgorithm = CoinSuggestionsAlgorithm.SHA256 | CoinSuggestionsAlgorithm.Scrypt;
            this.CheckForMinerUpdates = true;
            this.ListViewStyle = ListViewStyle.Details;
            this.HiddenColumns = new List<string>();
            this.SetGpuEnvironmentVariables = true;
            this.CoinWarzApiKey = String.Empty; //simplify handling NULL
            this.WhatMineApiKey = String.Empty;
            this.NetworkDeviceDetection = true;
        }

        public bool LaunchOnWindowsLogin { get; set; }
        public bool StartMiningOnStartup { get; set; }
        public int StartupMiningDelay { get; set; }
        public bool RestartCrashedMiners { get; set; }
        public bool MinimizeToNotificationArea { get; set; }
        public bool StartupMinimized { get; set; }
        public bool DetectDisownedMiners { get; set; }
        public bool Maximized { get; set; }
        public bool AutoSetDesktopMode { get; set; }
        public Rectangle AppPosition { get; set; }
        public bool CheckForMinerUpdates { get; set; }
        public bool BriefUserInterface { get; set; }
        public ListViewStyle ListViewStyle { get; set; }
        public List<string> HiddenColumns { get; set; }
        public bool ShowApiErrors { get; set; }
        public bool UseAccessibleMenu { get; set; }
        public bool SetGpuEnvironmentVariables { get; set; }
        public int TipsShown { get; set; }
        public bool AllowMultipleInstances { get; set; }
        public string SubmittedStatsVersion { get; set; }
        public bool ShowWorkUtility { get; set; }
        public bool ShowPoolPort { get; set; }
        public bool NetworkDeviceDetection { get; set; }
        public bool NetworkDeviceScanClassB { get; set; }
        public bool NetworkDeviceScanClassA { get; set; }
        public bool SaveCoinsToAllMachines { get; set; }

        public bool MobileMinerMonitoring { get; set; }
        public bool MobileMinerRemoteCommands { get; set; }
        public string MobileMinerEmailAddress { get; set; }
        public string MobileMinerApplicationKey { get; set; }
        public bool MobileMinerUsesHttps { get; set; }
        public bool MobileMinerPushNotifications { get; set; }
        public bool MobileMinerNetworkMonitorOnly { get; set; }

        public bool UseCoinWarzApi { get; set; }
        public string CoinWarzApiKey { get; set; }
        public string CoinWarzUrlParms { get; set; }

        public bool UseWhatMineApi { get; set; }
        public string WhatMineApiKey { get; set; }

        public bool UseWhatToMineApi { get; set; }
        public string WhatToMineUrlParms { get; set; }

        public TimerInterval StrategyCheckInterval { get; set; }
        
        public bool SuggestCoinsToMine { get; set; }
        public CoinSuggestionsAlgorithm SuggestionsAlgorithm { get; set; }

        public bool RollOverLogFiles { get; set; }
        public int OldLogFileSets { get; set; }
        public string LogFilePath { get; set; }
        public int DetailsAreaWidth { get; set; }
        public int InstancesAreaWidth { get; set; }

        public bool ScheduledRestartMining { get; set; }
        public TimerInterval ScheduledRestartMiningInterval { get; set; }
        public bool ScheduledRestartNetworkDevices { get; set; }
        public bool ScheduledRebootNetworkDevices { get; set; }
        public TimerInterval ScheduledRestartNetworkDevicesInterval { get; set; }

        private string configDirectory;
        public string ApplicationConfigurationFileName()
        {
            return Path.Combine(configDirectory, "ApplicationConfiguration.xml");
        }

        public void SaveApplicationConfiguration(string configDirectory = null)
        {
            InitializeConfigDirectory(configDirectory);

            ConfigurationReaderWriter.WriteConfiguration(this, ApplicationConfigurationFileName());

            if (OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix)
                ApplyLaunchOnWindowsStartup();
        }

        private void InitializeConfigDirectory(string configDirectory)
        {
            if (!String.IsNullOrEmpty(configDirectory))
                this.configDirectory = configDirectory;
            else if (String.IsNullOrEmpty(this.configDirectory))
                this.configDirectory = ApplicationPaths.AppDataPath();
        }

        private void ApplyLaunchOnWindowsStartup()
        {
            //launch using a .lnk file - launching via registry has proven troublesome, trouble launching cgminer after
#if !__MonoCS__
            if (LaunchOnWindowsLogin)
                StartupShortcut.CreateStartupFolderShortcut();
            else
                StartupShortcut.DeleteStartupFolderShortcuts();
#endif
        }

        public void LoadApplicationConfiguration(string configDirectory)
        {
            InitializeConfigDirectory(configDirectory);

            Application tmp = ConfigurationReaderWriter.ReadConfiguration<Application>(ApplicationConfigurationFileName());

            ObjectCopier.CopyObject(tmp, this);
        }

        public bool IsMobileMinerConfigured()
        {
            return !string.IsNullOrEmpty(MobileMinerApplicationKey) &&
                !string.IsNullOrEmpty(MobileMinerEmailAddress);
        }
    }
}
