using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Runtime.InteropServices;
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

            Shell32.Folder folder = GetShell32NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = null;
                try
                {
                    link = (Shell32.ShellLinkObject)folderItem.GetLink;
                }
                catch (COMException ex)
                {
                    return String.Empty; // reported by user
                }
                return link.Path;
            }

            return String.Empty; // Not found
        }

        //used instead of shellClass.NameSpace() for compatibility with various Windows OS's
        //http://techitongue.blogspot.com/2012/06/shell32-code-compiled-on-windows-7.html
        public static Shell32.Folder GetShell32NameSpace(Object folder)
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { folder });
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
