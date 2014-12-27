using MultiMiner.Engine;
using MultiMiner.Utility.Serialization;
using System;
using System.IO;
using System.Xml.Serialization;

namespace MultiMiner.UX.Data.Configuration
{
    [XmlType(TypeName = "PerksConfiguration")]
    public class Perks
    {
        public bool PerksEnabled { get; set; }
        public bool ShowExchangeRates { get; set; }
        public bool ShowIncomeRates { get; set; }
        public bool ShowIncomeInUsd { get; set; }
        public bool EnableRemoting { get; set; }
        public string RemotingPassword { get; set; }
        public bool AdvancedProxying { get; set; }

        private int donationPercent = 1;
        public int DonationPercent
        {
            get
            {
                return donationPercent;
            }
            set
            {
                donationPercent = Math.Max(1, Math.Min(100, value));
            }
        }

        private string configDirectory;
        public string PerksConfigurationFileName()
        {
            return Path.Combine(configDirectory, "PerksConfiguration.xml");
        }

        public void SavePerksConfiguration(string configDirectory = null)
        {
            InitializeConfigDirectory(configDirectory);

            ConfigurationReaderWriter.WriteConfiguration(this, PerksConfigurationFileName());
        }

        private void InitializeConfigDirectory(string configDirectory)
        {
            if (!String.IsNullOrEmpty(configDirectory))
                this.configDirectory = configDirectory;
            else if (String.IsNullOrEmpty(this.configDirectory))
                this.configDirectory = ApplicationPaths.AppDataPath();
        }

        public void LoadPerksConfiguration(string configDirectory)
        {
            InitializeConfigDirectory(configDirectory);

            Perks tmp = ConfigurationReaderWriter.ReadConfiguration<Perks>(PerksConfigurationFileName());

            ObjectCopier.CopyObject(tmp, this);
        }
    }
}
