using System;
using System.Runtime.InteropServices;

namespace MultiMiner.Utility.OS
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

        private static bool IsRunningOnMac()
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname ()
                if (NativeMethods.uname(buf) == 0)
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

        public static bool IsWindowsVistaOrHigher()
        {
            OperatingSystem OS = Environment.OSVersion;
            return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
        }
    }
}
