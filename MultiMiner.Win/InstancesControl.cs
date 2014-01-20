using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MultiMiner.Discovery;

namespace MultiMiner.Win
{
    public partial class InstancesControl : MessageBoxFontUserControl
    {
        public InstancesControl()
        {
            InitializeComponent();
        }

        public void RegisterInstance(Instance instance)
        {
            string nodeText = instance.MachineName;
            bool isThisPc = instance.MachineName.Equals(Environment.MachineName);
            if (isThisPc)
                nodeText = "This PC";

            TreeNode node = treeView1.Nodes[0].Nodes.Add(instance.IpAddress, nodeText);

            if (isThisPc)
            {
                node.ImageIndex = 4;
                node.SelectedImageIndex = 4;
            }
            else
            {
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;
            }

            treeView1.Nodes[0].ExpandAll();

            if (isThisPc)
            {
                treeView1.SelectedNode = node;
            }
        }

        public void UnregisterInstance(Instance instance)
        {
            treeView1.Nodes[0].Nodes.RemoveByKey(instance.IpAddress);
        }

        public void UnregisterInstances()
        {
            treeView1.Nodes[0].Nodes.Clear();
        }
    }
}
