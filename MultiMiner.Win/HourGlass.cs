using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MultiMiner.Utility;

namespace MultiMiner.Win
{
    public class HourGlass : IDisposable
    {
        public HourGlass()
        {
            Enabled = true;
        }

        public void Dispose()
        {
            Enabled = false;
        }

        public static bool Enabled
        {
            get { return Application.UseWaitCursor; }
            set
            {
                if (value == Application.UseWaitCursor) return;
                Application.UseWaitCursor = value;
                Form activeForm = Form.ActiveForm;
                if (activeForm != null)
                {
                    if (activeForm.InvokeRequired)
                        activeForm.BeginInvoke((Action)(() => { CheckAndShowCursor(activeForm); }));
                    else
                        CheckAndShowCursor(activeForm);
                }
                Application.DoEvents();
            }
        }

        private static void CheckAndShowCursor(Form activeForm)
        {
            if (activeForm.Handle != null
                                    && (OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix))
                // Send WM_SETCURSOR
                SendMessage(activeForm.Handle, 0x20, activeForm.Handle, (IntPtr)1);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }
}
