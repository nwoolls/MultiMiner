using MultiMiner.UX.IO;
using MultiMiner.UX.ViewModels;
using System;
using System.Linq;

namespace MultiMiner.TUI.Commands
{
    class ScreenCommand
    {
        private readonly ApplicationViewModel app;
        private readonly ScreenManager screenManager;
        private readonly Action<string> notificationHandler;

        public ScreenCommand(
            ApplicationViewModel app,
            ScreenManager screenManager,
            Action<string> notificationHandler)
        {
            this.app = app;
            this.screenManager = screenManager;
            this.notificationHandler = notificationHandler;
        }

        public bool HandleCommand(string[] input)
        {
            if (input.Count() == 2)
            {
                if (!screenManager.SetCurrentScreen(input[1]))
                    //unknown screen specified
                    notificationHandler(String.Format("unknown screen: {0}", input[1]));
            }
            else
                screenManager.AdvanceCurrentScreen();
            return true;
        }
    }
}
