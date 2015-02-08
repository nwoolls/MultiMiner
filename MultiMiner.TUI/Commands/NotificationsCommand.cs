using MultiMiner.TUI.Data;
using MultiMiner.UX.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.TUI.Commands
{
    class NotificationsCommand
    {
        private readonly List<NotificationEventArgs> notifications;

        public NotificationsCommand(List<NotificationEventArgs> notifications)
        {
            this.notifications = notifications;
        }

        public bool HandleCommand(string[] input)
        {
            if (input.Count() >= 2)
            {
                var verb = input[1];
                if (verb.Equals(CommandArguments.Clear, StringComparison.OrdinalIgnoreCase))
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
                            if (verb.Equals(CommandArguments.Remove, StringComparison.OrdinalIgnoreCase))
                            {
                                notifications.RemoveAt(index);
                                return true; //early exit - success
                            }
                            else if (verb.Equals(CommandArguments.Act, StringComparison.OrdinalIgnoreCase))
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
    }
}
