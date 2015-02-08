using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Win.Controls.Notifications
{
    public partial class NotificationControl : MessageBoxFontUserControl
    {
        private readonly Action clickHandler;
        private readonly Action<NotificationControl> closeHandler;
        private readonly string informationUrl;

        public NotificationControl(string text, Action clickHandler, Action<NotificationControl> closeHandler,
            string informationUrl = "")
        {
            InitializeComponent();

            linkLabel.Text = text;
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
            PositionControls();
        }

        //needs handling on Ubuntu/Linux
        private void PositionControls()
        {
            closeButton.Size = new Size(22, 22);
            const int Offset = 2;
            closeButton.Location = new Point(Width - closeButton.Width - Offset, 0 + Offset);
            infoPicture.Location = new Point(closeButton.Left - infoPicture.Width - Offset, closeButton.Top + 3);
            int labelWidth = infoPicture.Left - linkLabel.Left - Offset;
            if (!infoPicture.Visible)
                labelWidth += infoPicture.Width + Offset;
            linkLabel.Size = new Size(labelWidth, 20);
        }
    }
}
