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
        private bool perksEnabled = true;
        public bool PerksEnabled
        {
            get
            {
                return perksEnabled;
            }
            set
            {
                perksEnabled = value;
            }
        }

        private bool showExchangeRates = true;
        public bool ShowExchangeRates
        {
            get
            {
                return showExchangeRates;
            }
            set
            {
                showExchangeRates = value;
            }
        }

        private bool showIncomeRates = true;
        public bool ShowIncomeRates
        {
            get
            {
                return showIncomeRates;
            }
            set
            {
                showIncomeRates = value;
            }
        }

        private bool showIncomeInUsd = true;
        public bool ShowIncomeInUsd
        {
            get
            {
                return showIncomeInUsd;
            }
            set
            {
                showIncomeInUsd = value;
            }
        }


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
