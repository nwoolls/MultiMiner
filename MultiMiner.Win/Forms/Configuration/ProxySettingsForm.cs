using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace MultiMiner.Win.Forms.Configuration
{
    public partial class ProxySettingsForm : MessageBoxFontForm
    {
        private readonly MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration;

        public ProxySettingsForm(MultiMiner.Engine.Data.Configuration.Xgminer minerConfiguration)
        {
            InitializeComponent();

            this.minerConfiguration = minerConfiguration;
            this.proxyDescriptorBindingSource.DataSource = minerConfiguration.StratumProxies;

            proxyListBox.DataSource = proxyDescriptorBindingSource;
            proxyListBox.DisplayMember = "StratumPort";
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void ProxySettingsForm_Load(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void proxyListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            removeProxyButton.Enabled = proxyListBox.SelectedIndex > 0;
        }

        private void proxyListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor proxy = (MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor)e.ListItem;
            e.Value = String.Format("Getwork: {0} / Stratum: {1}", proxy.GetworkPort, proxy.StratumPort);
        }

        private void addProxyButton_Click(object sender, EventArgs e)
        {
            MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor lastProxy = minerConfiguration.StratumProxies.Last();

            MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor newProxy = new MultiMiner.Engine.Data.Configuration.Xgminer.ProxyDescriptor() 
            { 
                GetworkPort = lastProxy.GetworkPort + 1, 
                StratumPort = lastProxy.StratumPort + 1 
            };
            proxyDescriptorBindingSource.Add(newProxy);

            proxyListBox.SelectedIndex = minerConfiguration.StratumProxies.Count - 1;
            getworkPortEdit.Focus();
        }

        private void removeProxyButton_Click(object sender, EventArgs e)
        {
            DialogResult promptResult = MessageBox.Show("Remove the selected proxy configuration?", "Confirm", MessageBoxButtons.YesNo);
            if (promptResult == System.Windows.Forms.DialogResult.Yes)
            {
                proxyDescriptorBindingSource.RemoveAt(proxyListBox.SelectedIndex);
                getworkPortEdit.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/nwoolls/MultiMiner/wiki/Settings#advanced-miner-settings");
        }
    }
}
