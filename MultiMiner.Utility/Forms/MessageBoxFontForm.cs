using System.Drawing;
using System.Windows.Forms;

namespace MultiMiner.Utility.Forms
{
    public partial class MessageBoxFontForm : Form
    {
        public MessageBoxFontForm()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }
    }
}
