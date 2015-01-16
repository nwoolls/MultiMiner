using System;

namespace MultiMiner.Win.Forms
{
    public partial class CredentialsForm : MessageBoxFontForm
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public CredentialsForm(string target, string username, string password)
        {
            InitializeComponent();
            Text = String.Format("SSH {0}", target);
            usernameEdit.Text = username;
            passwordEdit.Text = password;
        }
        
        private void saveButton_Click(object sender, EventArgs e)
        {
            Username = usernameEdit.Text;
            Password = passwordEdit.Text;
        }
    }
}
