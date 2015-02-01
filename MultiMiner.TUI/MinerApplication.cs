using MultiMiner.Engine.Installers;
using MultiMiner.TUI.Data;
using MultiMiner.Utility.Async;
using MultiMiner.UX.Data;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.IO;
using MultiMiner.UX.OS;
using MultiMiner.UX.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Timers;

namespace MultiMiner.TUI
{
    class MinerApplication : ConsoleApplication
    {
        private const string Ellipsis = "..";

        private readonly ApplicationViewModel app = new ApplicationViewModel();
        private readonly ISynchronizeInvoke threadContext = new SimpleSyncObject();
        private readonly List<NotificationEventArgs> notifications = new List<NotificationEventArgs>();
        private readonly List<string> replBuffer = new List<string>();
        private readonly Timer mineOnStartTimer = new Timer(2000);
        private readonly CommandProcessor commandProcessor = new CommandProcessor();
        private readonly ScreenManager screenManager = new ScreenManager();

        private readonly bool isWindows = Utility.OS.OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
        private readonly bool isLinux = Utility.OS.OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix;
        private readonly bool isMac = Utility.OS.OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX;

        private string currentProgress = String.Empty;
        private PromptEventArgs currentPrompt;
        private DateTime promptTime;
        private string incomeSummaryText = String.Empty;
        private int replOffset = 0;
        private int screenNameWidth = 0;

        #region ConsoleApplication overrides
        protected override void SetupApplication()
        {
            Console.CursorVisible = false;

            RenderSplashScreen();

            app.DataModified += (object sender, EventArgs e) =>
            {
                ScreenDirty = true;
            };

            app.ConfigurationModified += (object sender, EventArgs e) =>
            {
                ScreenDirty = true;
            };

            app.ProgressStarted += (object sender, ProgressEventArgs e) =>
            {
                currentProgress = e.Text;
                RenderScreen();
            };

            app.ProgressCompleted += (object sender, EventArgs e) =>
            {
                currentProgress = String.Empty;
                RenderScreen();
            };

            app.NotificationReceived += (object sender, NotificationEventArgs e) =>
            {
                notifications.Add(e);
                ScreenDirty = true;
            };

            app.NotificationDismissed += (object sender, NotificationEventArgs e) =>
            {
                notifications.RemoveAll(n => !String.IsNullOrEmpty(n.Id) && n.Id.Equals(e.Id));
                ScreenDirty = true;
            };

            app.PromptReceived += (object sender, PromptEventArgs e) =>
            {
                currentPrompt = e;
                promptTime = DateTime.Now;
                RenderScreen();
            };

            app.Context = threadContext;

            mineOnStartTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                if (app.StartupMiningCountdownSeconds > 0)
                    currentProgress = string.Format("Mining will start automatically in {0} seconds...", app.StartupMiningCountdownSeconds);
                else
                {
                    currentProgress = String.Empty;
                    mineOnStartTimer.Enabled = false;
                }
                RenderScreen();
            };

            RegisterCommands();

            RegisterScreens();
        }

        private void RenderSplashScreen()
        {
            var compileDate = Assembly.GetExecutingAssembly().GetCompileDate();
            var minerVersion = MultiMinerInstaller.GetInstalledMinerVersion();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  _____     _ _   _ _____ _");
            Console.WriteLine(" |     |_ _| | |_|_|     |_|___ ___ ___");
            Console.WriteLine(" | | | | | | |  _| | | | | |   | -_|  _|");
            Console.WriteLine(" |_|_|_|___|_|_| |_|_|_|_|_|_|_|___|_|  ");

            var row = 5;
            if (SetCursorPosition(1, row))
            {
                var versionText = String.Format("{0}", minerVersion);
                var copyrightText = String.Format("(C) 2013-{0} - {1}", compileDate.Year, "http://multiminerapp.com");
                WriteText(versionText, ConsoleColor.White);

                if (SetCursorPosition(versionText.Length + 2, row))
                    WriteText("[", ConsoleColor.DarkGray);

                if (SetCursorPosition(versionText.Length + 3, row))
                    WriteText(copyrightText, ConsoleColor.Gray);

                if (SetCursorPosition(versionText.Length + 3 + copyrightText.Length, row))
                    WriteText("]", ConsoleColor.DarkGray);
            }
        }

        private void RegisterScreens()
        {
            screenManager.RegisterScreen(ScreenNames.Main, () =>
            {
                RenderMainScreen();
            });

            screenManager.RegisterScreen(ScreenNames.Repl, () =>
            {
                RenderReplScreen();
            });

            screenManager.RegisterScreen(ScreenNames.ApiLog, () =>
            {
                RenderApiLogScreen();
            });
        }

        private void RegisterCommands()
        {
            commandProcessor.RegisterCommand(CommandNames.Quit, CommandAliases.Quit, (input) =>
            {
                Quit();
            });

            commandProcessor.RegisterCommand(CommandNames.Start, String.Empty, (input) =>
            {
                app.StartMining();
            });

            commandProcessor.RegisterCommand(CommandNames.Stop, String.Empty, (input) =>
            {
                app.StopMining();
            });

            commandProcessor.RegisterCommand(CommandNames.Restart, String.Empty, (input) =>
            {
                app.RestartMining();
            });

            commandProcessor.RegisterCommand(CommandNames.Scan, String.Empty, (input) =>
            {
                app.ScanHardwareLocally();
            });

            commandProcessor.RegisterCommand(CommandNames.SwitchAll, CommandAliases.SwitchAll, (input) =>
            {
                if (input.Count() == 2)
                    app.SetAllDevicesToCoin(input[1], true);
                else
                    AddNotification(String.Format("{0} <symbol>", CommandNames.SwitchAll.ToLower()));
            });

            commandProcessor.RegisterCommand(CommandNames.Pool, CommandAliases.Pool, HandlePoolCommand);

            commandProcessor.RegisterCommand(CommandNames.Screen, CommandAliases.Screen, (input) =>
            {
                if (input.Count() == 2)
                {
                    if (!screenManager.SetCurrentScreen(input[1]))
                        //unknown screen specified
                        AddNotification(String.Format("unknown screen: {0}", input[1]));
                }
                else
                    screenManager.AdvanceCurrentScreen();
                RenderScreen();
            });

            commandProcessor.RegisterCommand(CommandNames.ClearScreen, CommandAliases.ClearScreen, (input) =>
            {
                replBuffer.Clear();
                RenderScreen();
            });

            commandProcessor.RegisterCommand(CommandNames.Strategies, string.Empty, HandleStrategiesCommand);

            commandProcessor.RegisterCommand(CommandNames.Notifications, string.Empty, HandeNotificationsCommand);

            commandProcessor.RegisterCommand(CommandNames.Network, string.Empty, HandeNetworkCommand);
        }

        protected override void LoadSettings()
        {
            app.ApplicationConfiguration.LoadApplicationConfiguration(app.PathConfiguration.SharedConfigPath);
            app.EngineConfiguration.LoadStrategyConfiguration(app.PathConfiguration.SharedConfigPath); //needed before refreshing coins
            app.EngineConfiguration.LoadCoinConfigurations(app.PathConfiguration.SharedConfigPath); //needed before refreshing coins
            app.LoadSettings();
        }

        protected override void StartupApplication()
        {
            //kill known owned processes to release inherited socket handles
            if (ApplicationViewModel.KillOwnedProcesses())
                //otherwise may still be prompted below by check for disowned miners
                System.Threading.Thread.Sleep(500);

            //check for disowned miners before refreshing devices
            if (app.ApplicationConfiguration.DetectDisownedMiners)
                app.CheckForDisownedMiners();

            app.SetupCoinApi(); //so we target the correct API
            app.RefreshCoinStats();

            app.SetupCoalescedTimers();
            app.UpdateBackendMinerAvailability();
            app.CheckAndDownloadMiners();
            app.SetupRemoting();
            app.SetupNetworkDeviceDetection();
            app.CheckForUpdates();
            app.SetupMiningOnStartup();

            mineOnStartTimer.Enabled = true;
        }

        protected override void TearDownApplication()
        {
            app.StopMiningLocally();
            app.DisableRemoting();
            app.Context = null;
        }
        
        protected override void SaveSettings()
        {
            app.EngineConfiguration.SaveAllConfigurations();
            app.ApplicationConfiguration.SaveApplicationConfiguration();
        }

        protected override void RenderScreen()
        {
            screenManager.RenderScreen();
        }
        
        protected override void RenderInput()
        {
            if (screenManager.CurrentScreen.Equals(ScreenNames.Main.ToLower()))
                OutputInput(Console.WindowWidth - (incomeSummaryText.Length > 0 ? incomeSummaryText.Length : screenNameWidth));
            else
                OutputInput(Console.WindowWidth - screenNameWidth);
        }

        private void RenderApiLogScreen()
        {
            OutputApiLog();

            screenNameWidth = OutputScreenName();

            OutputInput(Console.WindowWidth - screenNameWidth);
        }

        private void OutputApiLog()
        {
            var printableHeight = Console.WindowHeight - 1;
            List<ApiLogEntry> logEntries = GetVisibleApiLogEntries(printableHeight);
            var offset = printableHeight - logEntries.Count;

            for (int i = 0; i < offset; i++)
                ClearRow(i);

            for (int i = 0; i < logEntries.Count; i++)
            {
                var logEntry = logEntries[i];

                var line = logEntry.Machine.PadFitRight(20, Ellipsis)
                        + logEntry.Request.PadFitRight(10, Ellipsis)
                        + logEntry.Response.PadFitRight(Console.WindowWidth - 30, Ellipsis);

                if (SetCursorPosition(0, i + offset))
                    WriteText(logEntry.Machine.PadFitRight(20, Ellipsis), ConsoleColor.White);

                if (SetCursorPosition(20, i + offset))
                    WriteText(logEntry.Request.PadFitRight(10, Ellipsis), ConsoleColor.DarkGray);

                if (SetCursorPosition(30, i + offset))
                    WriteText(logEntry.Response.PadFitRight(Console.WindowWidth - 30, Ellipsis));
            }
        }

        private void RenderReplScreen()
        {
            OutputReplBuffer();

            screenNameWidth = OutputScreenName();

            OutputInput(Console.WindowWidth - screenNameWidth);
        }

        private void OutputReplBuffer()
        {
            var printableHeight = Console.WindowHeight - 1;
            List<string> lines = GetVisibleReplLines(printableHeight);
            var offset = printableHeight - lines.Count;

            for (int i = 0; i < offset; i++)
                ClearRow(i);

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (SetCursorPosition(0, i + offset))
                    WriteText(line.PadFitRight(Console.WindowWidth + 2, Ellipsis));
            }
        }

        private List<string> GetVisibleReplLines(int printableHeight)
        {
            var lines = replBuffer.ToList();
            lines.Reverse();
            lines.RemoveRange(0, replOffset);
            lines = lines.Take(printableHeight).ToList();
            lines.Reverse();
            return lines;
        }

        private List<ApiLogEntry> GetVisibleApiLogEntries(int printableHeight)
        {
            var entries = app.ApiLogEntries.ToList();
            entries.Reverse();
            entries = entries.Take(printableHeight).ToList();
            entries.Reverse();
            return entries;
        }

        private void RenderMainScreen()
        {
            OutputDevices();

            OutputSpecial();

            OutputNotifications();

            OutputStatus();

            incomeSummaryText = OutputIncome();
            var widthOffset = incomeSummaryText.Length;
            if (widthOffset == 0)
            {
                screenNameWidth = OutputScreenName();
                widthOffset = screenNameWidth;
            }

            //[ERROR] FATAL UNHANDLED EXCEPTION: System.NotImplementedException: The requested feature is not implemented.
            if (isWindows) FillLastCell();
            OutputInput(Console.WindowWidth - widthOffset);
        }

        protected override void HandleScreenNavigation(bool pageUp)
        {
            if (screenManager.CurrentScreen.Equals(ScreenNames.Repl.ToLower())) HandleReplScreenNavigation(pageUp);
        }

        private void HandleReplScreenNavigation(bool pageUp)
        {
            var printableHeight = Console.WindowHeight - 1;

            if (pageUp)
                replOffset = Math.Min(replBuffer.Count - printableHeight, replOffset + printableHeight);
            else
                replOffset = Math.Max(0, replOffset - printableHeight);

            OutputReplBuffer();
        }

        protected override bool HandleCommandInput(string input)
        {
            if (!commandProcessor.ProcessCommand(input))
            {
                AddNotification(String.Format("Unknown command: {0}", input.Split(' ').First()));
                return false; //exit early
            }

            RenderScreen();

            //successful command
            return true;
        }
        #endregion
        
        private void OutputNotifications()
        {
            const int NotificationCount = 5;

            var recentNotifications = notifications.ToList();
            recentNotifications.Reverse();
            recentNotifications = recentNotifications.Take(NotificationCount).ToList();
            recentNotifications.Reverse();
            for (int i = 0; i < recentNotifications.Count; i++)
            {
                var row = GetSpecialRow() - (recentNotifications.Count - i);
                if (SetCursorPosition(0, row))
                    WriteText(recentNotifications[i].Text.FitLeft(Console.WindowWidth, Ellipsis));
            }
        }

        private bool SetCursorPosition(int left, int top)
        {
            if ((left < 0) || (left >= Console.WindowWidth) || (top < 0) || (top >= Console.WindowHeight)) return false;

            Console.SetCursorPosition(left, top);

            return true;
        }

        private int OutputScreenName()
        {
            var screenName = screenManager.CurrentScreen;
            var offset = isWindows ? 1 : 0;
            var printableWidth = Console.WindowHeight - 1;

            if (SetCursorPosition(Console.WindowWidth - offset - screenName.Length - 2, printableWidth))
                WriteText("[", ConsoleColor.Gray, ConsoleColor.DarkGray);

            if (SetCursorPosition(Console.WindowWidth - offset - screenName.Length - 1, printableWidth))
                WriteText(screenName, ConsoleColor.White, ConsoleColor.DarkGray);

            if (SetCursorPosition(Console.WindowWidth - offset - 1, printableWidth))
                WriteText("]", ConsoleColor.Gray, ConsoleColor.DarkGray);

            //return width of printed characters
            return screenName.Length + 2;
        }

        private string OutputIncome()
        {
            var incomeSummary = app.GetIncomeSummaryText();
            if (SetCursorPosition(Console.WindowWidth - (isWindows ? 1 : 0) - incomeSummary.Length, Console.WindowHeight - 1))
            {
                WriteText(incomeSummary, ConsoleColor.White, ConsoleColor.DarkGray);
                return incomeSummary;
            }
            return String.Empty;
        }

        private void WriteText(string text, ConsoleColor foreground = ConsoleColor.Gray, ConsoleColor background = ConsoleColor.Black)
        {
            ConsoleColor oldForeground = Console.ForegroundColor;
            ConsoleColor oldBackground = Console.BackgroundColor;
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(text);
            Console.ForegroundColor = oldForeground;
            Console.BackgroundColor = oldBackground;
            SetCursorPosition(0, 0);
        }

        private void FillLastCell()
        {
            var row = Console.WindowHeight - 1;
            if (SetCursorPosition(0, row))
            {
                //http://stackoverflow.com/questions/25084384/filling-last-line-in-console
                WriteText(" ", ConsoleColor.Gray, ConsoleColor.DarkGray);
                Console.MoveBufferArea(0, row, 1, 1, Console.WindowWidth - 1, row);
            }
        }

        private void OutputInput(int totalWidth)
        {
            const string Prefix = "> ";
            var row = Console.WindowHeight - 1;
            if (SetCursorPosition(0, row))
            {
                WriteText(Prefix, ConsoleColor.Gray, ConsoleColor.DarkGray);
                if (SetCursorPosition(Prefix.Length, row))
                {
                    var width = totalWidth - Prefix.Length - (isWindows ? 1 : 0);
                    var text = CurrentInput.TrimStart().FitRight(width, Ellipsis);
                    WriteText(text, ConsoleColor.White, ConsoleColor.DarkGray);
                }
            }
        }

        private void OutputStatus()
        {
            const int Part1Width = 16;
            var deviceStatus = String.Format("{0} device(s)", app.GetVisibleDeviceCount()).FitRight(Part1Width, Ellipsis);
            var hashrateStatus = app.GetHashRateStatusText().Replace("   ", " ").FitLeft(Console.WindowWidth - deviceStatus.Length, Ellipsis);
            if (SetCursorPosition(0, Console.WindowHeight - 2))
            {
                var text = String.Format("{0}{1}", deviceStatus, hashrateStatus);
                WriteText(text, ConsoleColor.White, ConsoleColor.DarkGray);
            }
        }
        
        private void OutputSpecial()
        {
            var output = String.Empty;
            if (currentPrompt != null)
            {
                if ((DateTime.Now - promptTime).TotalSeconds > 30)
                    currentPrompt = null;
                else
                {
                    var text = String.Format("{0}: {1}", currentPrompt.Caption, currentPrompt.Text);
                    output = text.FitRight(Console.WindowWidth, Ellipsis);
                    if (SetCursorPosition(0, GetSpecialRow()))
                        WriteText(output, ConsoleColor.White, 
                            currentPrompt.Icon == PromptIcon.Error 
                            ? ConsoleColor.DarkRed 
                            : currentPrompt.Icon == PromptIcon.Warning
                            ? ConsoleColor.DarkYellow : ConsoleColor.DarkBlue);
                    return; //early exit, prompt rendered
                }
            }

            output = currentProgress.FitRight(Console.WindowWidth, Ellipsis);
            if (SetCursorPosition(0, GetSpecialRow()))
                WriteText(output, ConsoleColor.White, String.IsNullOrEmpty(currentProgress) ? ConsoleColor.Black : ConsoleColor.DarkBlue);
        }

        private static int GetSpecialRow()
        {
            return Console.WindowHeight - 3;
        }

        private void AddNotification(string text)
        {
            notifications.Add(new NotificationEventArgs
            {
                Text = text
            });
            RenderScreen();
        }

        private void OutputDevices()
        {
            var minerForm = app.GetViewModelToView();
            var devices = minerForm.Devices
                .Where(d => d.Visible)
                .ToList();

            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                var name = String.IsNullOrEmpty(device.FriendlyName) ? device.Name : device.FriendlyName;
                var averageHashrate = device.AverageHashrate > 0 ? device.AverageHashrate.ToHashrateString().Replace(" ", "") : String.Empty;
                var effectiveHashrate = device.WorkUtility > 0 ? app.WorkUtilityToHashrate(device.WorkUtility).ToHashrateString().Replace(" ", "") : String.Empty;
                var coinSymbol = device.Coin == null ? String.Empty : device.Coin.Id.ShortCoinSymbol();
                var exchange = app.GetExchangeRate(device);
                var pool = device.Pool.DomainFromHost();
                var kind = device.Kind.ToString().First();
                var difficulty = device.Difficulty > 0 ? device.Difficulty.ToDifficultyString().Replace(" ", "") : String.Empty;
                var temp = device.Temperature > 0 ? device.Temperature + "°" : String.Empty;

                if (SetCursorPosition(0, i))
                    WriteText(kind.ToString().PadRight(2), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(2, i))
                    WriteText(name.PadFitRight(12, Ellipsis), device.Enabled ? device.Kind == Xgminer.Data.DeviceKind.NET || app.MiningEngine.Mining ? ConsoleColor.White : ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(14, i))
                    WriteText(coinSymbol.PadFitRight(8, Ellipsis), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(21, i))
                    WriteText(difficulty.PadFitLeft(8, Ellipsis), ConsoleColor.DarkGray);

                if (SetCursorPosition(29, i))
                    WriteText(exchange.FitCurrency(9).PadLeft(10).PadRight(11), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(40, i))
                    WriteText(pool.PadFitRight(10, Ellipsis), ConsoleColor.DarkGray);

                if (SetCursorPosition(49, i))
                    WriteText(averageHashrate.PadFitLeft(11, Ellipsis), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(60, i))
                    WriteText(effectiveHashrate.FitLeft(11, Ellipsis), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                var left = 71;
                if (SetCursorPosition(left, i))
                    WriteText(temp.FitLeft(6, Ellipsis).PadRight(Console.WindowWidth - left), device.Enabled ? ConsoleColor.DarkGray : ConsoleColor.DarkGray);
            }

            for (int i = devices.Count; i < GetSpecialRow(); i++)
                ClearRow(i);
        }

        private void ClearRow(int row)
        {
            if (SetCursorPosition(0, row))
                WriteText(new string(' ', Console.WindowWidth));
        }
        
        private void HandlePoolCommand(string[] input)
        {
            var syntax = String.Format("{0} <add|remove|list> [symbol] [url] [user] [pass]", CommandNames.Pool.ToLower());

            if (input.Count() >= 2)
            {
                var verb = input[1];

                bool add = verb.Equals(CommandNames.Add, StringComparison.OrdinalIgnoreCase);
                bool remove = verb.Equals(CommandNames.Remove, StringComparison.OrdinalIgnoreCase);
                bool list = verb.Equals(CommandNames.List, StringComparison.OrdinalIgnoreCase);

                if (list)
                {
                    var symbol = String.Empty;
                    if (input.Count() >= 3)
                        symbol = input[2];

                    HandlePoolListCommand(symbol);
                }
                else if(input.Count() >= 4)
                {
                    var symbol = input[2];
                    var url = input[3];

                    CoinApi.Data.CoinInformation coin = app.CoinApiInformation.SingleOrDefault(c => c.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
                    if (coin == null)
                    {
                        AddNotification(String.Format("Unknown coin: {0}", symbol));
                        return; //early exit
                    }


                    if (add && (input.Count() == 6))
                    {
                        var user = input[4];
                        var pass = input[5];

                        app.AddNewPool(coin, url, user, pass);
                    }
                    else if (remove)
                    {
                        var user = input.Count() > 4 ? input[4] : String.Empty;

                        app.RemoveExistingPool(coin, url, user);
                    }
                    else
                        AddNotification(syntax);
                }
                else
                    AddNotification(syntax);
            }
            else
                AddNotification(syntax);
        }

        private void HandlePoolListCommand(string symbol)
        {
            var configs = app.EngineConfiguration.CoinConfigurations
                .Where(c => String.IsNullOrEmpty(symbol)
                    || (c.PoolGroup.Id.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                    || (c.PoolGroup.Id.ShortCoinSymbol().Equals(symbol, StringComparison.OrdinalIgnoreCase))));

            var index = 0;
            foreach (var config in configs)
            {
                config.Pools.ForEach((p) =>
                {
                    replBuffer.Add((++index).ToString().FitLeft(2, Ellipsis) + " "
                        + config.PoolGroup.Id.ShortCoinSymbol().PadFitRight(8, Ellipsis) 
                        + p.Host.ShortHostFromHost().PadFitRight(49, Ellipsis)
                        + p.Username.PadFitRight(20, Ellipsis));
                });
            }

            screenManager.SetCurrentScreen(ScreenNames.Repl);
            RenderScreen();
        }

        private void HandleStrategiesCommand(string[] input)
        {
            if (input.Count() >= 2)
            {
                var firstArgument = input[1];
                if (firstArgument.Equals(CommandNames.On, StringComparison.OrdinalIgnoreCase))
                {
                    app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = true;
                    AddNotification("Auto mining strategies enabled");
                }
                else if (firstArgument.Equals(CommandNames.Off, StringComparison.OrdinalIgnoreCase))
                {
                    app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = false;
                    AddNotification("Auto mining strategies disabled");
                }
                else if (firstArgument.Equals(CommandNames.Set, StringComparison.OrdinalIgnoreCase))
                {
                    var lastArgument = input.Last();
                    if (lastArgument.Equals(CommandNames.Profit, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Profitability;
                    else if (lastArgument.Equals(CommandNames.Diff, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Difficulty;
                    else if (lastArgument.Equals(CommandNames.Price, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Price;
                    else
                    {
                        AddNotification(String.Format("{0} <on|off|set> [profit|diff|price]", CommandNames.Strategies.ToLower()));
                        return; //early exit, wrong syntax
                    }
                    AddNotification("Auto mining basis set to " + app.EngineConfiguration.StrategyConfiguration.MiningBasis);
                }
                else
                {
                    AddNotification(String.Format("{0} <on|off|set> [profit|diff|price]", CommandNames.Strategies.ToLower()));
                    return; //early exit, wrong syntax
                }
                app.EngineConfiguration.SaveStrategyConfiguration();
            }
            else
                AddNotification(String.Format("{0} <on|off|set> [profit|diff|price]", CommandNames.Strategies.ToLower()));
        }

        private void HandeNotificationsCommand(string[] input)
        {
            if (input.Count() >= 2)
            {
                var verb = input[1];
                if (verb.Equals(CommandNames.Clear, StringComparison.OrdinalIgnoreCase))
                {
                    notifications.Clear();
                    return; //early exit - success
                }
                else if (input.Count() == 3)
                {
                    var last = input.Last();
                    var index = -1;
                    if (Int32.TryParse(last, out index))
                    {
                        index--; //user enters 1-based
                        if ((index >= 0) && (index < notifications.Count))
                        {
                            if (verb.Equals(CommandNames.Remove, StringComparison.OrdinalIgnoreCase))
                            {
                                notifications.RemoveAt(index);
                                return; //early exit - success
                            }
                            else if (verb.Equals(CommandNames.Act, StringComparison.OrdinalIgnoreCase))
                            {
                                notifications[index].ClickHandler();
                                return; //early exit - success
                            }
                        }
                    }
                }
            }

            AddNotification(String.Format("{0} <act|remove|clear> [note_number]", CommandNames.Notifications.ToLower()));
        }

        private void HandeNetworkCommand(string[] input)
        {
            if (input.Count() >= 3)
            {
                var verb = input[1];
                var path = input[2];
                if (!path.Contains(':')) path = path + ":4028";

                var networkDevice = app.LocalViewModel.Devices.SingleOrDefault((d) => d.Visible && d.Path.Equals(path, StringComparison.OrdinalIgnoreCase));

                if (networkDevice != null)
                {
                    if (verb.Equals(CommandNames.Restart, StringComparison.OrdinalIgnoreCase))
                    {
                        app.RestartNetworkDevice(networkDevice);
                        return; //early exit - success
                    }
                    else if (verb.Equals(CommandNames.Start, StringComparison.OrdinalIgnoreCase))
                    {
                        app.StartNetworkDevice(networkDevice);
                        return; //early exit - success
                    }
                    else if (verb.Equals(CommandNames.Stop, StringComparison.OrdinalIgnoreCase))
                    {
                        app.StopNetworkDevice(networkDevice);
                        return; //early exit - success
                    }
                    else if (verb.Equals(CommandNames.Reboot, StringComparison.OrdinalIgnoreCase))
                    {
                        app.RebootNetworkDevice(networkDevice);
                        return; //early exit - success
                    }
                }
            }

            AddNotification(String.Format("{0} <start|stop|restart|reboot> <ip_address[:port]>", CommandNames.Network.ToLower()));
        }
    }
}
