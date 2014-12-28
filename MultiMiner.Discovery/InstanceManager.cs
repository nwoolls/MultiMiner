using MultiMiner.Discovery.Data;
using System.Collections.Generic;

namespace MultiMiner.Discovery
{
    public class InstanceManager
    {
        public List<Instance> Instances { get; set; }
        public Instance ThisPCInstance { get; set; }

        public InstanceManager()
        {
            Instances = new List<Instance>();
        }
    }
}
