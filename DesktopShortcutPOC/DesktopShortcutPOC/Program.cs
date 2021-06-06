using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DesktopShortcutPOC
{
    class Program
    {
        public static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation)
        {
            string shortcutLocation = System.IO.Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "My shortcut description";   // The description of the shortcut
            //shortcut.IconLocation = @"c:\myicon.ico";           // The icon of the shortcut
            shortcut.TargetPath = targetFileLocation;                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }

        static void Main(string[] args)
        {
            int a = 61;
            Console.WriteLine(a / 10);
            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Dell Data Assistant.lnk");
            Console.WriteLine("Shortcut path: {0}", shortcutPath);
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Console.WriteLine("Desktop path: {0}", desktopPath);
            string specialPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Console.WriteLine("Special folder: {0}\n", specialPath);

            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            Console.WriteLine(Environment.UserName);
            Console.WriteLine(Environment.SpecialFolder.Desktop);
            //CreateShortcut("My Shortcut", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Assembly.GetExecutingAssembly().Location);
            //CreateShortcut("All desktops Shortcut", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("DONE");
            Console.ReadKey();
        }
    }
}
