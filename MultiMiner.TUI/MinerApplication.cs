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
        private const string QuitCommand = "quit";
        private const string StartCommand = "start";
        private const string StopCommand = "stop";

        private const string Ellipsis = "..";

        private readonly ApplicationViewModel app = new ApplicationViewModel();
        private readonly ISynchronizeInvoke threadContext = new SimpleSyncObject();
        private readonly Timer forceDirtyTimer = new Timer(1000);
        private readonly List<NotificationEventArgs> notifications = new List<NotificationEventArgs>();
        private bool screenDirty = false;
        private string currentInput = String.Empty;
        private string currentProgress = String.Empty;
        private bool quitApplication = false;
        private int oldWindowHeight = 0;
        private int oldWindowWidth = 0;

        public void Run()
        {
            Console.CursorVisible = false;

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
                screenDirty = true;
            };

            app.ProgressCompleted += (object sender, EventArgs e) =>
            {
                currentProgress = String.Empty;
                screenDirty = true;
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
                // Sleep for a short period
                System.Threading.Thread.Sleep(10);
                
                HandleInput();

                UpdateScreen();
            }
        }

        private void UpdateScreen()
        {
            if (!screenDirty) return;

            if ((oldWindowHeight != Console.WindowHeight) 
                || (oldWindowWidth != Console.WindowWidth))
                Console.Clear();

            oldWindowHeight = Console.WindowHeight;
            oldWindowWidth = Console.WindowWidth;

            OutputDevices();

            OutputNotifications();

            OutputProgress();

            OutputStatus();

            var output = OutputIncome();

            OutputInput(Console.WindowWidth - 1 - output.Length);

            screenDirty = false;
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
                if (SetCursorPosition(Console.WindowWidth - MaxWidth, Console.WindowHeight - (NotificationCount - 1 - i)))
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
            if (SetCursorPosition(0, Console.WindowHeight - 3))
                Console.Write(currentProgress);
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
                    Console.Write(kind);

                if (SetCursorPosition(2, i))
                    Console.Write(name.FitRight(11, Ellipsis));

                if (SetCursorPosition(14, i))
                    Console.Write(coinSymbol.FitRight(5, Ellipsis));

                if (SetCursorPosition(22, i))
                    Console.Write(difficulty.FitLeft(7, Ellipsis));

                if (SetCursorPosition(31, i))
                    Console.Write(exchange.FitCurrency(9));

                if (SetCursorPosition(41, i))
                    Console.Write(pool.FitRight(14, Ellipsis));

                if (SetCursorPosition(56, i))
                    Console.Write(hashrate.FitLeft(10, Ellipsis));
            }
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
                    if (currentInput.Equals(QuitCommand, StringComparison.OrdinalIgnoreCase))
                        quitApplication = true;
                    else if (currentInput.Equals(StartCommand, StringComparison.OrdinalIgnoreCase))
                        app.StartMining();
                    else if (currentInput.Equals(StopCommand, StringComparison.OrdinalIgnoreCase))
                        app.StopMining();

                    currentInput = String.Empty;
                }
                else
                {
                    string key = keyInfo.KeyChar.ToString().ToLower();
                    currentInput = currentInput + key;
                }
            }
        }
    }
}
