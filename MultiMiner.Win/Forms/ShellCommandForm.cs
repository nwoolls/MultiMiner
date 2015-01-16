using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiMiner.Win.Forms
{
    public partial class ShellCommandForm : MessageBoxFontForm
    {
        public string ShellCommand { get; set; }

        public ShellCommandForm(IEnumerable<string> recentCommands)
        {
            InitializeComponent();
            commandEdit.AutoCompleteCustomSource.AddRange(recentCommands.ToArray());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            ShellCommand = commandEdit.Text;
        }

        private void ShellCommandForm_Load(object sender, EventArgs e)
        {

        }
    }
}
