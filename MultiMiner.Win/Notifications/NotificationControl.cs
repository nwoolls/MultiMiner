using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Win.Notifications
{
    public partial class NotificationControl : UserControl
    {
        private readonly Action clickHandler;
        private readonly Action<NotificationControl> closeHandler;
        private readonly string informationUrl;

        public NotificationControl(string text, Action clickHandler, Action<NotificationControl> closeHandler,
            string informationUrl = "")
        {
            InitializeComponent();

            linkLabel1.Text = text;
            this.clickHandler = clickHandler;
            this.closeHandler = closeHandler;
            this.informationUrl = informationUrl;
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

        private void infoPicture_Click(object sender, EventArgs e)
        {
            Process.Start(informationUrl);
        }

        private void NotificationControl_Load(object sender, EventArgs e)
        {
            infoPicture.Visible = !String.IsNullOrEmpty(informationUrl);
            PositionCloseButton(); 
        }

        private void PositionCloseButton()
        {
            closeButton.Size = new Size(22, 22);
            const int offset = 2;
            closeButton.Location = new Point(this.Width - closeButton.Width - offset, 0 + offset);
        }
    }
}
