using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MultiMiner.Discovery;
using MultiMiner.Win.Extensions;
using System.Collections;
using MultiMiner.Utility.Forms;

namespace MultiMiner.Win.Controls
{
    public partial class InstancesControl : MessageBoxFontUserControl
    {
        private const string ThisPCText = "This PC";
        private const string NetworkText = "Network";

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

        private Dictionary<Instance, Remoting.Server.Data.Transfer.Machine> instanceMachines = new Dictionary<Instance, Remoting.Server.Data.Transfer.Machine>();

        public InstancesControl()
        {
            InitializeComponent();
            Instances = new List<Instance>();
            treeView1.TreeViewNodeSorter = new InstanceSorter();
        }

        public void RegisterInstance(Instance instance)
        {
            if (Instances.SingleOrDefault(i => i.IpAddress.Equals(instance.IpAddress)) != null)
                //instance already registered
                //added as an additional guard after user reported Sequence error with use of
                //SingleOrDefault in ApplyMachineInformation()
                return;

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
                treeView1.SelectedNode = node;
        }

        public void UnregisterInstance(Instance instance)
        {
            treeView1.Nodes[0].Nodes.RemoveByKey(instance.IpAddress);
            Instances.Remove(instance);
            instanceMachines.Remove(instance);
            RefreshNetworkTotals();
        }

        public void UnregisterInstances()
        {
            Instances.Clear();
            treeView1.Nodes[0].Nodes.Clear();
            instanceMachines.Clear();
        }

        private string GetMachineName(string ipAddress)
        {
            Instance instance = Instances.Single(i => i.IpAddress.Equals(ipAddress));
            return GetMachineName(instance);
        }

        private static string GetMachineName(Instance instance)
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

            Instance instance = Instances.SingleOrDefault(i => i.IpAddress.Equals(ipAddress));
            if (instance != null)
                instanceMachines[instance] = machine;

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

            RefreshNetworkTotals();
        }

        private void RefreshNetworkTotals()
        {
            double totalScrypt = instanceMachines.Values.Sum(m => m.TotalScryptHashrate);
            double totalSha256 = instanceMachines.Values.Sum(m => m.TotalSha256Hashrate);

            if ((totalSha256 > 0) && (totalScrypt > 0))
            {
                treeView1.Nodes[0].Text = String.Format("{0} ({1}, {2})",
                    NetworkText,
                    totalSha256.ToHashrateString(),
                    totalScrypt.ToHashrateString());
            }
            else if (totalSha256 > 0)
            {
                treeView1.Nodes[0].Text = String.Format("{0} ({1})",
                    NetworkText,
                    totalSha256.ToHashrateString());
            }
            else if (totalScrypt > 0)
            {
                treeView1.Nodes[0].Text = String.Format("{0} ({1})",
                    NetworkText,
                    totalScrypt.ToHashrateString());
            }
            else
            {
                treeView1.Nodes[0].Text = NetworkText;
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
                {
                    TreeNode[] foundNodes = treeView1.Nodes[0].Nodes.Find(key, false);
                    if (foundNodes.Length > 0)
                        treeView1.SelectedNode = foundNodes.First();
                }
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
