using System;
using System.Windows.Forms;

namespace MultiMiner.Win.Notifications
{
    public partial class NotificationControl : UserControl
    {
        private readonly Action clickHandler;
        private readonly Action<NotificationControl> closeHandler;

        public NotificationControl(string text, Action clickHandler, Action<NotificationControl> closeHandler)
        {
            InitializeComponent();

            linkLabel1.Text = text;
            this.clickHandler = clickHandler;
            this.closeHandler = closeHandler;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            closeHandler(this);
            clickHandler();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            closeHandler(this);
        }
    }
}
