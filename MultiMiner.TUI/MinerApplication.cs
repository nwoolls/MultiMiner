using MultiMiner.UX.Data;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace MultiMiner.TUI
{
    class MinerApplication
    {
        //upper-case chars serve as a command alias, e.g. Quit = q
        private const string QuitCommand = "Quit";
        private const string StartCommand = "start";
        private const string StopCommand = "stop";
        private const string RestartCommand = "restart";
        private const string ScanCommand = "scan";
        private const string SwitchAllCommand = "SwitchAll";

        private const string Ellipsis = "..";

        private readonly ApplicationViewModel app = new ApplicationViewModel();
        private readonly ISynchronizeInvoke threadContext = new SimpleSyncObject();
        private readonly Timer forceDirtyTimer = new Timer(1000);
        private readonly List<NotificationEventArgs> notifications = new List<NotificationEventArgs>();
        private readonly List<string> commandQueue = new List<string>();

        private bool screenDirty = false;
        private string currentInput = String.Empty;
        private string currentProgress = String.Empty;
        private bool quitApplication = false;
        private int oldWindowHeight = 0;
        private int oldWindowWidth = 0;
        private int commandIndex = -1;

        public void Run()
        {
            Console.CursorVisible = false;

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                quitApplication = true;  //exit main loop
                e.Cancel = true;         //prevent the app from quitting so we can close properly
            };

            forceDirtyTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                //so we pick up resized consoles
                screenDirty = true;
            };
            forceDirtyTimer.Enabled = true;

            app.DataModified += (object sender, EventArgs e) =>
            {
                screenDirty = true;
            };

            app.ConfigurationModified += (object sender, EventArgs e) =>
            {
                screenDirty = true;
            };

            app.ProgressStarted += (object sender, ProgressEventArgs e) =>
            {
                currentProgress = e.Text;
                UpdateScreen(true);
            };

            app.ProgressCompleted += (object sender, EventArgs e) =>
            {
                currentProgress = String.Empty;
                UpdateScreen(true);
            };

            app.NotificationReceived += (object sender, NotificationEventArgs e) =>
            {
                notifications.Add(e);
                screenDirty = true;
            };

            app.NotificationDismissed += (object sender, NotificationEventArgs e) =>
            {
                notifications.RemoveAll(n => n.Id.Equals(e.Id));
                screenDirty = true;
            };

            app.Context = threadContext;

            app.ApplicationConfiguration.LoadApplicationConfiguration(app.PathConfiguration.SharedConfigPath);
            app.EngineConfiguration.LoadStrategyConfiguration(app.PathConfiguration.SharedConfigPath); //needed before refreshing coins
            app.EngineConfiguration.LoadCoinConfigurations(app.PathConfiguration.SharedConfigPath); //needed before refreshing coins
            app.LoadSettings();

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

            MainLoop();

            app.Context = null;
            app.ApplicationConfiguration.SaveApplicationConfiguration();
            app.StopMiningLocally();
            app.DisableRemoting();
        }

        private void MainLoop()
        {
            while (!quitApplication)
            {
                System.Threading.Thread.Sleep(10);                
                HandleInput();
                UpdateScreen();
            }
        }

        private void UpdateScreen(bool force = false)
        {
            if (!screenDirty && !force) return;
            screenDirty = false;

            if ((oldWindowHeight != Console.WindowHeight) || (oldWindowWidth != Console.WindowWidth))
                Console.Clear();

            oldWindowHeight = Console.WindowHeight;
            oldWindowWidth = Console.WindowWidth;

#if DEBUG
            OutputJunk();
#endif

            OutputDevices();

            OutputProgress();

            OutputNotifications();

            OutputStatus();

            var output = OutputIncome();

            OutputInput(Console.WindowWidth - 1 - output.Length);
        }

        private void OutputJunk()
        {
            for (int row = 0; row < Console.WindowHeight - 1; row++)
            {
                if (SetCursorPosition(0, row))
                    Console.Write(new string('X', Console.WindowWidth));
            }
        }

        private void OutputNotifications()
        {
            const int NotificationCount = 5;

            var recentNotifications = notifications.ToList();
            recentNotifications.Reverse();
            recentNotifications = recentNotifications.Take(NotificationCount).ToList();
            recentNotifications.Reverse();
            for (int i = 0; i < recentNotifications.Count; i++)
            {
                const int MaxWidth = 55;
                var column = Console.WindowWidth - MaxWidth;
                var row = GetProgressRow() - (recentNotifications.Count - i);
                if (SetCursorPosition(column, row))
                    Console.Write(recentNotifications[i].Text.FitLeft(MaxWidth, Ellipsis));
            }
        }

        private bool SetCursorPosition(int left, int top)
        {
            if ((left < 0) || (left >= Console.WindowWidth) || (top < 0) || (top >= Console.WindowHeight)) return false;

            Console.SetCursorPosition(left, top);

            return true;
        }

        private string OutputIncome()
        {
            var incomeSummary = app.GetIncomeSummaryText();
            if (SetCursorPosition(Console.WindowWidth - 1 - incomeSummary.Length, Console.WindowHeight - 1))
            {
                Console.Write(incomeSummary);
                return incomeSummary;
            }
            return String.Empty;
        }

        private void OutputInput(int totalWidth)
        {
            const string Prefix = "> ";
            if (SetCursorPosition(0, Console.WindowHeight - 1))
                Console.Write("{0}{1}", Prefix, currentInput.TrimStart().FitRight(totalWidth - Prefix.Length - 1, Ellipsis));
        }

        private void OutputStatus()
        {
            const int Part1Width = 16;
            var deviceStatus = String.Format("{0} device(s)", app.GetVisibleDeviceCount()).FitRight(Part1Width, Ellipsis);
            var hashrateStatus = app.GetHashRateStatusText().Replace("   ", " ").FitLeft(Console.WindowWidth - deviceStatus.Length, Ellipsis);
            if (SetCursorPosition(0, Console.WindowHeight - 2))
                Console.Write("{0}{1}", deviceStatus, hashrateStatus);
        }
        
        private void OutputProgress()
        {
            var output = currentProgress.FitRight(Console.WindowWidth, Ellipsis);
            if (SetCursorPosition(0, GetProgressRow()))
                Console.Write(output);
        }

        private static int GetProgressRow()
        {
            return Console.WindowHeight - 3;
        }

        private void AddNotification(string text)
        {
            notifications.Add(new NotificationEventArgs
            {
                Text = text
            });
            screenDirty = true;
            UpdateScreen();
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
                var hashrate = device.CurrentHashrate.ToHashrateString().Replace(" ", "");
                var coinSymbol = device.Coin.Id.ShortCoinSymbol();
                var exchange = app.GetExchangeRate(device);
                var pool = device.Pool.DomainFromHost();
                var kind = device.Kind.ToString().First();
                var difficulty = device.Difficulty.ToDifficultyString().Replace(" ", "");

                if (SetCursorPosition(0, i))
                    Console.Write(kind.ToString().PadRight(2));

                if (SetCursorPosition(2, i))
                    Console.Write(name.PadFitRight(12, Ellipsis));

                if (SetCursorPosition(14, i))
                    Console.Write(coinSymbol.PadFitRight(8, Ellipsis));

                if (SetCursorPosition(21, i))
                    Console.Write(difficulty.PadFitLeft(8, Ellipsis));

                if (SetCursorPosition(29, i))
                    Console.Write(exchange.FitCurrency(9).PadLeft(10).PadRight(11));

                if (SetCursorPosition(40, i))
                    Console.Write(pool.PadFitRight(15, Ellipsis));

                var left = 55;
                if (SetCursorPosition(left, i))
                    Console.Write(hashrate.FitLeft(10, Ellipsis).PadRight(Console.WindowWidth - left));
            }

            for (int i = devices.Count; i < GetProgressRow(); i++)
                ClearRow(i);
        }

        private void ClearRow(int row)
        {
            if (SetCursorPosition(0, row))
                Console.Write(new string(' ', Console.WindowWidth));
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                screenDirty = true;

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (currentInput.Length > 0)
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    currentInput = String.Empty;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var input = currentInput.Trim();
                    currentInput = String.Empty;
                    UpdateScreen();

                    HandleCommandInput(input);
                }
                else if ((keyInfo.Key == ConsoleKey.UpArrow) || (keyInfo.Key == ConsoleKey.DownArrow))
                    HandleCommandNavigation(keyInfo.Key == ConsoleKey.UpArrow);
                else
                {
                    string key = keyInfo.KeyChar.ToString().ToLower();
                    currentInput = currentInput + key;
                }
            }
        }

        private bool InputMatchesCommand(string input, string command)
        {
            var firstWord = input.Split(' ').First();
            var alias = new String(command.Where(c => Char.IsUpper(c)).ToArray());
            return firstWord.Equals(command, StringComparison.OrdinalIgnoreCase)
                || (!String.IsNullOrEmpty(alias) && firstWord.Equals(alias, StringComparison.OrdinalIgnoreCase));
        }

        private void HandleCommandInput(string input)
        {
            if (InputMatchesCommand(input, QuitCommand))
                quitApplication = true;
            else if (InputMatchesCommand(input, StartCommand))
                app.StartMining();
            else if (InputMatchesCommand(input, StopCommand))
                app.StopMining();
            else if (InputMatchesCommand(input, RestartCommand))
                app.RestartMining();
            else if (InputMatchesCommand(input, ScanCommand))
                app.ScanHardwareLocally();
            else if (InputMatchesCommand(input, SwitchAllCommand))
            {
                var parts = input.Split(' ');
                if (parts.Count() == 2)
                    app.SetAllDevicesToCoin(parts[1], true);
                else
                    AddNotification(String.Format("Syntax: quickswitch symbol", input.Split(' ').First()));
            }
            else
            {
                AddNotification(String.Format("Unknown command: {0}", input.Split(' ').First()));
                return; //exit early
            }

            //successful command
            if ((commandQueue.Count == 0) || !commandQueue.Last().Equals(input))
                commandQueue.Add(input);
            commandIndex = commandQueue.Count - 1;
        }

        private void HandleCommandNavigation(bool navigateUp)
        {
            if (navigateUp)
            {
                if (commandIndex >= 0)
                {
                    currentInput = commandQueue[commandIndex];
                    if (commandIndex > 0) commandIndex--;
                }
            }
            else
            {
                if (commandIndex < commandQueue.Count - 1)
                {
                    commandIndex++;
                    currentInput = commandQueue[commandIndex];
                }
                else
                {
                    commandIndex = commandQueue.Count - 1;
                    currentInput = String.Empty;
                }
            }
        }
    }
}
