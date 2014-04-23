using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiMiner.MobileMiner.Data
{
    public class Notification
    {
        public string NotificationText { get; set; }
        public NotificationKind NotificationKind { get; set; }
        public string MachineName { get; set; }
    }
}
