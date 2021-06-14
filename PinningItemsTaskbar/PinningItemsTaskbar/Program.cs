using Microsoft.Win32;
using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PinningItemsTaskbar
{
    class Program
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);


        static void Main(string[] args)
        {
            //string l_strFilePath = @"C:\Program Files\Notepad++\notepad++.exe";
            //if (!File.Exists(l_strFilePath)) throw new FileNotFoundException(l_strFilePath);
            //int MAX_PATH = 255;
            //var actionIndex = true ? 5386 : 5387;
            //StringBuilder szPinToStartLocalized = new StringBuilder(MAX_PATH);
            //IntPtr hShell32 = LoadLibrary("Shell32.dll");
            //LoadString(hShell32, (uint)actionIndex, szPinToStartLocalized, MAX_PATH);
            //string localizedVerb = szPinToStartLocalized.ToString();
            //string path = Path.GetDirectoryName(l_strFilePath);
            //string fileName = Path.GetFileName(l_strFilePath);
            //dynamic shellApplication = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
            //dynamic directory = shellApplication.NameSpace(path);
            //dynamic link = directory.ParseName(fileName);

            //dynamic verbs = link.Verbs();
            //for (int i = 0; i < verbs.Count(); i++)
            //{
            //    dynamic verb = verbs.Item(i);
            //    if (verb.Name.Equals(localizedVerb))
            //    {
            //        verb.DoIt();
            //        return;
            //    }
            //}


            RegistryKey TaskbarPinned = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\Windows.taskbarpin");
            string explorerCommandHandler = TaskbarPinned.GetValue("ExplorerCommandHandler", 0).ToString();
            Console.WriteLine(explorerCommandHandler);

            string filePath = @"C:\Program Files\Notepad++\notepad++.exe";
            if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);
            string path = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            RegistryKey createKey = Registry.CurrentUser.OpenSubKey(@"Software\Classes\*\shell", true);
            createKey.CreateSubKey("{:}", true);
            RegistryKey openNewKey = Registry.CurrentUser.OpenSubKey(@"Software\Classes\*\shell\{:}\", true);
            openNewKey.SetValue("ExplorerCommandHandler", explorerCommandHandler, RegistryValueKind.String);

            Type t = Type.GetTypeFromProgID("Shell.Application");
            dynamic shellApplication = Activator.CreateInstance(t);

            Folder directory = shellApplication.NameSpace(path);
            FolderItem link = directory.ParseName(fileName);
            FolderItemVerbs verbs = link.Verbs();
            for (int i = 0; i < verbs.Count; i++)
            {
                FolderItemVerb verb = verbs.Item(i);
                string verbName = verb.Name.Replace("&", "");
                Console.WriteLine(verbName);
                //verbName.Equals("pin to taskbar") || verbName.Equals("taskbarpin") ||
                if (verbName.Equals("{:}"))
                {
                    Console.WriteLine("DO IT");
                    verb.DoIt();
                }
            }
            createKey.DeleteSubKey("{:}");
            Console.ReadKey();
        }
    }
}
