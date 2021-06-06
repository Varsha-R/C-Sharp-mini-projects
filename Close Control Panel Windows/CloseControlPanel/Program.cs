using System;
using System.IO;
using Shell32;

namespace CloseControlPanel
{
    class Program
    {
        static bool IsControlPanelRecursive(Folder folder)
        {
            var f = folder;
            do
            {
                // Check if Control Panel is one of the parents
                // + Control's Panel parent is "Desktop"
                if (string.Compare(f.Title, "Control Panel", true) == 0 &&
                   f.ParentFolder != null && (string.Compare(f.ParentFolder.Title, "Desktop") == 0))
                    return true;
                f = f.ParentFolder;
            } while (f != null);
            return false;
        }

        static void Main(string[] args)
        {
            SHDocVw.InternetExplorer explorerWindow = null;
            var shellWindows = new SHDocVw.ShellWindows();
            int totalCount = 0;
            int shellWindowsCount = shellWindows.Count;
            bool eq;
            do
            {
                shellWindowsCount = shellWindows.Count;
                Console.WriteLine("SHELL WINDOW COUNT");
                Console.WriteLine(shellWindowsCount);
                totalCount = 0;
                foreach (var shellWindow in shellWindows)
                {
                    totalCount++;
                    try
                    {
                        if (shellWindow == null)
                            continue;

                        explorerWindow = shellWindow as SHDocVw.InternetExplorer;

                        if (explorerWindow == null || explorerWindow.Document == null)
                            continue;

                        if (string.IsNullOrWhiteSpace(explorerWindow.FullName))
                            continue;

                        var file = Path.GetFileName(explorerWindow.FullName);

                        if (string.Compare(file, "explorer.exe", true) != 0)
                            continue;

                        var item = ((Shell32.IShellFolderViewDual2)explorerWindow.Document);

                        if (IsControlPanelRecursive(item.Folder))
                            explorerWindow.Quit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                Console.WriteLine("TOTAL COUNT");
                Console.WriteLine(totalCount);

                if (totalCount == shellWindowsCount)
                    eq = true;
                else
                {
                    eq = false;
                    shellWindowsCount -= totalCount;
                    totalCount = 0;
                }
                Console.WriteLine(eq);
            } while (!eq);
            Console.ReadKey();            
        }
    }
}
