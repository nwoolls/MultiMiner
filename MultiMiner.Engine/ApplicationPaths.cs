using System;
using System.IO;

namespace MultiMiner.Engine
{
    public static class ApplicationPaths
    {
        public static string AppDataPath()
        {
            //if running in "portable" mode
            if (IsRunningInPortableMode())
            {
                //store files in the working directory rather than AppData
                return GetWorkingDirectory();
            }

            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootPath, "MultiMiner");
        }

        //simply check for a file named "portable" in the same folder
        private static bool IsRunningInPortableMode()
        {
            string assemblyDirectory = GetWorkingDirectory();
            string portableFile = Path.Combine(assemblyDirectory, "portable");
            return File.Exists(portableFile);
        }

        private static string GetWorkingDirectory()
        {
            string assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(assemblyFilePath);
        }
    }
}
