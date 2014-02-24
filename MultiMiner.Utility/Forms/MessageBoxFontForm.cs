using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Utility.Forms
{
    public partial class MessageBoxFontForm : Form
    {
        public MessageBoxFontForm()
        {
            if (OS.OSVersionPlatform.GetGenericPlatform() != System.PlatformID.Unix)
                this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }
    }
}
