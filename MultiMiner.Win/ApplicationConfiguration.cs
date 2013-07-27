using MultiMiner.Engine.Configuration;
using MultiMiner.Xgminer;
using System;
using System.IO;

namespace MultiMiner.Win
{
    public class ApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            this.StartupMiningDelay = 45;
        }

        public bool LaunchOnWindowsLogin { get; set; }
        public bool StartMiningOnStartup { get; set; }
        public int StartupMiningDelay { get; set; }
        public bool RestartCrashedMiners { get; set; }
        public bool MinimizeToNotificationArea { get; set; }
        public bool DetectDisownedMiners { get; set; }
        public bool Maximized { get; set; }
        public bool LogAreaVisible { get; set; }

        public bool MobileMinerMonitoring { get; set; }
        public bool MobileMinerRemoteCommands { get; set; }
        public string MobileMinerEmailAddress { get; set; }
        public string MobileMinerApplicationKey { get; set; }

        private static string AppDataPath()
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }

        private static string DeviceConfigurationsFileName()
        {
            return Path.Combine(AppDataPath(), "ApplicationConfiguration.xml");
        }

        public void SaveApplicationConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(this, DeviceConfigurationsFileName());

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

        public void LoadApplicationConfiguration()
        {
            ApplicationConfiguration tmp = ConfigurationReaderWriter.ReadConfiguration<ApplicationConfiguration>(DeviceConfigurationsFileName());

            this.LaunchOnWindowsLogin = tmp.LaunchOnWindowsLogin;
            this.StartMiningOnStartup = tmp.StartMiningOnStartup;
            this.StartupMiningDelay = tmp.StartupMiningDelay;
            this.RestartCrashedMiners = tmp.RestartCrashedMiners;
            this.MinimizeToNotificationArea = tmp.MinimizeToNotificationArea;
            this.DetectDisownedMiners = tmp.DetectDisownedMiners;
            this.Maximized = tmp.Maximized;
            this.LogAreaVisible = tmp.LogAreaVisible;

            this.MobileMinerMonitoring = tmp.MobileMinerMonitoring;
            this.MobileMinerRemoteCommands = tmp.MobileMinerRemoteCommands;
            this.MobileMinerEmailAddress = tmp.MobileMinerEmailAddress;
            this.MobileMinerApplicationKey = tmp.MobileMinerApplicationKey;
        }
    }
}
