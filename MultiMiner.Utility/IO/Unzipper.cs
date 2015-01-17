using MultiMiner.Utility.OS;
using System;
using System.Diagnostics;
using System.IO;

namespace MultiMiner.Utility.IO
{
    public static class Unzipper
    {
        public static void UnzipFileToFolder(string zipFilePath, string destionationFolder)
        {
            Directory.CreateDirectory(destionationFolder);

            if (OSVersionPlatform.GetGenericPlatform() == PlatformID.Unix)
                UnzipFileToFolderUnix(zipFilePath, destionationFolder);
#if !__MonoCS__
            else
                UnzipFileToFolderWindows(zipFilePath, destionationFolder);
#endif
        }

        private static void UnzipFileToFolderUnix(string zipFilePath, string destinationFolder)
        {

            string temporaryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temporaryPath);
            
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "tar";
            startInfo.Arguments = String.Format("-xzf \"{0}\" -C \"{1}\"", zipFilePath, temporaryPath);
            
            Process process = Process.Start(startInfo);
            process.WaitForExit();

            DirectoryInfo directoryInfo = new DirectoryInfo(temporaryPath);
            FileInfo[] files = directoryInfo.GetFiles();
            DirectoryInfo[] directories = directoryInfo.GetDirectories();

            //if the zip file contains a single directory, extract that directory's contents
            if ((files.Length == 0) && (directories.Length == 1))
            {
                CopyDirectoryContents(directories[0].FullName, destinationFolder);
            }
            else
            {
                CopyDirectoryContents(temporaryPath, destinationFolder);
            }
            
            Directory.Delete(temporaryPath, true);
        }

        private static void CopyDirectoryContents(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);

            foreach (var directory in Directory.GetDirectories(sourceDir))
                CopyDirectoryContents(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

#if !__MonoCS__
        private static void UnzipFileToFolderWindows(string zipFilePath, string destinationFolder)
        {
            const bool showProgress = false;
            const bool yesToAll = true;
            
            Shell32.Folder fromFolder = GetShell32NameSpace(zipFilePath);
            Directory.CreateDirectory(destinationFolder);
            Shell32.Folder toFolder = GetShell32NameSpace(destinationFolder);
            Shell32.FolderItems sourceFolderItems = fromFolder.Items();

            Shell32.FolderItems itemsToExtract;

            //if the zip file contains a single directory, extract that directory's contents
            if ((sourceFolderItems.Count == 1) &&
                (sourceFolderItems.Item(0).GetFolder is Shell32.Folder))
            {
                Shell32.FolderItem rootItem = sourceFolderItems.Item(0);
                itemsToExtract = ((Shell32.Folder)rootItem.GetFolder).Items();
            }
            else 
            {
                itemsToExtract = sourceFolderItems;
            }

            int options = 0;
            if (!showProgress)
                options += 4;
            if (yesToAll)
                options += 16;

            toFolder.CopyHere(itemsToExtract, options);
        }

        //used instead of shellClass.NameSpace() for compatibility with various Windows OS's
        //http://techitongue.blogspot.com/2012/06/shell32-code-compiled-on-windows-7.html
        private static Shell32.Folder GetShell32NameSpace(Object folder)
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { folder });
        }
#endif

    }
}
