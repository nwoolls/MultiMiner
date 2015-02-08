using MultiMiner.TUI.Data;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.IO;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.TUI.Commands
{
    class PoolCommand
    {
        private readonly List<string> replBuffer;
        private readonly ApplicationViewModel app;
        private readonly ScreenManager screenManager;
        private readonly Action<string> notificationHandler;

        public PoolCommand(
            ApplicationViewModel app, 
            ScreenManager screenManager,
            Action<string> notificationHandler, 
            List<string> replBuffer)
        {
            this.app = app;
            this.screenManager = screenManager;
            this.notificationHandler = notificationHandler;
            this.replBuffer = replBuffer;
        }

        public bool HandleCommand(string[] input)
        {
            if (input.Count() >= 2)
            {
                var verb = input[1];

                bool add = verb.Equals(CommandArguments.Add, StringComparison.OrdinalIgnoreCase);
                bool remove = verb.Equals(CommandArguments.Remove, StringComparison.OrdinalIgnoreCase);
                bool list = verb.Equals(CommandArguments.List, StringComparison.OrdinalIgnoreCase);
                bool edit = verb.Equals(CommandArguments.Edit, StringComparison.OrdinalIgnoreCase);

                if (list)
                {
                    var symbol = String.Empty;
                    if (input.Count() >= 3)
                        symbol = input[2];

                    HandlePoolListCommand(symbol);
                    return true; //early exit
                }
                else if (remove)
                {
                    if (HandlePoolRemoveCommand(input))
                        return true;
                }
                else if (edit)
                {
                    if ((input.Count() == 6) && HandlePoolEditCommand(input))
                        return true;
                }
                else if (input.Count() >= 3)
                {
                    var symbol = input[2];

                    CoinApi.Data.CoinInformation coin = app.CoinApiInformation.SingleOrDefault(
                        c => c.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                        || c.Symbol.ShortCoinSymbol().Equals(symbol, StringComparison.Ordinal));

                    if (coin == null)
                    {
                        notificationHandler(String.Format("Unknown coin: {0}", symbol));
                        return true; //early exit
                    }
                    else if (input.Count() >= 4)
                    {
                        var url = input[3];

                        if (add && (input.Count() == 6))
                        {
                            var user = input[4];
                            var pass = input[5];

                            app.AddNewPool(coin, url, user, pass);
                            return true; //early exit
                        }
                    }
                }
            }

            return false;
        }

        private bool HandlePoolEditCommand(string[] input)
        {
            var symbol = input[2];
            var url = input[3];
            var user = input[4];

            var coinConfig = app.EngineConfiguration.CoinConfigurations.SingleOrDefault(
                c => c.PoolGroup.Id.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                || c.PoolGroup.Id.ShortCoinSymbol().Equals(symbol, StringComparison.OrdinalIgnoreCase));
            MiningPool poolConfig = null;

            if (coinConfig == null)
            {
                var index = -1;
                if (int.TryParse(symbol, out index))
                {
                    index--;
                    var fullPoolList = GetPoolList();
                    if ((index >= 0) && (index < fullPoolList.Count))
                    {
                        coinConfig = fullPoolList[index].Configuration;
                        poolConfig = fullPoolList[index].Pool;

                    }
                }
            }
            else
                poolConfig = app.FindPoolConfiguration(coinConfig, url, String.Empty);

            if (poolConfig != null)
            {
                var pass = input[5];
                Uri uri = new UriBuilder(url).Uri;

                poolConfig.Host = uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped);
                poolConfig.Port = uri.Port;
                poolConfig.Password = pass;
                poolConfig.Username = user;
                app.EngineConfiguration.SaveCoinConfigurations();

                notificationHandler(String.Format("Pool {0} updated", url));

                return true;
            }

            return false;
        }

        private bool HandlePoolRemoveCommand(string[] input)
        {
            var symbol = input[2];

            var coin = app.EngineConfiguration.CoinConfigurations.SingleOrDefault(
                c => c.PoolGroup.Id.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                || c.PoolGroup.Id.ShortCoinSymbol().Equals(symbol, StringComparison.OrdinalIgnoreCase));

            if (coin == null)
            {
                var index = -1;
                if (int.TryParse(symbol, out index))
                {
                    index--;
                    var fullPoolList = GetPoolList();
                    if ((index >= 0) && (index < fullPoolList.Count))
                    {
                        fullPoolList[index].Configuration.Pools.Remove(fullPoolList[index].Pool);
                        app.EngineConfiguration.SaveCoinConfigurations();
                        notificationHandler(String.Format("Pool {0}:{1} removed", fullPoolList[index].Pool.Host, fullPoolList[index].Pool.Port));
                        return true; //early exit

                    }
                    notificationHandler(String.Format("Invalid pool number: {0}", symbol));
                    return true; //early exit
                }
            }
            else if (input.Count() >= 4)
            {
                var url = input[3];
                var user = input.Count() > 4 ? input[4] : String.Empty;

                if (app.RemoveExistingPool(coin.PoolGroup.Id, url, user))
                    notificationHandler(String.Format("Pool {0} removed", url));
                else
                    notificationHandler(String.Format("Pool {0} not found", url));
                return true; //early exit
            }
            return false;
        }

        private List<PoolListEntry> GetPoolList()
        {
            var result = new List<PoolListEntry>();
            var configs = app.EngineConfiguration.CoinConfigurations;
            if (configs.Count() == 0) return result;

            foreach (var config in configs)
            {
                config.Pools.ForEach((p) =>
                {
                    result.Add(new PoolListEntry
                    {
                        Configuration = config,
                        Pool = p
                    });
                });
            }

            return result;
        }

        private void HandlePoolListCommand(string symbol)
        {
            var fullPoolList = GetPoolList();
            var filteredPoolList = fullPoolList
                .Where(p => string.IsNullOrEmpty(symbol)
                    || (p.Configuration.PoolGroup.Id.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                    || (p.Configuration.PoolGroup.Id.ShortCoinSymbol().Equals(symbol, StringComparison.OrdinalIgnoreCase))))
                .ToList();

            if (filteredPoolList.Count() == 0) return;

            replBuffer.Add(String.Empty);

            filteredPoolList.ForEach((p) =>
            {
                UriBuilder builder = new UriBuilder(p.Pool.Host.Trim());
                builder.Port = p.Pool.Port;

                replBuffer.Add((fullPoolList.IndexOf(p) + 1).ToString().FitLeft(2) + " "
                    + p.Configuration.PoolGroup.Id.ShortCoinSymbol().PadFitRight(8)
                    + builder.Uri.ToString().ShortHostFromHost().PadFitRight(47)
                    + p.Pool.Username.PadFitRight(20));
            });

            screenManager.SetCurrentScreen(ScreenNames.Repl);
        }
    }
}
