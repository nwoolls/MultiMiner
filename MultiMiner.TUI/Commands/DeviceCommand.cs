using MultiMiner.TUI.Data;
using MultiMiner.UX.Extensions;
using MultiMiner.UX.ViewModels;
using MultiMiner.Xgminer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.TUI.Commands
{
    class DeviceCommand
    {
        private readonly ApplicationViewModel app;
        private readonly Action<string> notificationHandler;

        public DeviceCommand(
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
                var deviceId = input[2];

                var device = app.GetDeviceById(deviceId);

                if (device != null)
                {
                    if ((verb.Equals(CommandArguments.Enable, StringComparison.OrdinalIgnoreCase) ||
						//allow "device disable" even though not in help - POLA
						verb.Equals(CommandArguments.Disable, StringComparison.OrdinalIgnoreCase))
                        //can't enable/disable Network Devices
                        && (device.Kind != DeviceKind.NET))
                    {
                        bool enabled = !device.Enabled;
                        app.ToggleDevices(new List<DeviceDescriptor> { device }, enabled);
                        app.SaveChanges();
						//don't identify by device.Path (for output) - blank for GPUs
						notificationHandler(String.Format("{0} is now {1}", device.EasyName, enabled ? "enabled" : "disabled"));
                        return true; //early exit - success
                    }
                    else if (input.Count() >= 4)
                    {
                        if (verb.Equals(CommandArguments.Switch, StringComparison.OrdinalIgnoreCase)
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
                                notificationHandler(String.Format("{0} set to {1}: type restart to apply", device.EasyName, coinName));
                                return true; //early exit - success
                            }
                        }
                        else
                        {
                            var lastWords = String.Join(" ", input.Skip(3).ToArray());

                            if (verb.Equals(CommandArguments.Name, StringComparison.OrdinalIgnoreCase))
                            {
								var oldName = device.EasyName;
                                app.RenameDevice(device, lastWords);
                                notificationHandler(String.Format("{0} renamed to {1}", oldName, lastWords));
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
