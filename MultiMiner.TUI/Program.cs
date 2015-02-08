using System;
using System.Diagnostics;

namespace MultiMiner.TUI
{
    //http://broadcast.oreilly.com/2010/08/understanding-c-text-mode-games.html
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            new MinerApplication().Run();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry("Application Error", (e.ExceptionObject as Exception).ToString(), EventLogEntryType.Error, 1000);
        }
    }
}
