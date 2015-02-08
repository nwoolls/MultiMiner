using MultiMiner.UX.ViewModels;
using System;
using System.Threading;
using System.Timers;

namespace MultiMiner.TUI
{
    //http://broadcast.oreilly.com/2010/08/understanding-c-text-mode-games.html
    class Program
    {
        static void Main(string[] args)
        {
            new MinerApplication().Run();
        }
    }
}
