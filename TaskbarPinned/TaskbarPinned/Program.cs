using IWshRuntimeLibrary;
using Newtonsoft.Json;
using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskbarPinned
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Shell shell = new Shell();
            //string shortcutFilename = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\TaskBar";
            //string[] shortcuts = Directory.GetFiles(shortcutFilename);
            //Dictionary<string, string> map = new Dictionary<string, string>();
            //foreach(string shortcut in shortcuts)
            //{
            //    if (!shortcut.Equals(Path.Combine(shortcutFilename, "desktop.ini")))
            //    {
            //        Console.WriteLine(shortcut.Split('\\').Last());
            //        string pathOnly = Path.GetDirectoryName(shortcut);
            //        string filenameOnly = Path.GetFileName(shortcut);
            //        Console.WriteLine(filenameOnly);
            //        Folder folder = shell.NameSpace(pathOnly);
            //        FolderItem folderItem = folder.ParseName(filenameOnly);
            //        if (folderItem != null)
            //        {
            //            ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
            //            link.Resolve(65);
            //            map.Add(filenameOnly, link.Path);
            //            Console.WriteLine(link.Path);
            //        }
            //    }                
            //}


            //string json = JsonConvert.SerializeObject(map);
            //File.WriteAllText("C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\TaskbarPinned.json", json);
            string importFiles = "C:\\Users\\Varsha.Ravindra\\Documents\\Registry";
            Dictionary<object, object> importJson = new Dictionary<object, object>();
            importJson = JsonConvert.DeserializeObject<Dictionary<object, object>>(System.IO.File.ReadAllText(Path.Combine(importFiles, "TaskbarPinned.json"), Encoding.UTF8));
            Console.WriteLine();
            foreach(KeyValuePair<object, object> val in importJson)
            {
                string targetPath = val.Value.ToString();
                if(string.IsNullOrEmpty(targetPath))
                {
                    continue;
                }
                else if (targetPath.Contains("C:\\Users\\"))
                {
                    string userFolder = Directory.GetParent(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString()).ToString();
                    List<string> splitTarget = new List<string>();
                    splitTarget = targetPath.Split('\\').ToList();
                    splitTarget.RemoveRange(0, 3);
                    string restPath = string.Join("\\", splitTarget);
                    targetPath = Path.Combine(userFolder, restPath);
                }
                if (System.IO.File.Exists(targetPath))
                {
                    Console.WriteLine("EXISTS : {0}", targetPath);
                }
            }
            //foreach (string target in targetPaths)
            //{
            //    string targetPath = target;
            //    if (string.IsNullOrEmpty(target))
            //    {
            //        continue;
            //    }
            //    else if (target.Contains("C:\\Users\\"))
            //    {
            //        Console.WriteLine("ENVIRONMENT PATH");
            //        string userFolder = Directory.GetParent(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString()).ToString();
            //        List<string> splitTarget = new List<string>();
            //        splitTarget = target.Split('\\').ToList();
            //        splitTarget.RemoveRange(0, 3);
            //        string restPath = string.Join("\\", splitTarget);
            //        targetPath = Path.Combine(userFolder, restPath);
            //    }
            //    Console.WriteLine(targetPath);
            //    if (File.Exists(targetPath))
            //    {
            //        Console.WriteLine("EXISTS");
            //    }
            //}
            Console.ReadKey();
        }
    }
}
