using System;
using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Remoting.Server.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
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
            TwelveHours = 7
        }

        [Flags]
        public enum CoinSuggestionsAlgorithm
        {
            None = 0x0,
            SHA256 = 0x1,
            Scrypt = 0x2
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
        public string[] HiddenColumns { get; set; }
        public bool ShowApiErrors { get; set; }
        public bool UseAccessibleMenu { get; set; }
        public bool SetGpuEnvironmentVariables { get; set; }
        public int TipsShown { get; set; }
        public bool AllowMultipleInstances { get; set; }
        public string SubmittedStatsVersion { get; set; }
        public bool ShowWorkUtility { get; set; }
        public bool NetworkDeviceDetection { get; set; }

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
    }
}
