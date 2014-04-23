using MultiMiner.Utility.Forms;
using System;
using System.Windows.Forms;

namespace MultiMiner.Win.Controls.Notifications
{
    public partial class NotificationsControl : MessageBoxFontUserControl
    {
        //events
        //delegate declarations
        public delegate void NotificationsChangedHandler(object sender);
        public delegate void NotificationAddedHandler(string text, MobileMiner.Data.NotificationKind kind);

        //event declarations        
        public event NotificationsChangedHandler NotificationsChanged;
        public event NotificationAddedHandler NotificationAdded;

        public NotificationsControl()
        {
            InitializeComponent();
        }

        public void RemoveNotification(string id)
        {
            NotificationControl notificationControl;

            foreach (Control control in containerPanel.Controls)
            {
                notificationControl = (NotificationControl)control;
                if ((string)notificationControl.Tag == id)
                {
                    notificationControl.Parent = null;
                    if (NotificationsChanged != null)
                        NotificationsChanged(this);
                }
            }
        }

        public void AddNotification(string id, string text, Action clickHandler, MobileMiner.Data.NotificationKind kind, string informationUrl = "")
        {
            NotificationControl notificationControl;

            foreach (Control control in containerPanel.Controls)
            {
                notificationControl = (NotificationControl)control;
                if ((string)notificationControl.Tag == id)
                    return;
            }

            notificationControl = new NotificationControl(text, clickHandler, (nc) => { 
                nc.Parent = null;
                if (NotificationsChanged != null)
                    NotificationsChanged(this);
            }, informationUrl);

            notificationControl.Height = 28;
            notificationControl.Parent = containerPanel;
            notificationControl.Top = Int16.MaxValue;
            notificationControl.Tag = (object)id;

            notificationControl.BringToFront();

            notificationControl.Dock = DockStyle.Top;

            containerPanel.ScrollControlIntoView(notificationControl);

            if (NotificationAdded != null)
                NotificationAdded(text, kind);

            if (NotificationsChanged != null)
                NotificationsChanged(this);           
        }

        public int NotificationCount()
        {
            return containerPanel.Controls.Count;
        }
    }
}
