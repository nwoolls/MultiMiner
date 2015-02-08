using MultiMiner.TUI.Data;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
using System;
using System.Linq;

namespace MultiMiner.TUI.Commands
{
    class NetworkCommand
    {
        private readonly ApplicationViewModel app;
        private readonly Action<string> notificationHandler;

        public NetworkCommand(
            ApplicationViewModel app,
            Action<string> notificationHandler)
        {
            this.app = app;
            this.notificationHandler = notificationHandler;
        }

        public bool HandleCommand(string[] input)
        {
            if (input.Count() >= 3)
            {
                var verb = input[1];
                var path = input[2];
                if (!path.Contains(':')) path = path + ":4028";

                var networkDevice = app.LocalViewModel.Devices.SingleOrDefault((d) => d.Visible && d.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
                if (networkDevice == null)
                {
                    networkDevice = app.GetDeviceById(input[2]);
                    if (networkDevice.Kind != DeviceKind.NET) networkDevice = null;
                }

                if (networkDevice != null)
                {
                    if (verb.Equals(CommandArguments.Restart, StringComparison.OrdinalIgnoreCase))
                    {
                        app.RestartNetworkDevice(networkDevice);
                        notificationHandler(String.Format("Restarting {0}", networkDevice.Path));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(CommandArguments.Start, StringComparison.OrdinalIgnoreCase))
                    {
                        app.StartNetworkDevice(networkDevice);
                        notificationHandler(String.Format("Starting {0}", networkDevice.Path));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(CommandArguments.Stop, StringComparison.OrdinalIgnoreCase))
                    {
                        app.StopNetworkDevice(networkDevice);
                        notificationHandler(String.Format("Stopping {0}", networkDevice.Path));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(CommandArguments.Reboot, StringComparison.OrdinalIgnoreCase))
                    {
                        if (app.RebootNetworkDevice(networkDevice))
                            notificationHandler(String.Format("Rebooting {0}", networkDevice.Path));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(CommandArguments.Pin, StringComparison.OrdinalIgnoreCase))
                    {
                        bool sticky;
                        app.ToggleNetworkDeviceSticky(networkDevice, out sticky);
                        notificationHandler(String.Format("{0} is now {1}", networkDevice.Path, sticky ? "pinned" : "unpinned"));
                        return true; //early exit - success
                    }
                    else if (verb.Equals(CommandArguments.Hide, StringComparison.OrdinalIgnoreCase))
                    {
                        //current limitations in how .Visible is treated mean we can only hide and not un-hide
                        //hiding means the entry will no longer be in the view model to un-hide
                        //the GUI currently has the same limitation - must unhide via XML
                        bool hidden;
                        app.ToggleNetworkDeviceHidden(networkDevice, out hidden);
                        notificationHandler(String.Format("{0} is now {1}", networkDevice.Path, hidden ? "hidden" : "visible"));
                        return true; //early exit - success
                    }
                    else if (input.Count() >= 4)
                    {
                        var lastWords = String.Join(" ", input.Skip(3).ToArray());

                        if (verb.Equals(CommandArguments.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            app.RenameDevice(networkDevice, lastWords);
                            notificationHandler(String.Format("{0} renamed to {1}", networkDevice.Path, lastWords));
                            return true; //early exit - success
                        }
                        else if (verb.Equals(CommandArguments.Switch, StringComparison.OrdinalIgnoreCase))
                        {
                            var index = -1;
                            if (int.TryParse(lastWords, out index))
                            {
                                index--;
                                if (app.SetNetworkDevicePoolIndex(networkDevice, index))
                                    notificationHandler(String.Format("Switching {0} to pool #{1}", networkDevice.Path, lastWords));
                                else
                                    notificationHandler(String.Format("Pool #{0} is invalid for {1}", lastWords, networkDevice.Path));

                                return true; //early exit - success
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
