using MultiMiner.TUI.Data;
using MultiMiner.UX.ViewModels;
using System;
using System.Linq;

namespace MultiMiner.TUI.Commands.Settings
{
    class PerksCommand
    {
        private readonly ApplicationViewModel app;
        private readonly Action<string> notificationHandler;

        public PerksCommand(
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
                    app.PerksConfiguration.PerksEnabled = true;
                    app.PerksConfiguration.ShowExchangeRates = true;
                    app.PerksConfiguration.ShowIncomeInUsd = true;
                    app.PerksConfiguration.ShowIncomeRates = true;
                    notificationHandler(String.Format("Perks set to: {0}", arg1));
                    success = true;
                }
                else if (arg1.Equals(SettingArguments.Off, StringComparison.OrdinalIgnoreCase))
                {
                    app.PerksConfiguration.PerksEnabled = false;
                    notificationHandler(String.Format("Perks set to: {0}", arg1));
                    success = true;
                }
                else if (input.Count() == 3)
                {
                    var arg2 = input[2];
                    var percent = 1;

                    if (arg1.Equals(SettingArguments.Percent, StringComparison.OrdinalIgnoreCase)
                        && int.TryParse(arg2, out percent)
                        && (percent >= 1))
                    {
                        notificationHandler(String.Format("Perks donation percent set to: {0}", arg2));
                        success = true;
                    }
                }

                if (success)
                    app.PerksConfiguration.SavePerksConfiguration();
            }
            return success;
        }
    }
}
