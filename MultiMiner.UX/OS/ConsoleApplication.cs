using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MultiMiner.UX.OS
{
    public abstract class ConsoleApplication
    {
        private readonly List<string> commandQueue = new List<string>();

        private bool quitApplication;
        private int oldWindowHeight;
        private int oldWindowWidth;
        private int commandIndex = -1;
        private ConsoleColor initialForegroundColor;
        private ConsoleColor initialBackgroundColor;

        protected bool ScreenDirty { get; set; }
        protected string CurrentInput { get; set; }

        protected abstract void SetupApplication();
        protected abstract void LoadSettings();
        protected abstract void StartupApplication();
        protected abstract void RenderScreen();
        protected abstract void RenderInput();
        protected abstract bool HandleCommandInput(string input);
        protected abstract void HandleInputCanceled();
        protected abstract void HandleScreenNavigation(bool pageUp);
        protected abstract void SaveSettings();
        protected abstract void TearDownApplication();

        public ConsoleApplication()
        {
            CurrentInput = String.Empty;
        }

        public void Run()
        {
            BackupColors();

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                quitApplication = true;  //exit MainLoop()
                e.Cancel = true;         //prevent so we can quit gracefully in MainLoop()
            };
            
            SetupApplication();

            LoadSettings();

            StartupApplication();

            MainLoop();

            SaveSettings();

            TearDownApplication();

            //otherwise artifacts are left on-screen
            Console.Clear();

            RestoreColors();
        }

        private void RestoreColors()
        {
            Console.ForegroundColor = initialForegroundColor;
            Console.BackgroundColor = initialBackgroundColor;
        }

        private void BackupColors()
        {
            initialForegroundColor = Console.ForegroundColor;
            initialBackgroundColor = Console.BackgroundColor;
        }

        protected void Quit()
        {
            quitApplication = true;
        }

        private void MainLoop()
        {
            while (!quitApplication)
            {
                //sleeping longer than 2 does not seem to improve resources but does hurt responiveness
                Thread.Sleep(2);
                HandleInput();
                UpdateScreen();
            }
        }

        private void UpdateScreen()
        {
            if ((oldWindowHeight != Console.WindowHeight) || (oldWindowWidth != Console.WindowWidth))
            {
                Console.Clear();
                ScreenDirty = true;
            }

            if (!ScreenDirty) return;
            ScreenDirty = false;

            oldWindowHeight = Console.WindowHeight;
            oldWindowWidth = Console.WindowWidth;

            RenderScreen();
        }
        
        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (CurrentInput.Length > 0)
                        CurrentInput = CurrentInput.Substring(0, CurrentInput.Length - 1);
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    HandleInputCanceled();
                    CurrentInput = String.Empty;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    if (String.IsNullOrEmpty(CurrentInput)) return;

                    var input = CurrentInput.Trim();
                    CurrentInput = String.Empty;
                    RenderInput();

                    if (HandleCommandInput(input))
                    {
                        if ((commandQueue.Count == 0) || !commandQueue.Last().Equals(input))
                            commandQueue.Add(input);
                        commandIndex = commandQueue.Count - 1;
                    }
                }
                else if ((keyInfo.Key == ConsoleKey.UpArrow) || (keyInfo.Key == ConsoleKey.DownArrow))
                    HandleCommandNavigation(keyInfo.Key == ConsoleKey.UpArrow);
                else if ((keyInfo.Key == ConsoleKey.PageUp) || (keyInfo.Key == ConsoleKey.PageDown))
                    HandleScreenNavigation(keyInfo.Key == ConsoleKey.PageUp);
                else
                {                    
                    var keyChar = keyInfo.KeyChar;
                    //disallow control chars - these can come in over remote terminals e.g. VNC
                    if (!Char.IsControl(keyChar))
                    {
                        //allow mixed-case, e.g. naming entities
                        string key = keyChar.ToString();
                        CurrentInput = CurrentInput + key;
                    }
                }
                RenderInput();
            }
        }

        private void HandleCommandNavigation(bool navigateUp)
        {
            if (navigateUp)
            {
                if (commandIndex >= 0)
                {
                    CurrentInput = commandQueue[commandIndex];
                    if (commandIndex > 0) commandIndex--;
                }
            }
            else
            {
                if (commandIndex < commandQueue.Count - 1)
                {
                    commandIndex++;
                    CurrentInput = commandQueue[commandIndex];
                }
                else
                {
                    commandIndex = commandQueue.Count - 1;
                    CurrentInput = String.Empty;
                }
            }
        }
    }
}
