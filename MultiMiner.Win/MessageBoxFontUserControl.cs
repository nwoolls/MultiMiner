using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Win
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
