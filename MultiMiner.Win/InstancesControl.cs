using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MultiMiner.Discovery;
using MultiMiner.Win.Extensions;
using System.Collections;

namespace MultiMiner.Win
{
    public partial class InstancesControl : MessageBoxFontUserControl
    {
        private const string ThisPCText = "This PC";
        public class InstanceSorter : IComparer
        {
            // compare between two tree nodes
            public int Compare(object thisObj, object thatObj)
            {
                TreeNode thisNode = thisObj as TreeNode;
                TreeNode thatNode = thatObj as TreeNode;

                if (thisNode.Tag != null)
                    return -1;
                else if (thatNode.Tag != null)
                    return 1;

                //alphabetically sorting
                return thisNode.Text.CompareTo(thatNode.Text);
            }
        } 

        //events
        //delegate declarations
        public delegate void SelectedInstanceChangedHandler(object sender, Instance instance);

        //event declarations        
        public event SelectedInstanceChangedHandler SelectedInstanceChanged;

        public List<Instance> Instances { get; set; }

        public InstancesControl()
        {
            InitializeComponent();
            Instances = new List<Instance>();
            treeView1.TreeViewNodeSorter = new InstanceSorter();
        }

        public void RegisterInstance(Instance instance)
        {
            string nodeText = instance.MachineName;
            bool isThisPc = instance.MachineName.Equals(Environment.MachineName);

            TreeNode node;

            if (isThisPc)
            {
                nodeText = ThisPCText;
                node = treeView1.Nodes[0].Nodes.Insert(0, instance.IpAddress, nodeText);
            }
            else
            {
                node = treeView1.Nodes[0].Nodes.Add(instance.IpAddress, nodeText);
            }

            if (isThisPc)
            {
                node.Tag = 1;
                node.ImageIndex = 4;
                node.SelectedImageIndex = 4;
            }
            else
            {
                node.Tag = null;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;
            }

            Instances.Add(instance);

            SortTree();

            treeView1.Nodes[0].ExpandAll();

            if (isThisPc)
            {
                treeView1.SelectedNode = node;
            }
        }

        public void UnregisterInstance(Instance instance)
        {
            treeView1.Nodes[0].Nodes.RemoveByKey(instance.IpAddress);
            Instances.Remove(instance);
        }

        public void UnregisterInstances()
        {
            Instances.Clear();
            treeView1.Nodes[0].Nodes.Clear();
        }

        private string GetMachineName(string ipAddress)
        {
            Instance instance = Instances.Single(i => i.IpAddress.Equals(ipAddress));
            return GetMachineName(instance);
        }

        private string GetMachineName(Instance instance)
        {
            string result = instance.MachineName;
            bool isThisPc = result.Equals(Environment.MachineName);
            if (isThisPc)
                result = ThisPCText;
            return result;
        }

        public void ApplyMachineInformation(string ipAddress, Remoting.Server.Data.Transfer.Machine machine)
        {
            if (ipAddress.Equals("localhost"))
                ipAddress = Instances.Single(i => i.MachineName.Equals(Environment.MachineName)).IpAddress;

            TreeNode[] nodes = treeView1.Nodes[0].Nodes.Find(ipAddress, false);
            if (nodes.Length > 0)
            {
                if ((machine.TotalSha256Hashrate > 0) && (machine.TotalScryptHashrate > 0))
                {
                    nodes[0].Text = String.Format("{0} ({1}, {2})",
                        GetMachineName(ipAddress),
                        machine.TotalSha256Hashrate.ToHashrateString(),
                        machine.TotalScryptHashrate.ToHashrateString());
                }
                else if (machine.TotalSha256Hashrate > 0)
                {
                    nodes[0].Text = String.Format("{0} ({1})",
                        GetMachineName(ipAddress),
                        machine.TotalSha256Hashrate.ToHashrateString());
                }
                else if (machine.TotalScryptHashrate > 0)
                {
                    nodes[0].Text = String.Format("{0} ({1})",
                        GetMachineName(ipAddress),
                        machine.TotalScryptHashrate.ToHashrateString());
                }
                else
                {
                    nodes[0].Text = GetMachineName(ipAddress);
                }
            }
        }

        private void SortTree()
        {
            treeView1.BeginUpdate();
            try
            {
            string key = treeView1.SelectedNode == null ? null : treeView1.SelectedNode.Name;
            treeView1.Sort();
            if (!String.IsNullOrEmpty(key))
                treeView1.SelectedNode = treeView1.Nodes[0].Nodes.Find(key, false).First();
            }
            finally
            {
                treeView1.EndUpdate();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (SelectedInstanceChanged != null)
            {
                Instance instance = Instances.SingleOrDefault(i => i.IpAddress.Equals(e.Node.Name));
                if (instance != null)
                {
                    SelectedInstanceChanged(this, instance);
                }
            }
        }
    }
}
