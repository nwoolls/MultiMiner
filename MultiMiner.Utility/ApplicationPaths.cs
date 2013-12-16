using System;
using System.IO;

namespace MultiMiner.Utility
{
    public static class ApplicationPaths
    {
        public static string AppDataPath()
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }
    }
}
