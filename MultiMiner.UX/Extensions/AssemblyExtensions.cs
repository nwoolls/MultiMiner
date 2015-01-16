using System.Reflection;
using System;

namespace MultiMiner.UX.Extensions
{
    public static class AssemblyExtensions
    {
        public static DateTime GetCompileDate(this Assembly assembly)
        {
            return RetrieveLinkerTimestamp(assembly.Location);
        }

        // http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html
        private static DateTime RetrieveLinkerTimestamp(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.FileStream s = null;
            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                    s.Close();
            }
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(BitConverter.ToInt32(b, BitConverter.ToInt32(b, peHeaderOffset) + linkerTimestampOffset));
            return dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
        }
    }
}
