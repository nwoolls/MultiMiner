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

        private const string Ellipsis = "..";

        private readonly ApplicationViewModel app = new ApplicationViewModel();
        private readonly ISynchronizeInvoke threadContext = new SimpleSyncObject();
        private readonly List<NotificationEventArgs> notifications = new List<NotificationEventArgs>();
        private readonly List<string> replBuffer = new List<string>();
        private readonly Timer mineOnStartTimer = new Timer(2000);
        private readonly CommandProcessor commandProcessor;
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
        }

        #region ConsoleApplication overrides
        protected override void SetupApplication()
        {
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
                WriteText(versionText, ConsoleColor.White);

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

            screenManager.RegisterScreen(ScreenNames.ApiLog, () =>
            {
                RenderApiLogScreen();
            });
        }

        private void RegisterCommands()
        {
            commandProcessor.RegisterCommand(
                CommandNames.Quit, 
                CommandAliases.Quit,
                String.Empty,
                (input) =>
            {
                Quit();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Start, 
                String.Empty,
                String.Empty,
                (input) =>
            {
                app.StartMining();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Stop, 
                String.Empty,
                String.Empty,
                (input) =>
            {
                app.StopMining();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Restart, 
                String.Empty,
                String.Empty,
                (input) =>
            {
                app.RestartMining();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Scan, 
                String.Empty,
                String.Empty,
                (input) =>
            {
                app.ScanHardwareLocally();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.SwitchAll, 
                CommandAliases.SwitchAll,
                "symbol",
                (input) =>
            {
                if (input.Count() == 2)
                    app.SetAllDevicesToCoin(input[1], true);
                else
                    return false;

                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Pools, 
                CommandAliases.Pools,
                "add|remove|edit|list [symbol|pool#] [url] [user] [pass]",
                HandlePoolCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Screen, 
                CommandAliases.Screen,
                "[main|repl|apilog]",
                (input) =>
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
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.ClearScreen, 
                CommandAliases.ClearScreen,
                String.Empty,
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
                HandleStrategiesCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Notifications, 
                String.Empty,
                "act|remove|clear [note#]",
                HandeNotificationsCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Network, 
                CommandAliases.Network,
                "start|stop|restart|reboot|switch|hide|pin|name ip[:port]|id [pool#|name]", 
                HandeNetworkCommand);

            commandProcessor.RegisterCommand(
                CommandNames.Help, 
                CommandAliases.Help,
                String.Empty,
                (input) =>
            {
                replBuffer.Add(String.Empty);
                commandProcessor.OutputHelp();
                screenManager.SetCurrentScreen(ScreenNames.Repl);
                RenderScreen();
                return true;
            });

            commandProcessor.RegisterCommand(
                CommandNames.Device,
                CommandAliases.Device,
                "enable|switch|name dev_id [symbol|name]",
                HandeDeviceCommand);
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
                    WriteText(line.PadFitRight(Console.WindowWidth, Ellipsis));
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
            if (screenManager.CurrentScreen.Equals(ScreenNames.Main.ToLower())) HandleMainScreenNavigation(pageUp);
            if (screenManager.CurrentScreen.Equals(ScreenNames.Repl.ToLower())) HandleReplScreenNavigation(pageUp);
        }

        private void HandleMainScreenNavigation(bool pageUp)
        {
            var printableHeight = GetSpecialRow() - GetVisibleNotifications().Count;
            var visibleDeviceCount = GetVisibleDevices().Count;

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

            if (!commandProcessor.ProcessCommand(input))
            {
                AddNotification(String.Format("Unknown command: {0}", input.Split(' ').First()));
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
        #endregion
        
        private void OutputNotifications()
        {
            List<NotificationEventArgs> recentNotifications = GetVisibleNotifications();
            for (int i = 0; i < recentNotifications.Count; i++)
            {
                var row = GetSpecialRow() - (recentNotifications.Count - i);
                if (SetCursorPosition(0, row))
                    WriteText(recentNotifications[i].Text.FitLeft(Console.WindowWidth, Ellipsis));
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
            var output = currentProgress.FitRight(Console.WindowWidth, Ellipsis);
            if (SetCursorPosition(0, GetSpecialRow()))
                WriteText(output, ConsoleColor.White, String.IsNullOrEmpty(currentProgress) ? ConsoleColor.Black : ConsoleColor.DarkBlue);
        }

        private void OutputCredentialsPrompt()
        {
            var prompt = String.Format(credentialsPhase == CredentialsPhase.Username ? 
                "SSH username for {0} (ESC to cancel):" : 
                "SSH password for {0} (ESC to cancel):", 
                credentialsTarget.Path);
            var output = prompt.FitRight(Console.WindowWidth, Ellipsis);
            if (SetCursorPosition(0, GetSpecialRow()))
                WriteText(output, ConsoleColor.White, ConsoleColor.DarkBlue);
        }

        private void OutputCurrentPrompt()
        {
            var text = String.Format("{0}: {1}", currentPrompt.Caption, currentPrompt.Text);
            var output = text.FitRight(Console.WindowWidth, Ellipsis);
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
            notifications.Add(new NotificationEventArgs
            {
                Text = text
            });
            replBuffer.Add(text); //so TUI-specific notes show on both screens
            RenderScreen();
        }

        private void OutputDevices()
        {
            List<DeviceViewModel> devices = GetVisibleDevices();

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
                    WriteText(name.PadFitRight(12, Ellipsis), device.Enabled ? device.Kind == Xgminer.Data.DeviceKind.NET || app.MiningEngine.Mining ? ConsoleColor.White : ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(16, row))
                    WriteText(coinSymbol.PadFitRight(8, Ellipsis), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(23, row))
                    WriteText(difficulty.PadFitLeft(8, Ellipsis), ConsoleColor.DarkGray);

                if (SetCursorPosition(31, row))
                    WriteText(exchange.FitCurrency(9).PadLeft(10).PadRight(11), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(42, row))
                    WriteText(pool.PadFitRight(10, Ellipsis), ConsoleColor.DarkGray);

                if (SetCursorPosition(51, row))
                    WriteText(averageHashrate.PadFitLeft(11, Ellipsis), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                if (SetCursorPosition(62, row))
                    WriteText(effectiveHashrate.FitLeft(11, Ellipsis), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);

                var left = 73;
                if (SetCursorPosition(left, row))
                    WriteText(temperature.FitLeft(5, Ellipsis).PadRight(Console.WindowWidth - left), device.Enabled ? ConsoleColor.DarkGray : ConsoleColor.DarkGray);

                row++;
            }

            for (int i = devices.Count; i < GetSpecialRow(); i++)
                ClearRow(i);
        }

        private List<DeviceViewModel> GetVisibleDevices()
        {
            var minerForm = app.GetViewModelToView();
            var devices = minerForm.Devices
                .Where(d => d.Visible)
                .ToList();
            return devices;
        }

        private void ClearRow(int row)
        {
            if (SetCursorPosition(0, row))
                WriteText(new string(' ', Console.WindowWidth));
        }
        
        private bool HandlePoolCommand(string[] input)
        {
            if (input.Count() >= 2)
            {
                var verb = input[1];

                bool add = verb.Equals(ArgumentNames.Add, StringComparison.OrdinalIgnoreCase);
                bool remove = verb.Equals(ArgumentNames.Remove, StringComparison.OrdinalIgnoreCase);
                bool list = verb.Equals(ArgumentNames.List, StringComparison.OrdinalIgnoreCase);
                bool edit = verb.Equals(ArgumentNames.Edit, StringComparison.OrdinalIgnoreCase);

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
                        AddNotification(String.Format("Unknown coin: {0}", symbol));
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

                AddNotification(String.Format("Pool {0} updated", url));
                
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
                        AddNotification(String.Format("Pool {0}:{1} removed", fullPoolList[index].Pool.Host, fullPoolList[index].Pool.Port));
                        return true; //early exit

                    }
                    AddNotification(String.Format("Invalid pool number: {0}", symbol));
                    return true; //early exit
                }
            }
            else if (input.Count() >= 4)
            {
                var url = input[3];
                var user = input.Count() > 4 ? input[4] : String.Empty;

                if (app.RemoveExistingPool(coin.PoolGroup.Id, url, user))
                    AddNotification(String.Format("Pool {0} removed", url));
                else
                    AddNotification(String.Format("Pool {0} not found", url));
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
                
                replBuffer.Add((fullPoolList.IndexOf(p) + 1).ToString().FitLeft(2, Ellipsis) + " "
                    + p.Configuration.PoolGroup.Id.ShortCoinSymbol().PadFitRight(8, Ellipsis) 
                    + builder.Uri.ToString().ShortHostFromHost().PadFitRight(47, Ellipsis)
                    + p.Pool.Username.PadFitRight(20, Ellipsis));
            });

            screenManager.SetCurrentScreen(ScreenNames.Repl);
            RenderScreen();
        }

        private bool HandleStrategiesCommand(string[] input)
        {
            if (input.Count() >= 2)
            {
                var firstArgument = input[1];
                if (firstArgument.Equals(ArgumentNames.On, StringComparison.OrdinalIgnoreCase))
                {
                    app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = true;
                    AddNotification("Auto mining strategies enabled");
                }
                else if (firstArgument.Equals(ArgumentNames.Off, StringComparison.OrdinalIgnoreCase))
                {
                    app.EngineConfiguration.StrategyConfiguration.AutomaticallyMineCoins = false;
                    AddNotification("Auto mining strategies disabled");
                }
                else if (firstArgument.Equals(ArgumentNames.Set, StringComparison.OrdinalIgnoreCase))
                {
                    var lastArgument = input.Last();
                    if (lastArgument.Equals(ArgumentNames.Profit, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Profitability;
                    else if (lastArgument.Equals(ArgumentNames.Diff, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Difficulty;
                    else if (lastArgument.Equals(ArgumentNames.Price, StringComparison.OrdinalIgnoreCase))
                        app.EngineConfiguration.StrategyConfiguration.MiningBasis = Engine.Data.Configuration.Strategy.CoinMiningBasis.Price;
                    else
                        return false; //early exit, wrong syntax

                    AddNotification("Auto mining basis set to " + app.EngineConfiguration.StrategyConfiguration.MiningBasis);
                }
                else
                    return false; //early exit, wrong syntax

                app.EngineConfiguration.SaveStrategyConfiguration();
            }
            else
                return false; //early exit, wrong syntax

            return true;
        }

        private bool HandeNotificationsCommand(string[] input)
        {
            if (input.Count() >= 2)
            {
                var verb = input[1];
                if (verb.Equals(ArgumentNames.Clear, StringComparison.OrdinalIgnoreCase))
                {
                    notifications.Clear();
                    return true; //early exit - success
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
                            if (verb.Equals(ArgumentNames.Remove, StringComparison.OrdinalIgnoreCase))
                            {
                                notifications.RemoveAt(index);
                                return true; //early exit - success
                            }
                            else if (verb.Equals(ArgumentNames.Act, StringComparison.OrdinalIgnoreCase))
                            {
                                notifications[index].ClickHandler();
                                return true; //early exit - success
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool HandeNetworkCommand(string[] input)
        {
            if (input.Count() >= 3)
            {
                var verb = input[1];
                var path = input[2];
                if (!path.Contains(':')) path = path + ":4028";

                var networkDevice = app.LocalViewModel.Devices.SingleOrDefault((d) => d.Visible && d.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
                if (networkDevice == null)
                {
                    networkDevice = GetDeviceById(input[2]);
                    if (networkDevice.Kind != DeviceKind.NET) networkDevice = null;
                }

                if (networkDevice != null)
                {
                    if (verb.Equals(ArgumentNames.Restart, StringComparison.OrdinalIgnoreCase))
                    {
                        app.RestartNetworkDevice(networkDevice);
                        AddNotification(String.Format("Restarting {0}", networkDevice.Path));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(ArgumentNames.Start, StringComparison.OrdinalIgnoreCase))
                    {
                        app.StartNetworkDevice(networkDevice);
                        AddNotification(String.Format("Starting {0}", networkDevice.Path));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(ArgumentNames.Stop, StringComparison.OrdinalIgnoreCase))
                    {
                        app.StopNetworkDevice(networkDevice);
                        AddNotification(String.Format("Stopping {0}", networkDevice.Path));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(ArgumentNames.Reboot, StringComparison.OrdinalIgnoreCase))
                    {
                        if (app.RebootNetworkDevice(networkDevice))
                            AddNotification(String.Format("Rebooting {0}", networkDevice.Path));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(ArgumentNames.Pin, StringComparison.OrdinalIgnoreCase))
                    {
                        bool sticky;
                        app.ToggleNetworkDeviceSticky(networkDevice, out sticky);
                        AddNotification(String.Format("{0} is now {1}", networkDevice.Path, sticky ? "pinned" : "unpinned"));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(ArgumentNames.Hide, StringComparison.OrdinalIgnoreCase))
                    {
                        //current limitations in how .Visible is treated mean we can only hide and not un-hide
                        //hiding means the entry will no longer be in the view model to un-hide
                        //the GUI currently has the same limitation - must unhide via XML
                        bool hidden;
                        app.ToggleNetworkDeviceHidden(networkDevice, out hidden);
                        AddNotification(String.Format("{0} is now {1}", networkDevice.Path, hidden ? "hidden" : "visible"));
                        return true; //early exit - success
                    }
                    else if (input.Count() >= 4)
                    {
                        var lastWords = String.Join(" ", input.Skip(3).ToArray());

                        if (verb.Equals(ArgumentNames.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            app.RenameDevice(networkDevice, lastWords);
                            AddNotification(String.Format("{0} renamed to {1}", networkDevice.Path, lastWords));
                            return true; //early exit - success
                        }
                        else if (verb.Equals(ArgumentNames.Switch, StringComparison.OrdinalIgnoreCase))
                        {
                            var index = -1;
                            if (int.TryParse(lastWords, out index))
                            {
                                index--;
                                if (app.SetNetworkDevicePoolIndex(networkDevice, index))
                                    AddNotification(String.Format("Switching {0} to pool #{1}", networkDevice.Path, lastWords));
                                else
                                    AddNotification(String.Format("Pool #{0} is invalid for {1}", lastWords, networkDevice.Path));

                                return true; //early exit - success
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool HandeDeviceCommand(string[] input)
        {
            if (input.Count() >= 3)
            {
                var verb = input[1];
                var deviceId = input[2];

                var device = GetDeviceById(deviceId);
                
                if (device != null)
                {
                    if (verb.Equals(ArgumentNames.Enable, StringComparison.OrdinalIgnoreCase)
                        //can't enable/disable Network Devices
                        && (device.Kind != DeviceKind.NET))
                    {
                        bool enabled = !device.Enabled;
                        app.ToggleDevices(new List<DeviceDescriptor> { device }, enabled);
                        app.SaveChanges();
                        AddNotification(String.Format("{0} is now {1}",  device.Path, enabled ? "enabled" : "disabled"));
                        return true; //early exit - success
                    }
                    else if (input.Count() >= 4)
                    {                        
                        if (verb.Equals(ArgumentNames.Switch, StringComparison.OrdinalIgnoreCase)
                            //can't enable/disable Network Devices
                            && (device.Kind != DeviceKind.NET))
                        {
                            var symbol = input[3];

                            var configs = app.EngineConfiguration.CoinConfigurations
                                .Where(c => String.IsNullOrEmpty(symbol)
                                    || (c.PoolGroup.Id.Equals(symbol, StringComparison.OrdinalIgnoreCase)
                                    || (c.PoolGroup.Id.ShortCoinSymbol().Equals(symbol, StringComparison.OrdinalIgnoreCase))))
                                .ToList();

                            if (configs.Count > 0)
                            {
                                var coinName = configs.First().PoolGroup.Name;
                                app.SetDevicesToCoin(new List<DeviceDescriptor> { device }, coinName);
                                app.SaveChanges();
                                AddNotification(String.Format("{0} set to {1}: type restart to apply", device.Path, coinName));
                                return true; //early exit - success
                            }
                        }
                        else
                        {
                            var lastWords = String.Join(" ", input.Skip(3).ToArray());

                            if (verb.Equals(ArgumentNames.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                app.RenameDevice(device, lastWords);
                                AddNotification(String.Format("{0} renamed to {1}", device.Path, lastWords));
                                return true; //early exit - success
                            }
                        }
                    }
                }
            }
            return false;
        }

        private DeviceViewModel GetDeviceById(string deviceId)
        {
            var index = -1;
            var valid = int.TryParse(deviceId.Substring(1), out index);

            if (!valid) return null;

            index--;

            var devices = GetVisibleDevices();
            var kindId = deviceId.First().ToString();
            var kindDevices = devices
                .Where(d => d.Kind.ToString().StartsWith(kindId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if ((index >= 0) && (index < kindDevices.Count))
                return kindDevices[index];

            return null;         
        }
    }
}
