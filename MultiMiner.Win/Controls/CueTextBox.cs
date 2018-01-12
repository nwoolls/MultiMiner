using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiMiner.Win.Controls
{
    /// <summary>
    /// Represents a Windows text box control with a cue banner.
    /// </summary>
    public class CueTextBox : TextBox
    {
        /// <summary>
        /// The cue banner text.
        /// </summary>
        private string cueText;

        /// <summary>
        /// Gets or sets the cue banner text.
        /// </summary>
        public string CueText
        {
            get
            {
                return cueText;
            }

            set
            {
                cueText = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">A Windows Message object.</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            const int WM_PAINT = 0xF;
            if (m.Msg == WM_PAINT)
            {
                if (!Focused && String.IsNullOrEmpty(Text) && !String.IsNullOrEmpty(CueText))
                {
                    using (var graphics = CreateGraphics())
                    {
                        TextRenderer.DrawText(
                            dc: graphics,
                            text: CueText,
                            font: Font,
                            bounds: ClientRectangle,
                            foreColor: SystemColors.GrayText,
                            backColor: Enabled ? BackColor : SystemColors.Control,
                            flags: TextFormatFlags.Top | TextFormatFlags.Left);
                    }
                }
            }
        }
    }
}
