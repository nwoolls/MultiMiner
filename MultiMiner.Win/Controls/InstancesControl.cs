using MultiMiner.Discovery;
using MultiMiner.Discovery.Data;
using MultiMiner.UX.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MultiMiner.Win.Controls
{
    public partial class InstancesControl : MessageBoxFontUserControl
    {
        private const string ThisPCText = "This PC";
        private const string NetworkText = "Network";

        private class InstanceSorter : IComparer
        {
            // compare between two tree nodes
            public int Compare(object thisObj, object thatObj)
            {
                TreeNode thisNode = thisObj as TreeNode;
                TreeNode thatNode = thatObj as TreeNode;

                if (thisNode.Tag != null)
                    return -1;
                if (thatNode.Tag != null)
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

        private InstanceManager instanceManager;

        private Dictionary<Instance, Remoting.Data.Transfer.Machine> instanceMachines = new Dictionary<Instance, Remoting.Data.Transfer.Machine>();
        private Dictionary<Instance, DateTime> instanceUpdateDates = new Dictionary<Instance, DateTime>();

        public InstancesControl(InstanceManager instanceManager)
        {
            InitializeComponent();
            treeView1.TreeViewNodeSorter = new InstanceSorter();
            this.instanceManager = instanceManager;
        }

        public void RegisterInstance(Instance instance)
        {
            if (instanceManager.Instances.SingleOrDefault(i => i.IpAddress.Equals(instance.IpAddress)) != null)
                //instance already registered
                //added as an additional guard after user reported Sequence error with use of
                //SingleOrDefault in ApplyMachineInformation()
                return;

            string nodeText = instance.MachineName;
            bool isThisPc = instance.MachineName.Equals(Environment.MachineName);

            TreeNode node;

            if (isThisPc)
            {
                instanceManager.ThisPCInstance = instance;
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

            instanceManager.Instances.Add(instance);

            instanceUpdateDates[instance] = DateTime.Now;

            SortTree();

            treeView1.Nodes[0].ExpandAll();

            if (isThisPc)
                treeView1.SelectedNode = node;
        }

        public void UnregisterInstance(Instance instance)
        {
            treeView1.Nodes[0].Nodes.RemoveByKey(instance.IpAddress);
            instanceManager.Instances.Remove(instance);
            instanceMachines.Remove(instance);
            RefreshNetworkTotals();
        }

        public void UnregisterInstances()
        {
            instanceManager.Instances.Clear();
            treeView1.Nodes[0].Nodes.Clear();
            instanceMachines.Clear();
        }

        private string GetMachineName(string ipAddress)
        {
            Instance instance = instanceManager.Instances.Single(i => i.IpAddress.Equals(ipAddress));
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

        public void ApplyMachineInformation(string ipAddress, Remoting.Data.Transfer.Machine machine)
        {
            if (ipAddress.Equals("localhost"))
                ipAddress = instanceManager.ThisPCInstance.IpAddress;

            Instance instance = instanceManager.Instances.SingleOrDefault(i => i.IpAddress.Equals(ipAddress));
            if (instance != null)
            {
                instanceUpdateDates[instance] = DateTime.Now;
                instanceMachines[instance] = machine;
            }

            TreeNode[] nodes = treeView1.Nodes[0].Nodes.Find(ipAddress, false);
            if (nodes.Length > 0)
            {
                if (machine.TotalHashrates.Keys.Count > 0)
                {
                    string text = String.Empty;
                    foreach (string algorithm in machine.TotalHashrates.Keys)
                    {
                        if (!String.IsNullOrEmpty(text))
                            text = text + ", ";
                        text = String.Format("{0}{1}: {2}", text, algorithm, machine.TotalHashrates[algorithm].ToHashrateString());
                    }
                    nodes[0].Text = String.Format("{0} ({1})", GetMachineName(ipAddress), text);
                }                
                else
                {
                    nodes[0].Text = GetMachineName(ipAddress);
                }
            }

            RemoveOrphans();

            RefreshNetworkTotals();
        }

        private void RemoveOrphans()
        {
            //remove instances (not This PC) that haven't broadcast a hashrate in 5 minutes
            IEnumerable<Instance> orphans = instanceManager.Instances.Where(i => (i != instanceManager.ThisPCInstance) && (instanceUpdateDates[i].AddMinutes(5) < DateTime.Now)).ToList();
            foreach (Instance orphan in orphans)
            {
                treeView1.Nodes[0].Nodes.RemoveByKey(orphan.IpAddress);
                instanceManager.Instances.Remove(orphan);
            }
        }

        private void RefreshNetworkTotals()
        {
            Dictionary<string, double> algorithmTotals = new Dictionary<string, double>();

            foreach (KeyValuePair<Instance, Remoting.Data.Transfer.Machine> instanceMachine in instanceMachines)
            {
                Remoting.Data.Transfer.Machine machine = instanceMachine.Value;

                foreach (KeyValuePair<string, double> totalHashrate in machine.TotalHashrates)
                {
                    string algorithm = totalHashrate.Key;
                    double hashrate = totalHashrate.Value;
                    if (hashrate > 0.00)
                    {
                        if (algorithmTotals.ContainsKey(algorithm))
                            hashrate += algorithmTotals[algorithm];
                        algorithmTotals[algorithm] = hashrate;
                    }
                }
            }

            string text = String.Empty;
            foreach (KeyValuePair<string, double> algorithmTotal in algorithmTotals)
            {
                string algorithm = algorithmTotal.Key;
                double hashrate = algorithmTotal.Value;
                if (!String.IsNullOrEmpty(text))
                    text = text + ", ";
                text = String.Format("{0}{1}: {2}", text, algorithm, hashrate.ToHashrateString());
            }

            if (!String.IsNullOrEmpty(text))
                treeView1.Nodes[0].Text = String.Format("{0}, ({1})", NetworkText, text);
            else
                treeView1.Nodes[0].Text = NetworkText;
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
                Instance instance = instanceManager.Instances.SingleOrDefault(i => i.IpAddress.Equals(e.Node.Name));
                if (instance != null)
                {
                    SelectedInstanceChanged(this, instance);
                }
            }
        }
    }
}
