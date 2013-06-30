using Microsoft.Win32;
using MultiMiner.Engine.Configuration;
using System;
using System.IO;
using System.Windows.Forms;

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
            ApplyLaunchOnWindowsStartup();
        }

        private void ApplyLaunchOnWindowsStartup()
        {
            RegistryKey registrykey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (LaunchOnWindowsLogin)
                registrykey.SetValue("MultiMiner", Application.ExecutablePath);
            else
                registrykey.DeleteValue("MultiMiner", false);    
        }

        public void LoadApplicationConfiguration()
        {
            ApplicationConfiguration tmp = ConfigurationReaderWriter.ReadConfiguration<ApplicationConfiguration>(DeviceConfigurationsFileName());

            this.LaunchOnWindowsLogin = tmp.LaunchOnWindowsLogin;
            this.StartMiningOnStartup = tmp.StartMiningOnStartup;
            this.StartupMiningDelay = tmp.StartupMiningDelay;
            this.RestartCrashedMiners = tmp.RestartCrashedMiners;
        }
    }
}
