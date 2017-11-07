using MultiMiner.TUI.Data;
using MultiMiner.UX.ViewModels;
using System;
using System.Linq;

namespace MultiMiner.TUI.Commands.Settings
{
    class CoinApiCommand
    {
        private readonly ApplicationViewModel app;
        private readonly Action<string> notificationHandler;

        public CoinApiCommand(
            ApplicationViewModel app,
            Action<string> notificationHandler)
        {
            this.app = app;
            this.notificationHandler = notificationHandler;
        }

        public bool HandleCommand(string[] input)
        {
            var success = false;

            if (input.Count() == 2)
            {
                var arg1 = input[1];
                if (arg1.Equals(SettingArguments.CoinWarz, StringComparison.OrdinalIgnoreCase))
                {
                    app.ApplicationConfiguration.UseCoinWarzApi = true;
                    app.ApplicationConfiguration.UseWhatMineApi = false;
                    app.ApplicationConfiguration.UseWhatToMineApi = false;
                    success = true;
                }
                else if (arg1.Equals(SettingArguments.WhatToMine, StringComparison.OrdinalIgnoreCase))
                {
                    app.ApplicationConfiguration.UseCoinWarzApi = false;
                    app.ApplicationConfiguration.UseWhatMineApi = false;
                    app.ApplicationConfiguration.UseWhatToMineApi = true;
                    success = true;
                }

                if (success)
                {
                    app.ApplicationConfiguration.SaveApplicationConfiguration();
                    notificationHandler(String.Format("Preferred Coin API set to: {0}", arg1));
                }
            }

            return success;
        }
    }
}
