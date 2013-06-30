using MultiMiner.Engine.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMiner.Engine
{
    public class MiningEngine
    {
        private List<Process> miningProcesses = new List<Process>();

        public void StartMining(EngineConfiguration engineConfiguration)
        {
            StopMining();


        }

        public void StopMining()
        {
            foreach (Process miningProcess in miningProcesses)
                miningProcess.Kill();

            miningProcesses.Clear();
        }
    }
}
