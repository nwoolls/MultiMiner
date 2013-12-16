using MultiMiner.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MultiMiner.Win.Configuration
{
    public class ApplicationConfiguration
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
            TwelveHours = 7
        }

        [Flags]
        public enum CoinSuggestionsAlgorithm
        {
            None = 0x0,
            SHA256 = 0x1,
            Scrypt = 0x2
        }

        public ApplicationConfiguration()
        {
            this.StartupMiningDelay = 45;
            this.StrategyCheckInterval = TimerInterval.FifteenMinutes;
            this.RollOverLogFiles = true;
            this.OldLogFileSets = 1;
            this.SuggestCoinsToMine = false;
            this.MobileMinerUsesHttps = true;
            this.SuggestionsAlgorithm = CoinSuggestionsAlgorithm.SHA256 | CoinSuggestionsAlgorithm.Scrypt;
            this.CheckForMinerUpdates = true;
            this.ListViewStyle = View.Details;
            this.HiddenColumns = new List<string>();
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
        public View ListViewStyle { get; set; }
        public List<string> HiddenColumns { get; set; }
        public bool ShowApiErrors { get; set; }

        public bool MobileMinerMonitoring { get; set; }
        public bool MobileMinerRemoteCommands { get; set; }
        public string MobileMinerEmailAddress { get; set; }
        public string MobileMinerApplicationKey { get; set; }
        public bool MobileMinerUsesHttps { get; set; }
        public bool MobileMinerPushNotifications { get; set; }

        public bool UseCoinWarzApi { get; set; }
        public string CoinWarzApiKey { get; set; }

        public TimerInterval StrategyCheckInterval { get; set; }
        
        public bool SuggestCoinsToMine { get; set; }
        public CoinSuggestionsAlgorithm SuggestionsAlgorithm { get; set; }

        public bool LogAreaVisible { get; set; }
        public int LogAreaTabIndex { get; set; }
        public int LogAreaDistance { get; set; }
        public bool RollOverLogFiles { get; set; }
        public int OldLogFileSets { get; set; }
        public string LogFilePath { get; set; }

        public bool ScheduledRestartMining { get; set; }
        public TimerInterval ScheduledRestartMiningInterval { get; set; }

        private string configDirectory;
        public string ApplicationConfigurationFileName()
        {
            return Path.Combine(configDirectory, "ApplicationConfiguration.xml");
        }

        public void SaveApplicationConfiguration(string configDirectory = null)
        {
            if (!String.IsNullOrEmpty(configDirectory))
                this.configDirectory = configDirectory;

            ConfigurationReaderWriter.WriteConfiguration(this, ApplicationConfigurationFileName());

            if (OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix)
                ApplyLaunchOnWindowsStartup();
        }

        private void ApplyLaunchOnWindowsStartup()
        {
            //launch using a .lnk file - launching via registry has proven troublesome, trouble launching cgminer after
            if (LaunchOnWindowsLogin)
                WindowsStartupShortcut.CreateStartupFolderShortcut();
            else
                WindowsStartupShortcut.DeleteStartupFolderShortcuts();
        }

        public void LoadApplicationConfiguration(string configDirectory)
        {
            if (String.IsNullOrEmpty(configDirectory))
                this.configDirectory = ApplicationPaths.AppDataPath();
            else
                this.configDirectory = configDirectory;

            ApplicationConfiguration tmp = ConfigurationReaderWriter.ReadConfiguration<ApplicationConfiguration>(ApplicationConfigurationFileName());

            ObjectCopier.CopyObject(tmp, this);
        }
    }
}
