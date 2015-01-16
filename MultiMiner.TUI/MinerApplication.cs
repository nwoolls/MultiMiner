using MultiMiner.UX.Data;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.ViewModels;
using System;
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

        private readonly ApplicationViewModel app = new ApplicationViewModel();
        private bool screenDirty = false;
        private string currentInput = String.Empty;
        private string currentProgress = String.Empty;
        private bool quitApplication = false;
        private readonly ISynchronizeInvoke threadContext = new SimpleSyncObject();
        private readonly Timer forceDirtyTimer = new Timer(1000);

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

            app.Context = threadContext;
            app.ApplicationConfiguration.LoadApplicationConfiguration(app.PathConfiguration.SharedConfigPath);
            app.LoadSettings();
            app.SetupCoalescedTimers();
            app.UpdateBackendMinerAvailability();
            app.CheckAndDownloadMiners();
            app.SetupRemoting();
            app.SetupNetworkDeviceDetection();

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
                System.Threading.Thread.Sleep(100);
                
                HandleInput();

                UpdateScreen();
            }
        }

        private void UpdateScreen()
        {
            if (!screenDirty) return;

            Console.Clear();

            var minerForm = app.GetViewModelToView();
            var devices = minerForm.Devices
                .Where(d => d.Visible)
                .ToList();

            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                var name = String.IsNullOrEmpty(device.FriendlyName) ? device.Name : device.FriendlyName;
                var hashrate = device.CurrentHashrate.ToHashrateString();

                Console.SetCursorPosition(0, i);
                Console.Write(name);

                Console.SetCursorPosition(20, i);
                Console.Write(hashrate);
            }

            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.Write(currentProgress);

            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write("Input: {0}", currentInput);

            screenDirty = false;
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
