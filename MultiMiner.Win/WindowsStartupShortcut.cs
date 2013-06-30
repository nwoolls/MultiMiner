using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Windows.Forms;

namespace MultiMiner.Win
{
    class WindowsStartupShortcut
    {
        public static void CreateStartupFolderShortcut()
        {
            WshShellClass wshShell = new WshShellClass();
            IWshShortcut shortcut;
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            // Create the shortcut
            shortcut = (IWshShortcut)wshShell.CreateShortcut(Path.Combine(startUpFolderPath, Application.ProductName + ".lnk"));

            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Application.StartupPath;

            shortcut.Save();
        }

        private static string GetShortcutTargetFile(string shortcutFilename)
        {
            string pathOnly = Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.ShellClass();
            Shell32.Folder folder = shell.NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return String.Empty; // Not found
        }

        public static void DeleteStartupFolderShortcuts()
        {
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            DirectoryInfo di = new DirectoryInfo(startUpFolderPath);
            FileInfo[] files = di.GetFiles("*.lnk");

            foreach (FileInfo fi in files)
            {
                string shortcutTargetFile = GetShortcutTargetFile(fi.FullName);

                if (shortcutTargetFile.Equals(Application.ExecutablePath,
                      StringComparison.InvariantCultureIgnoreCase))
                {
                    System.IO.File.Delete(fi.FullName);
                }
            }
        }
    }
}
