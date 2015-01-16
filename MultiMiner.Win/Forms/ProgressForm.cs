using MultiMiner.Win.Properties;
using System.Drawing;

namespace MultiMiner.Win.Forms
{
    public partial class ProgressForm : MessageBoxFontForm
    {
        public ProgressForm(string labelText)
        {
            InitializeComponent();
            label1.Text = labelText;
            IsDownload = true;
        }

        public bool IsDownload { get; set; }

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

        private void ProgressForm_Load(object sender, System.EventArgs e)
        {
            if (IsDownload)
                pictureBox1.Image = (Image)Resources.ResourceManager.GetObject("internet_download");
            else
                pictureBox1.Image = (Image)Resources.ResourceManager.GetObject("hardware-find1");
        }
    }
}
