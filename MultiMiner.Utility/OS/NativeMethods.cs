using System;
using System.Runtime.InteropServices;

namespace MultiMiner.Utility.OS
{
    static class NativeMethods
    {
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("User32.dll")]
        internal static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        internal static extern uint GetLastError();
        
        //From Managed.Windows.Forms/XplatUI
        [DllImport("libc")]
        internal static extern int uname(IntPtr buf);
    }
}
