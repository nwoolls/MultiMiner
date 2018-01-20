using MultiMiner.Engine.Installers;
using MultiMiner.TUI.Data;
using MultiMiner.Utility.Async;
using MultiMiner.UX.Data;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.IO;
using MultiMiner.UX.OS;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
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
        enum CredentialsPhase
        {
            None,
            Username,
            Password
        }

        private readonly ApplicationViewModel app = new ApplicationViewModel();
        private readonly ISynchronizeInvoke threadContext = new SimpleSyncObject();
        private readonly List<NotificationEventArgs> notifications = new List<NotificationEventArgs>();
        private readonly List<string> replBuffer = new List<string>();
        private readonly Timer mineOnStartTimer = new Timer(2000);
        private readonly CommandProcessor commandProcessor;
        private readonly CommandProcessor settingsProcessor;
        private readonly ScreenManager screenManager = new ScreenManager();

        private readonly bool isWindows = Utility.OS.OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
        private readonly bool isLinux = Utility.OS.OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix;
        private readonly bool isMac = Utility.OS.OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX;

        private string currentProgress = String.Empty;
        private PromptEventArgs currentPrompt;
        private DateTime promptTime;
        private string incomeSummaryText = String.Empty;
        private int replOffset = 0;
        private int mainOffset = 0;
        private int screenNameWidth = 0;

        private CredentialsPhase credentialsPhase;
        private DeviceViewModel credentialsTarget;
        private string credentialsUsername;

        public MinerApplication()
        {
            commandProcessor = new CommandProcessor(AddNotification, (s) =>
            {
                replBuffer.Add(s);
            });

            var setCommand = CommandNames.Set.ToLower();
            settingsProcessor = new CommandProcessor((s) =>
            {
                AddNotification(String.Format("{0} {1}", setCommand, s));
            }, 
            (s) =>
            {
                if (s.StartsWith("\t"))
                    replBuffer.Add(s);
                else
                    replBuffer.Add(String.Format("{0} {1}", setCommand, s));
            });
        }

        protected override void SetupApplication()
        {
            RenderSplashScreen();

            app.SetupMiningEngineEvents();
            app.LoadPreviousHistory();

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

            app.CredentialsRequested += (object sender, CredentialsEventArgs e) =>
            {
                e.CredentialsProvided = false;
                credentialsPhase = CredentialsPhase.Username;
                credentialsTarget = app.LocalViewModel.GetNetworkDeviceByFriendlyName(e.ProtectedResource);
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
            RegisterSettings();
        }

        private void RenderSplashScreen()
        {
            Console.CursorVisible = false;
            Console.Clear();

            OutputAboutInfo();
        }

        private const int SplashInfoHeight = 6;
        private void OutputAboutInfo()
        {
            var compileDate = Assembly.GetExecutingAssembly().GetCompileDate();
            var minerVersion = MultiMinerInstaller.GetInstalledMinerVersion();

            var row = 0;

            if (SetCursorPosition(0, row++))
                WriteText("  _____     _ _   _ _____ _".PadRight(Console.WindowWidth), ConsoleColor.Cyan);
            if (SetCursorPosition(0, row++))
                WriteText(" |     |_ _| | |_|_|     |_|___ ___ ___".PadRight(Console.WindowWidth), ConsoleColor.Cyan);
            if (SetCursorPosition(0, row++))
                WriteText(" | | | | | | |  _| | | | | |   | -_|  _|".PadRight(Console.WindowWidth), ConsoleColor.Cyan);
            if (SetCursorPosition(0, row++))
                WriteText(" |_|_|_|___|_|_| |_|_|_|_|_|_|_|___|_|  ".PadRight(Console.WindowWidth), ConsoleColor.Cyan);

            ClearRow(row);
            row++;

            var col = 0;
            if (SetCursorPosition(col, row))
            {
                var versionText = String.Format(" {0}", minerVersion);
                var copyrightText = String.Format("(C) 2013-{0} - {1}", compileDate.Year, "http://multiminerapp.com");
                WriteText(versionText.PadRight(8), ConsoleColor.White);

                col = versionText.Length + 2;
                if (SetCursorPosition(col, row))
                    WriteText("[", ConsoleColor.DarkGray);

                col++;
                if (SetCursorPosition(col, row))
                    WriteText(copyrightText, ConsoleColor.Gray);

                col += copyrightText.Length;
                if (SetCursorPosition(col, row))
                    WriteText("]".PadRight(Console.WindowWidth - col), ConsoleColor.DarkGray);
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

            screenManager.RegisterScreen(ScreenNames.History, () =>
            {
                RenderHistoryScreen();
            });

            screenManager.RegisterScreen(ScreenNames.ApiLog, () =>
            {
                RenderApiLogScreen();
            });

            screenManager.RegisterScreen(ScreenNames.ProcLog, () =>
            {
                RenderProcLogScreen();
            });
        }

        private void RegisterSettings()
        {
            settingsProcessor.RegisterCommand(
                SettingNames.MobileMiner,
                string.Empty,
                "on|off|email|appkey [address|key]",
                new string[]
                {
                    "set mobileminer on",
                    "set mobileminer off",
                    "set mobileminer email user@example.org",
                    "set mobileminer appkey mr8q-lp67-bvt1"
                },
                new Commands.Settings.MobileMinerCommand(app, AddNotification).HandleCommand);

            settingsProcessor.RegisterCommand(
                CommandNames.Help,
                CommandAliases.Help,
                "[setting]",
                new string[]
                {
                    "help",
                    "h mobileminer"
                },
                (input) =>
                {
                    if (input.Count() <= 2)
                    {
                        OutputCommandHelp(settingsProcessor, input);
                        return true;
                    }
                    return false;
                });

            settingsProcessor.RegisterCommand(
                SettingNames.CoinApi,
                string.Empty,
                "coinchoose|coinwarz|whatmine|whattomine",
                new string[]
                {
                    "set coinapi coinwarz",
                    "set coinapi whattomine"
                },
                new Commands.Settings.CoinApiCommand(app, AddNotification).HandleCommand);

            settingsProcessor.RegisterCommand(
                SettingNames.CoinWarz,
                string.Empty,
                "apikey key",
                new string[]
                {
                    "set coinwarz apikey 9602a70905884c4e9609d20b90163408"
                },
                new Commands.Settings.CoinWarzCommand(app, AddNotification).HandleCommand);

            settingsProcessor.RegisterCommand(
                SettingNames.WhatMine,
                string.Empty,
                "apikey key",
                new string[]
                {
                    "set coinwarz whatmine 9602a70905884c4e9609d20b90163408"
                },
                new Commands.Settings.WhatMineCommand(app, AddNotification).HandleCommand);

            settingsProcessor.RegisterCommand(
                SettingNames.Perks,
                string.Empty,
                "on|off|percent [percent]",
                new string[]
                {
                    "set perks on",
                    "set perks percent 2"
                },
                new Commands.Settings.PerksCommand(app, AddNotification).HandleCommand);
        }
        
        private void RegisterCommands()
        {
            commandProcessor.RegisterCommand(
                CommandNames.Quit, 
                CommandAliases.Quit,
                String.Empty,
                new string[] { },
                (input) =>
            {
                Quit();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Start, 
                String.Empty,
                String.Empty,
                new string[] { },
                (input) =>
            {
                app.StartMining();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Stop, 
                String.Empty,
                String.Empty,
                new string[] { },
                (input) =>
            {
                app.StopMining();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Restart, 
                String.Empty,
                String.Empty,
                new string[] { },
                (input) =>
            {
                app.RestartMining();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Scan, 
                String.Empty,
                String.Empty,
                new string[] { },
                (input) =>
            {
                app.ScanHardwareLocally();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.SwitchAll, 
                CommandAliases.SwitchAll,
                "symbol",
                new string[] 
                {
                    "switchall btc",
                    "sa ltc"
                },
                new Commands.SwitchAllCommand(app).HandleCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Pools, 
                CommandAliases.Pools,
                "add|remove|edit|list [symbol|pool#] [url] [user] [pass]",
                new string[]
                {
                    "pools list",
                    "p add btc stratum+tcp://some.pool.com:3333 my.worker pass",
                    "p remove btc some.pool.com:3333",
                    "p remove 3",
                    "p edit 3 stratum+tcp://other.pool.com:3334 my.worker pass"
                },
                new Commands.PoolCommand(app, screenManager, AddNotification, replBuffer).HandleCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Screen, 
                CommandAliases.Screen,
                "[main|repl|history|apilog|proclog]",
                new string[]
                {
                    "screen main",
                    "sc repl",
                    "sc apilog",
                    "sc"
                },
                new Commands.ScreenCommand(app, screenManager, AddNotification).HandleCommand);

            commandProcessor.RegisterCommand(
                CommandNames.ClearScreen, 
                CommandAliases.ClearScreen,
                String.Empty,
                new string[] { },
                (input) =>
            {
                replBuffer.Clear();
                RenderScreen();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Strategies, 
                String.Empty,
                "on|off|set [profit|diff|price]",
                new string[] 
                {
                    "strats on",
                    "strats off",
                    "strats set proffit"
                },
                new Commands.StrategiesCommand(app, AddNotification).HandleCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Notifications, 
                String.Empty,
                "act|remove|clear [note#]",
                new string[] 
                {
                    "notes remove 1",
                    "notes clear",
                    "notes act 2"
                },
                new Commands.NotificationsCommand(notifications).HandleCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Network, 
                CommandAliases.Network,
                "start|stop|restart|reboot|switch|hide|pin|name ip[:port]|id [pool#|name]",
                new string[] 
                {
                    "net start 192.168.0.99",
                    "n stop 192.168.0.199:4029",
                    "n restart n3",
                    "n switch n3 2",
                    "n hide 192.168.0.99",
                    "n name 192.168.0.99 My Network Miner"
                },
                new Commands.NetworkCommand(app, AddNotification).HandleCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Help, 
                CommandAliases.Help,
                "[command]",
                new string[] 
                {
                    "help",
                    "h pool"
                },
                (input) =>
            {
                if (input.Count() <= 2)
                {
                    OutputCommandHelp(commandProcessor, input);
                    return true;
                }
                return false;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Device,
                CommandAliases.Device,
                "enable|switch|name dev_id [symbol|name]",
                new string[] 
                {
                    "dev enable u1",
                    "d switch g1 ltc",
                    "d name u2 My USB Miner"
                },
                new Commands.DeviceCommand(app, AddNotification).HandleCommand);
        }

        private void OutputCommandHelp(CommandProcessor processor, string[] input)
        {
            replBuffer.Add(String.Empty);

            if (input.Count() == 1)
                processor.OutputHelp();
            else
                processor.OutputComamndHelp(input[1]);

            screenManager.SetCurrentScreen(ScreenNames.Repl);
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
            //app.SubmitMultiMinerStatistics();

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

        private void RenderProcLogScreen()
        {
            OutputProcLog();

            screenNameWidth = OutputScreenName();

            OutputInput(Console.WindowWidth - screenNameWidth);
        }

        private void OutputProcLog()
        {
            var printableHeight = Console.WindowHeight - 1;
            List<Xgminer.LogLaunchArgs> logEntries = GetVisibleLogLaunchEntries(printableHeight);
            var offset = printableHeight - logEntries.Count;

            for (int i = 0; i < offset; i++)
                ClearRow(i);

            for (int i = 0; i < logEntries.Count; i++)
            {
                var logEntry = logEntries[i];

                if (SetCursorPosition(0, i + offset))
                    WriteText(logEntry.DateTime.ToReallyShortDateTimeString().PadFitRight(14), ConsoleColor.DarkGray);

                if (SetCursorPosition(14, i + offset))
                    WriteText(logEntry.CoinName.PadFitRight(9), ConsoleColor.White);

                if (SetCursorPosition(23, i + offset))
                    WriteText(System.IO.Path.GetFileName(logEntry.ExecutablePath).PadFitRight(13));

                var lastColWidth = 16;
                var col = 36;
                if (SetCursorPosition(col, i + offset))
                    WriteText(logEntry.Arguments.PadFitRight(Console.BufferWidth - col - lastColWidth), ConsoleColor.DarkGray);

                if (SetCursorPosition(Console.BufferWidth - lastColWidth, i + offset))
                    WriteText(logEntry.Reason.PadFitRight(lastColWidth));
            }
        }

        private void RenderApiLogScreen()
        {
            OutputApiLog();

            screenNameWidth = OutputScreenName();

            OutputInput(Console.WindowWidth - screenNameWidth);
        }

        private void RenderHistoryScreen()
        {
            OutputHistory();

            screenNameWidth = OutputScreenName();

            OutputInput(Console.WindowWidth - screenNameWidth);
        }

        private void OutputHistory()
        {
            var printableHeight = Console.WindowHeight - 1;
            List<Engine.LogProcessCloseArgs> logEntries = GetVisibleLogCloseEntries(printableHeight);
            var offset = printableHeight - logEntries.Count;

            for (int i = 0; i < offset; i++)
                ClearRow(i);

            for (int i = 0; i < logEntries.Count; i++)
            {
                var logEntry = logEntries[i];
                
                if (SetCursorPosition(0, i + offset))
                    WriteText(logEntry.EndDate.ToReallyShortDateTimeString().PadFitRight(14), ConsoleColor.DarkGray);

                if (SetCursorPosition(14, i + offset))
                    WriteText(logEntry.CoinSymbol.ShortCoinSymbol().PadFitRight(8), ConsoleColor.White);

                if (SetCursorPosition(22, i + offset))
                    WriteText(logEntry.StartPrice.ToFriendlyString().PadFitLeft(9) + " ");

                TimeSpan timeSpan = logEntry.EndDate - logEntry.StartDate;
                var duration = String.Format("{0:0.##} min", timeSpan.TotalMinutes);

                if (SetCursorPosition(32, i + offset))
                    WriteText(duration.PadFitRight(11));

                var devicesString = String.Empty;
                if (logEntry.DeviceDescriptors != null)
                    devicesString = GetFormattedDevicesString(logEntry.DeviceDescriptors);

                var col = 43;
                if (SetCursorPosition(col, i + offset))
                    WriteText(devicesString.PadFitRight(Console.WindowWidth - col));
            }
        }

        private static string GetFormattedDevicesString(List<DeviceDescriptor> deviceDescriptors)
        {
            return String.Join(" ", deviceDescriptors.Select(d => d.ToString()).ToArray());
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

                if (SetCursorPosition(0, i + offset))
                    WriteText(logEntry.Machine.PadFitRight(20), ConsoleColor.White);

                if (SetCursorPosition(20, i + offset))
                    WriteText(logEntry.Request.PadFitRight(10), ConsoleColor.DarkGray);

                var col = 30;
                if (SetCursorPosition(col, i + offset))
                    WriteText(logEntry.Response.PadFitRight(Console.WindowWidth - col + 2));
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
            var start = 0;

            if (offset >= SplashInfoHeight)
            {
                OutputAboutInfo();
                start = SplashInfoHeight;
            }

            for (int i = start; i < offset; i++)
                ClearRow(i);

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (String.IsNullOrEmpty(line))
                {
                    ClearRow(i + offset);
                    continue;
                }

                if (SetCursorPosition(0, i + offset))
                    WriteText(":", ConsoleColor.White);
                if (SetCursorPosition(1, i + offset))
                    WriteText(": ", ConsoleColor.DarkGray);

                if (SetCursorPosition(3, i + offset))
                    WriteText(line.PadFitRight(Console.WindowWidth));
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

        private List<Engine.LogProcessCloseArgs> GetVisibleLogCloseEntries(int printableHeight)
        {
            var entries = app.LogCloseEntries.ToList();
            entries.Reverse();
            entries = entries.Take(printableHeight).ToList();
            entries.Reverse();
            return entries;
        }

        private List<Xgminer.LogLaunchArgs> GetVisibleLogLaunchEntries(int printableHeight)
        {
            var entries = app.LogLaunchEntries.ToList();
            entries.Reverse();
            entries = entries.Take(printableHeight).ToList();
            entries.Reverse();
            return entries;
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
            if (screenManager.CurrentScreen.Equals(ScreenNames.Main.ToLower())) HandleMainScreenNavigation(pageUp);
            if (screenManager.CurrentScreen.Equals(ScreenNames.Repl.ToLower())) HandleReplScreenNavigation(pageUp);
        }

        private void HandleMainScreenNavigation(bool pageUp)
        {
            var printableHeight = GetSpecialRow() - GetVisibleNotifications().Count;
            var visibleDeviceCount = app.GetVisibleDevices().Count;

            if (pageUp)
                mainOffset = mainOffset - printableHeight;
            else
                mainOffset = Math.Min(visibleDeviceCount - printableHeight, mainOffset + printableHeight);

            mainOffset = Math.Max(0, mainOffset);

            RenderMainScreen();
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

        protected override void HandleInputCanceled()
        {
            credentialsPhase = CredentialsPhase.None;
            RenderScreen();
        }

        protected override bool HandleCommandInput(string input)
        {
            if (credentialsPhase != CredentialsPhase.None)
            {
                HandleCredentialsInput(input);
                return true;
            }

            var firstWord = input.Split(' ').First();
            if (firstWord.Equals(CommandNames.Set, StringComparison.OrdinalIgnoreCase))
            {
                var setInput = input.Remove(0, CommandNames.Set.Length).Trim();
                if (String.IsNullOrEmpty(setInput))
                    OutputCommandHelp(settingsProcessor, input.Split(' '));
                else
                {
                    firstWord = setInput.Split(' ').First();
                    if (!settingsProcessor.ProcessCommand(setInput))
                    {
                        AddNotification(string.Format("Unknown setting: {0}", firstWord));
                        return false; //exit early
                    }
                }
                return true;
            }

            if (!commandProcessor.ProcessCommand(input))
            {
                AddNotification(string.Format("Unknown command: {0}", firstWord));
                return false; //exit early
            }

            RenderScreen();

            //successful command
            return true;
        }

        private void HandleCredentialsInput(string input)
        {
            if (credentialsPhase == CredentialsPhase.Username)
            {
                credentialsUsername = input;
                credentialsPhase = CredentialsPhase.Password;
                RenderScreen();
            }
            else
            {
                var networkDevice = app.GetNetworkDeviceByPath(credentialsTarget.Path);

                networkDevice.Username = credentialsUsername;
                networkDevice.Password = input;

                //clear prompt & render before rebooting - rebooting currently blocks
                credentialsPhase = CredentialsPhase.None;
                RenderScreen();

                var success = app.RebootNetworkDevice(credentialsTarget);
                
                if (success)
                {
                    AddNotification(String.Format("Rebooting {0}", credentialsTarget.Path));
                    app.NetworkDevicesConfiguration.SaveNetworkDevicesConfiguration();
                }
                else
                {
                    AddNotification(String.Format("Unable to reboot {0}", credentialsTarget.Path));
                    networkDevice.Username = String.Empty;
                    networkDevice.Password = String.Empty;
                }
            }
        }
        
        private void OutputNotifications()
        {
            List<NotificationEventArgs> recentNotifications = GetVisibleNotifications();
            for (int i = 0; i < recentNotifications.Count; i++)
            {
                var row = GetSpecialRow() - (recentNotifications.Count - i);
                if (SetCursorPosition(0, row))
                    WriteText(recentNotifications[i].Text.FitLeft(Console.WindowWidth));
            }
        }

        private List<NotificationEventArgs> GetVisibleNotifications()
        {
            const int NotificationCount = 5;

            var recentNotifications = notifications.ToList();
            recentNotifications.Reverse();
            recentNotifications = recentNotifications.Take(NotificationCount).ToList();
            recentNotifications.Reverse();
            return recentNotifications;
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
                    var text = CurrentInput.TrimStart().FitRight(width);
                    WriteText(text, ConsoleColor.White, ConsoleColor.DarkGray);
                }
            }
        }

        private void OutputStatus()
        {
            const int Part1Width = 16;
            var deviceStatus = String.Format("{0} device(s)", app.GetVisibleDeviceCount()).FitRight(Part1Width);
            var hashrateStatus = app.GetHashRateStatusText().Replace("   ", " ").FitLeft(Console.WindowWidth - deviceStatus.Length);
            if (SetCursorPosition(0, Console.WindowHeight - 2))
            {
                var text = String.Format("{0}{1}", deviceStatus, hashrateStatus);
                WriteText(text, ConsoleColor.White, ConsoleColor.DarkGray);
            }
        }
        
        private void OutputSpecial()
        {
            if (credentialsPhase != CredentialsPhase.None)
            {
                OutputCredentialsPrompt();
                return; //early exit, prompt rendered
            }

            if (currentPrompt != null)
            {
                if ((DateTime.Now - promptTime).TotalSeconds > 30)
                    currentPrompt = null;
                else
                {
                    OutputCurrentPrompt();
                    return; //early exit, prompt rendered
                }
            }

            OutputCurrentProgress();
        }

        private void OutputCurrentProgress()
        {
            var output = currentProgress.FitRight(Console.WindowWidth);
            if (SetCursorPosition(0, GetSpecialRow()))
                WriteText(output, ConsoleColor.White, String.IsNullOrEmpty(currentProgress) ? ConsoleColor.Black : ConsoleColor.DarkBlue);
        }

        private void OutputCredentialsPrompt()
        {
            var prompt = String.Format(credentialsPhase == CredentialsPhase.Username ? 
                "SSH username for {0} (ESC to cancel):" : 
                "SSH password for {0} (ESC to cancel):", 
                credentialsTarget.Path);
            var output = prompt.FitRight(Console.WindowWidth);
            if (SetCursorPosition(0, GetSpecialRow()))
                WriteText(output, ConsoleColor.White, ConsoleColor.DarkBlue);
        }

        private void OutputCurrentPrompt()
        {
            var text = String.Format("{0}: {1}", currentPrompt.Caption, currentPrompt.Text);
            var output = text.FitRight(Console.WindowWidth);
            if (SetCursorPosition(0, GetSpecialRow()))
                WriteText(output, ConsoleColor.White,
                    currentPrompt.Icon == PromptIcon.Error
                    ? ConsoleColor.DarkRed
                    : currentPrompt.Icon == PromptIcon.Warning
                    ? ConsoleColor.DarkYellow : ConsoleColor.DarkBlue);
        }

        private static int GetSpecialRow()
        {
            return Console.WindowHeight - 3;
        }
        
        private void AddNotification(string text)
        {
            notifications.Add(new NotificationEventArgs { Text = text });
            replBuffer.Add(text); //so TUI-specific notes show on both screens
            RenderScreen();
        }

        private void OutputDevices()
        {
            List<DeviceViewModel> devices = app.GetVisibleDevices();

            var kindCounts = new Dictionary<char, int>();

            var firstIndex = 0 + mainOffset;
            var row = 0;

            for (int i = firstIndex; (i < devices.Count) && (row < GetSpecialRow() - GetVisibleNotifications().Count); i++)
            {
                var device = devices[i];
                var name = String.IsNullOrEmpty(device.FriendlyName) ? device.Name : device.FriendlyName;
                var averageHashrate = device.AverageHashrate > 0 ? device.AverageHashrate.ToHashrateString().Replace(" ", "") : String.Empty;
                var effectiveHashrate = device.WorkUtility > 0 ? app.WorkUtilityToHashrate(device.WorkUtility).ToHashrateString().Replace(" ", "") : String.Empty;
                var coinSymbol = device.Coin == null ? String.Empty : device.Coin.Id.ShortCoinSymbol();
                var exchange = app.GetExchangeRate(device);
                var pool = device.Pool.DomainFromHost();
                var kind = device.Kind.ToString().First();
                if (kindCounts.ContainsKey(kind))
                    kindCounts[kind]++;
                else
                    kindCounts[kind] = 1;
                var deviceId = kind + (kindCounts[kind] + firstIndex).ToString();
                var difficulty = device.Difficulty > 0 ? device.Difficulty.ToDifficultyString().Replace(" ", "") : String.Empty;
                var temperature = device.Temperature > 0 ? (int)device.Temperature + "°" : String.Empty;

                if (SetCursorPosition(0, row))
                    WriteText(deviceId.ToString().PadRight(4), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(4, row))
                    WriteText(name.PadFitRight(12), device.Enabled ? device.Kind == Xgminer.Data.DeviceKind.NET || app.MiningEngine.Mining ? ConsoleColor.White : ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(16, row))
                    WriteText(coinSymbol.PadFitRight(8), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(23, row))
                    WriteText(difficulty.PadFitLeft(8), ConsoleColor.DarkGray);

                if (SetCursorPosition(31, row))
                    WriteText(exchange.FitCurrency(9).PadLeft(10).PadRight(11), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(42, row))
                    WriteText(pool.PadFitRight(10), ConsoleColor.DarkGray);

                if (SetCursorPosition(51, row))
                    WriteText(averageHashrate.PadFitLeft(11), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(62, row))
                    WriteText(effectiveHashrate.FitLeft(11), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                var left = 73;
                if (SetCursorPosition(left, row))
                    WriteText(temperature.FitLeft(5).PadRight(Console.WindowWidth - left), device.Enabled ? ConsoleColor.DarkGray : ConsoleColor.DarkGray);

                row++;
            }

            for (int i = devices.Count; i < GetSpecialRow(); i++)
                ClearRow(i);
        }

        private void ClearRow(int row)
        {
            if (SetCursorPosition(0, row))
                WriteText(new string(' ', Console.WindowWidth));
        }
    }
}
