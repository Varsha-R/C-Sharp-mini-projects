using Newtonsoft.Json;
using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PinUnpin
{
    static class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int LoadString(IntPtr hInstance, uint wID, StringBuilder lpBuffer, int nBufferMax);

        //static void Main(string[] args)
        //{
        //    string importFiles = "C:\\Users\\Varsha.Ravindra\\Documents\\Registry";
        //    Dictionary<object, object> importJson = new Dictionary<object, object>();
        //    importJson = JsonConvert.DeserializeObject<Dictionary<object, object>>(File.ReadAllText(Path.Combine(importFiles, "TaskbarPinned.json"), Encoding.UTF8));
        //    Console.WriteLine();
        //    foreach (KeyValuePair<object, object> val in importJson)
        //    {
        //        string targetPath = val.Value.ToString();
        //        if (string.IsNullOrEmpty(targetPath))
        //        {
        //            continue;
        //        }
        //        else if (targetPath.Contains("C:\\Users\\"))
        //        {
        //            string userFolder = Directory.GetParent(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString()).ToString();
        //            List<string> splitTarget = new List<string>();
        //            splitTarget = targetPath.Split('\\').ToList();
        //            splitTarget.RemoveRange(0, 3);
        //            string restPath = string.Join("\\", splitTarget);
        //            targetPath = Path.Combine(userFolder, restPath);
        //        }
        //        if (System.IO.File.Exists(targetPath))
        //        {
        //            Console.WriteLine("EXISTS : {0}", targetPath);
        //            PinUnpinTaskbar(targetPath, true);
        //            Console.WriteLine();
        //        }
        //    }
        //    Console.ReadKey();
        //}

        static void Main(string[] args)
        {
            string filePath = @"C:\Program Files\Notepad++\notepad++.exe";
            if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);
            // create the shell application object            
            Type t = Type.GetTypeFromProgID("Shell.Application");
            dynamic shellApplication = Activator.CreateInstance(t);

            string path = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            Folder directory = shellApplication.NameSpace(path);
            FolderItem link = directory.ParseName(fileName);
            FolderItemVerbs verbs = link.Verbs();
            for (int i = 0; i < verbs.Count; i++)
            {
                FolderItemVerb verb = verbs.Item(i);
                string verbName = verb.Name;
                Console.WriteLine(verbName);
                if (verbName.Equals("pin to taskbar"))
                {
                    verb.DoIt();
                }
            }
            shellApplication = null;
            Console.ReadKey();
        }
    }
}
