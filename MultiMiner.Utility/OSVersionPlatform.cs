using System;
using System.Runtime.InteropServices;

namespace MultiMiner.Utility
{
    public class OSVersionPlatform
    {
        public static PlatformID GetGenericPlatform()
        {
            return Environment.OSVersion.Platform;
        }

        public static PlatformID GetConcretePlatform()
        {
            PlatformID result = Environment.OSVersion.Platform;

            if ((result == PlatformID.Unix) && IsRunningOnMac())
                result = PlatformID.MacOSX;

            return result;
        }

        //From Managed.Windows.Forms/XplatUI
        [DllImport("libc")]
        private static extern int uname(IntPtr buf);

        private static bool IsRunningOnMac()
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname ()
                if (uname(buf) == 0)
                {
                    string os = Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin")
                        return true;
                }
            }
            catch
            {
            }
            finally
            {
                if (buf != IntPtr.Zero)
                    Marshal.FreeHGlobal(buf);
            }
            return false;
        }

        public static string GetHomeDirectoryPath()
        {
            string result = (GetGenericPlatform() == PlatformID.Unix)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            return result;
        }
    }
}
