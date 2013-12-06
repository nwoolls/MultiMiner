using MultiMiner.Utility;
using System;
using System.IO;

namespace MultiMiner.Win.Configuration
{
    public class PerksConfiguration
    {
        public bool PerksEnabled { get; set; }
        public bool ShowExchangeRates { get; set; }
        public bool ShowIncomeRates { get; set; }

        private static string AppDataPath()
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }

        public static string PerksConfigurationFileName()
        {
            return Path.Combine(AppDataPath(), "PerksConfiguration.xml");
        }

        public void SavePerksConfiguration()
        {
            ConfigurationReaderWriter.WriteConfiguration(this, PerksConfigurationFileName());
        }
        
        public void LoadPerksConfiguration()
        {
            PerksConfiguration tmp = ConfigurationReaderWriter.ReadConfiguration<PerksConfiguration>(PerksConfigurationFileName());

            ObjectCopier.CopyObject(tmp, this);
        }
    }
}
