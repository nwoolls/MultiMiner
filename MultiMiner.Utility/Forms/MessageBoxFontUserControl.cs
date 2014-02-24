using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Utility.Forms
{
    public partial class MessageBoxFontUserControl : UserControl
    {
        public MessageBoxFontUserControl()
        {
            if (OS.OSVersionPlatform.GetGenericPlatform() != System.PlatformID.Unix)
                this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }
    }
}
