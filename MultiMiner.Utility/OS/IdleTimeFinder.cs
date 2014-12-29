using System;
using System.Runtime.InteropServices;

namespace MultiMiner.Utility.OS
{
    /// <summary>
    /// Helps to find the idle time, (in ticks) spent since the last user input
    /// </summary>
    public class IdleTimeFinder
    {
        /// <summary>
        /// Get the last input time in ticks
        /// </summary>
        /// <returns></returns>
        public static uint GetLastInputTime()
        {
            NativeMethods.LASTINPUTINFO lastInPut = new NativeMethods.LASTINPUTINFO();
            lastInPut.cbSize = (uint)Marshal.SizeOf(lastInPut);
            if (!NativeMethods.GetLastInputInfo(ref lastInPut))
                throw new Exception(NativeMethods.GetLastError().ToString());
            return lastInPut.dwTime;
        }
    }
}