using MultiMiner.TUI.Data;
using MultiMiner.UX.ViewModels;
using System;
using System.Linq;

namespace MultiMiner.TUI.Commands
{
    class StrategiesCommand
    {
        private readonly ApplicationViewModel app;
        private readonly Action<string> notificationHandler;

        public StrategiesCommand(
            ApplicationViewModel app,
            Action<string> notificationHandler)
        {
            this.app = app;
            this.notificationHandler = notificationHandler;
        }

        public bool HandleCommand(string[] input)
        {
            if (input.Count() >= 2)
            {
                var firstArgument = input[1];
                if (firstArgument.Equals(CommandArguments.On, StringComparison.OrdinalIgnoreCase))
                {
                    app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = true;
                    notificationHandler("Auto mining strategies enabled");
                }
                else if (firstArgument.Equals(CommandArguments.Off, StringComparison.OrdinalIgnoreCase))
                {
                    app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = false;
                    notificationHandler("Auto mining strategies disabled");
                }
                else if (firstArgument.Equals(CommandArguments.Set, StringComparison.OrdinalIgnoreCase))
                {
                    var lastArgument = input.Last();
                    if (lastArgument.Equals(CommandArguments.Profit, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Profitability;
                    else if (lastArgument.Equals(CommandArguments.Diff, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Difficulty;
                    else if (lastArgument.Equals(CommandArguments.Price, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Price;
                    else
                        return false; //early exit, wrong syntax

                    notificationHandler("Auto mining basis set to " + app.EngineConfiguration.StrategyConfiguration.MiningBasis);
                }
                else
                    return false; //early exit, wrong syntax

                app.EngineConfiguration.SaveStrategyConfiguration();
            }
            else
                return false; //early exit, wrong syntax

            return true;
        }
    }
}
