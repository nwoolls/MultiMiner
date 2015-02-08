using MultiMiner.TUI.Data;
using MultiMiner.UX.ViewModels;
using System;
using System.Linq;

namespace MultiMiner.TUI.Commands.Settings
{
    class MobileMinerCommand
    {
        private readonly ApplicationViewModel app;
        private readonly Action<string> notificationHandler;

        public MobileMinerCommand(
            ApplicationViewModel app,
            Action<string> notificationHandler)
        {
            this.app = app;
            this.notificationHandler = notificationHandler;
        }

        public bool HandleCommand(string[] input)
        {
            var success = false;

            if (input.Count() >= 2)
            {
                var arg1 = input[1];
                if (arg1.Equals(SettingArguments.On, StringComparison.OrdinalIgnoreCase))
                {
                    app.ApplicationConfiguration.MobileMinerMonitoring = true;
                    notificationHandler(String.Format("MobileMiner monitoring set to: {0}", arg1));
                    success = true;
                }
                else if (arg1.Equals(SettingArguments.Off, StringComparison.OrdinalIgnoreCase))
                {
                    app.ApplicationConfiguration.MobileMinerMonitoring = true;
                    notificationHandler(String.Format("MobileMiner monitoring set to: {0}", arg1));
                    success = true;
                }
                else if (input.Count() >= 3)
                {
                    var arg2 = input[1];
                    if (arg1.Equals(SettingArguments.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        app.ApplicationConfiguration.MobileMinerEmailAddress = arg2;
                        notificationHandler(String.Format("MobileMiner email set to: {0}", arg1));
                        success = true;
                    }
                    else if (arg1.Equals(SettingArguments.AppKey, StringComparison.OrdinalIgnoreCase))
                    {
                        app.ApplicationConfiguration.MobileMinerApplicationKey = arg2;
                        notificationHandler(String.Format("MobileMiner appkey set to: {0}", arg1));
                        success = true;
                    }
                }
            }

            if (success)
                app.ApplicationConfiguration.SaveApplicationConfiguration();
            return success;
        }
    }
}
