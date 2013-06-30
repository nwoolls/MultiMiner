using MultiMiner.Xgminer.Api;
using System.Collections.Generic;
using System.Diagnostics;

namespace MultiMiner.Engine
{
    public class MinerProcess
    {
        public MinerProcess()
        {
            DevicesIndexes = new List<int>();
        }

        public Process Process { get; set; }
        public ApiContext ApiContext { get; set; }
        public List<int> DevicesIndexes { get; set; }
    }
}
