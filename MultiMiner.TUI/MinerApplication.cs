using MultiMiner.UX.Data;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
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
        private const string PoolCommand = "Pool";
        private const string AddVerb = "Add";
        private const string RemoveVerb = "Remove";

        private const string Ellipsis = "..";

        private readonly ApplicationViewModel app = new ApplicationViewModel();
        private readonly ISynchronizeInvoke threadContext = new SimpleSyncObject();
        private readonly Timer forceDirtyTimer = new Timer(1000);
        private readonly List<NotificationEventArgs> notifications = new List<NotificationEventArgs>();
        private readonly List<string> commandQueue = new List<string>();
        private readonly bool isWindows = Utility.OS.OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix;
        private readonly bool isLinux = Utility.OS.OSVersionPlatform.GetConcretePlatform() == PlatformID.Unix;
        private readonly bool isMac = Utility.OS.OSVersionPlatform.GetConcretePlatform() == PlatformID.MacOSX;

        private bool screenDirty = false;
        private string currentInput = String.Empty;
        private string currentProgress = String.Empty;
        private PromptEventArgs currentPrompt;
        private DateTime promptTime;
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
                notifications.RemoveAll(n => !String.IsNullOrEmpty(n.Id) && n.Id.Equals(e.Id));
                screenDirty = true;
            };

            app.PromptReceived += (object sender, PromptEventArgs e) =>
            {
                currentPrompt = e;
                promptTime = DateTime.Now;
                UpdateScreen(true);
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
            //OutputJunk();
#endif

            OutputDevices();

            OutputSpecial();

            OutputNotifications();

            OutputStatus();

            var output = OutputIncome();

            OutputInput(Console.WindowWidth - output.Length);
        }

        private void OutputJunk()
        {
            for (int row = 0; row < Console.WindowHeight; row++)
            {
                if (SetCursorPosition(0, row))
                    WriteText(new string('X', Console.WindowWidth));
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
                var row = GetSpecialRow() - (recentNotifications.Count - i);
                if (SetCursorPosition(column, row))
                    WriteText(recentNotifications[i].Text.FitLeft(MaxWidth, Ellipsis));
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

        private void OutputInput(int totalWidth)
        {
            const string Prefix = "> ";
            var row = Console.WindowHeight - 1;
            if (SetCursorPosition(0, row))
            {
                //[ERROR] FATAL UNHANDLED EXCEPTION: System.NotImplementedException: The requested feature is not implemented.
                if (isWindows)
                {
                    //http://stackoverflow.com/questions/25084384/filling-last-line-in-console
                    WriteText(" ", ConsoleColor.Gray, ConsoleColor.DarkGray);
                    Console.MoveBufferArea(0, row, 1, 1, Console.WindowWidth - 1, row);
                }

                SetCursorPosition(0, row);
                var width = totalWidth - Prefix.Length - (isWindows ? 1 : 0);
                var text = String.Format("{0}{1}", Prefix, currentInput.TrimStart().FitRight(width, Ellipsis));
                WriteText(text, ConsoleColor.White, ConsoleColor.DarkGray);
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
                var coinSymbol = device.Coin == null ? String.Empty : device.Coin.Id.ShortCoinSymbol();
                var exchange = app.GetExchangeRate(device);
                var pool = device.Pool.DomainFromHost();
                var kind = device.Kind.ToString().First();
                var difficulty = device.Difficulty.ToDifficultyString().Replace(" ", "");

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
                    WriteText(pool.PadFitRight(15, Ellipsis), ConsoleColor.DarkGray);

                var left = 55;
                if (SetCursorPosition(left, i))
                    WriteText(hashrate.FitLeft(10, Ellipsis).PadRight(Console.WindowWidth - left), device.Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray);
            }

            for (int i = devices.Count; i < GetSpecialRow(); i++)
                ClearRow(i);
        }

        private void ClearRow(int row)
        {
            if (SetCursorPosition(0, row))
                WriteText(new string(' ', Console.WindowWidth));
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
                    if (String.IsNullOrEmpty(currentInput)) return;

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
                HandleSwitchAllCommand(input);
            else if (InputMatchesCommand(input, PoolCommand))
                HandlePoolCommand(input);
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

        private void HandleSwitchAllCommand(string input)
        {
            var parts = input.Split(' ');
            if (parts.Count() == 2)
                app.SetAllDevicesToCoin(parts[1], true);
            else
                AddNotification(String.Format("Syntax: {0} symbol", SwitchAllCommand.ToLower()));
        }

        private void HandlePoolCommand(string input)
        {
            var syntax = String.Format("Syntax:{0} {{ add | remove }} symbol url user pass", PoolCommand.ToLower());
            var parts = input.Split(' ');
            if (parts.Count() == 6)
            {
                var verb = parts[1];
                var symbol = parts[2];
                var url = parts[3];
                var user = parts[4];
                var pass = parts[5];

                CoinApi.Data.CoinInformation coin = app.CoinApiInformation.SingleOrDefault(c => c.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
                if (coin == null)
                {
                    AddNotification(String.Format("Unknown coin: {0}", symbol));
                    return; //early exit
                }

                bool add = verb.Equals(AddVerb, StringComparison.OrdinalIgnoreCase);
                bool remove = verb.Equals(RemoveVerb, StringComparison.OrdinalIgnoreCase);
                if (add)
                {
                    Engine.Data.Configuration.Coin coinConfig = app.EngineConfiguration.CoinConfigurations.SingleOrDefault(c => c.PoolGroup.Id.Equals(symbol, StringComparison.OrdinalIgnoreCase));
                    if (coinConfig == null)
                    {
                        coinConfig = new Engine.Data.Configuration.Coin();
                        var algorithm = coin.Algorithm;

                        //we don't want the FullName
                        KnownAlgorithm knownAlgorithm = KnownAlgorithms.Algorithms.SingleOrDefault(a => a.FullName.Equals(algorithm, StringComparison.OrdinalIgnoreCase));
                        if (knownAlgorithm != null) algorithm = knownAlgorithm.Name;

                        coinConfig.PoolGroup = new Engine.Data.PoolGroup
                        {
                            Algorithm = algorithm,
                            Id = coin.Symbol,
                            Kind = coin.Symbol.Contains(':') ? Engine.Data.PoolGroup.PoolGroupKind.MultiCoin : Engine.Data.PoolGroup.PoolGroupKind.SingleCoin,
                            Name = coin.Name
                        };
                        app.EngineConfiguration.CoinConfigurations.Add(coinConfig);
                    }

                    Uri uri = new Uri(url);
                    Xgminer.Data.MiningPool poolConfig = new Xgminer.Data.MiningPool
                    {
                        Host = uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped),
                        Port = uri.Port,
                        Password = pass,
                        Username = user
                    };
                    coinConfig.Pools.Add(poolConfig);
                    app.EngineConfiguration.SaveCoinConfigurations();
                }
                else if (remove)
                {

                }
                else
                {
                    AddNotification(syntax);
                }
            }
            else
                AddNotification(syntax);
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
