using System;
using System.Windows.Forms;

namespace MultiMiner.UX.Data
{
    public struct Notification
    {
        public string Id;
        public string Text;
        public Action ClickHandler;
        public ToolTipIcon Kind;
        public string InformationUrl;
    }
}
