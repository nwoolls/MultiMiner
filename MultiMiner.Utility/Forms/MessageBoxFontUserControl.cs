using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Utility.Forms
{
    public partial class MessageBoxFontUserControl : UserControl
    {
        public MessageBoxFontUserControl()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }
    }
}
