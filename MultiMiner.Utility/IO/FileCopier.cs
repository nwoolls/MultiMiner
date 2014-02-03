using System.IO;

namespace MultiMiner.Utility.IO
{
    public static class FileCopier
    {

        public static void CopyFilesToFolder(string sourceDirectory, string destinationDirectory, string searchPattern)
        {
            if (Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
                foreach (string sourceFilePath in Directory.GetFiles(sourceDirectory, searchPattern))
                {
                    string fileName = Path.GetFileName(sourceFilePath);
                    string destinationFilePath = Path.Combine(destinationDirectory, fileName);

                    File.Copy(sourceFilePath, destinationFilePath, true);
                }
            }
        }
    }
}
