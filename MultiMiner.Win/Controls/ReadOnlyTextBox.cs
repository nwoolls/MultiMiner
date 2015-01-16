using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MultiMiner.Win.Controls
{
    public class ReadOnlyTextBox : TextBox
    {
        public ReadOnlyTextBox()
        {
            this.ReadOnly = true;
            this.GotFocus += TextBoxGotFocus;
            this.Cursor = Cursors.Arrow;
        }

        private void TextBoxGotFocus(object sender, EventArgs args)
        {
            NativeMethods.HideCaret(this.Handle);
        }
    }
}
