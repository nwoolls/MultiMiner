using MultiMiner.Engine.Data.Configuration;
using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace MultiMiner.Remoting.Data.Transfer.Configuration
{
    //do not descend from anything - messes up WCF+Linux+Windows+Mono
    [DataContract]
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
            None = 0,
            SHA256 = 1 << 0,
            Scrypt = 1 << 1
        }

        [DataMember]
        public bool LaunchOnWindowsLogin { get; set; }

        [DataMember]
        public bool StartMiningOnStartup { get; set; }

        [DataMember]
        public int StartupMiningDelay { get; set; }

        [DataMember]
        public bool RestartCrashedMiners { get; set; }

        [DataMember]
        public bool MinimizeToNotificationArea { get; set; }

        [DataMember]
        public bool StartupMinimized { get; set; }

        [DataMember]
        public bool DetectDisownedMiners { get; set; }

        [DataMember]
        public bool Maximized { get; set; }

        [DataMember]
        public bool AutoSetDesktopMode { get; set; }

        [DataMember]
        public Rectangle AppPosition { get; set; }

        [DataMember]
        public bool CheckForMinerUpdates { get; set; }

        [DataMember]
        public bool BriefUserInterface { get; set; }

        [DataMember]
        public ListViewStyle ListViewStyle { get; set; }

        [DataMember]
        public string[] HiddenColumns { get; set; }

        [DataMember]
        public bool ShowApiErrors { get; set; }

        [DataMember]
        public bool UseAccessibleMenu { get; set; }

        [DataMember]
        public bool SetGpuEnvironmentVariables { get; set; }

        [DataMember]
        public int TipsShown { get; set; }

        [DataMember]
        public bool AllowMultipleInstances { get; set; }

        [DataMember]
        public string SubmittedStatsVersion { get; set; }

        [DataMember]
        public bool ShowWorkUtility { get; set; }

        [DataMember]
        public bool MobileMinerMonitoring { get; set; }

        [DataMember]
        public bool MobileMinerRemoteCommands { get; set; }

        [DataMember]
        public string MobileMinerEmailAddress { get; set; }

        [DataMember]
        public string MobileMinerApplicationKey { get; set; }

        [DataMember]
        public bool MobileMinerUsesHttps { get; set; }

        [DataMember]
        public bool MobileMinerPushNotifications { get; set; }

        [DataMember]
        public bool UseCoinWarzApi { get; set; }

        [DataMember]
        public string CoinWarzApiKey { get; set; }

        [DataMember]
        public TimerInterval StrategyCheckInterval { get; set; }

        [DataMember]
        public bool SuggestCoinsToMine { get; set; }

        [DataMember]
        public CoinSuggestionsAlgorithm SuggestionsAlgorithm { get; set; }

        [DataMember]
        public bool LogAreaVisible { get; set; }

        [DataMember]
        public int LogAreaTabIndex { get; set; }

        [DataMember]
        public int LogAreaDistance { get; set; }

        [DataMember]
        public bool RollOverLogFiles { get; set; }

        [DataMember]
        public int OldLogFileSets { get; set; }

        [DataMember]
        public string LogFilePath { get; set; }

        [DataMember]
        public bool ScheduledRestartMining { get; set; }

        [DataMember]
        public TimerInterval ScheduledRestartMiningInterval { get; set; }

        [DataMember]
        public bool NetworkDeviceDetection { get; set; }
    }
}
