using System.Windows.Forms;

namespace MultiMiner.Win
{
    public partial class ProgressForm : Form
    {
        public ProgressForm(string labelText)
        {
            InitializeComponent();
            label1.Text = labelText;
        }

        public string LabelText
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }
    }
}
