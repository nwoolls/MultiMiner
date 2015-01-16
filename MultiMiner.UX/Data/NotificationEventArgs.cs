using MultiMiner.MobileMiner.Data;
using System;

namespace MultiMiner.UX.Data
{
    public class NotificationEventArgs : EventArgs
    {
        public string Id;
        public string Text;
        public Action ClickHandler;
        public NotificationKind Kind;
        public string InformationUrl;
    }
}
