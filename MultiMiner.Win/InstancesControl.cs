using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class InstancesControl : MessageBoxFontUserControl
    {
        public InstancesControl()
        {
            InitializeComponent();
        }

        public void RegisterInstance(string ipAddress)
        {
            TreeNode node = treeView1.Nodes[0].Nodes.Add(ipAddress, ipAddress);
            node.ImageIndex = 1;
            node.SelectedImageIndex = 1;
            treeView1.Nodes[0].ExpandAll();
        }

        public void UnregisterInstance(string ipAddress)
        {
            treeView1.Nodes[0].Nodes.RemoveByKey(ipAddress);
        }

        public void UnregisterInstances()
        {
            treeView1.Nodes[0].Nodes.Clear();
        }
    }
}
