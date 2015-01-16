using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Win.Controls
{
    public partial class MessageBoxFontUserControl : UserControl
    {
        protected MessageBoxFontUserControl()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }
    }
}
