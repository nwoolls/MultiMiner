using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MultiMiner.Utility.OS;

namespace MultiMiner.Win.Forms
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

        private static bool Enabled
        {
            get { return Application.UseWaitCursor; }
            set
            {
                //this just doesn't work well consistently under Linux - usually the hourglass
                //cursor is left spinning
                if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix) return;

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
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private static void CheckAndShowCursor(Form activeForm)
        {
            if (OSVersionPlatform.GetGenericPlatform() != PlatformID.Unix)
                // Send WM_SETCURSOR
                NativeMethods.SendMessage(activeForm.Handle, 0x20, activeForm.Handle, (IntPtr)1);
        }
    }
}
