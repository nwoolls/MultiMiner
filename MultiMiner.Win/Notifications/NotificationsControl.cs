using System;
using System.Windows.Forms;

namespace MultiMiner.Win.Notifications
{
    public partial class NotificationsControl : UserControl
    {
        //events
        //delegate declarations
        public delegate void NotificationsChangedHandler(object sender);

        //event declarations        
        public event NotificationsChangedHandler NotificationsChanged;

        public NotificationsControl()
        {
            InitializeComponent();
        }

        public void AddNotification(int id, string text, Action clickHandler)
        {
            NotificationControl notificationControl;

            foreach (Control control in containerPanel.Controls)
            {
                notificationControl = (NotificationControl)control;
                if ((int)notificationControl.Tag == id)
                    return;
            }

            notificationControl = new NotificationControl(text, clickHandler, (nc) => { 
                nc.Parent = null;
                if (NotificationsChanged != null)
                    NotificationsChanged(this);
            });

            notificationControl.Height = 28;
            notificationControl.Parent = containerPanel;
            notificationControl.Top = Int16.MaxValue;
            notificationControl.Tag = (object)id;

            notificationControl.BringToFront();

            notificationControl.Dock = DockStyle.Top;

            containerPanel.ScrollControlIntoView(notificationControl);

            if (NotificationsChanged != null)
                NotificationsChanged(this);           
        }

        public int NotificationCount()
        {
            return containerPanel.Controls.Count;
        }
    }
}
