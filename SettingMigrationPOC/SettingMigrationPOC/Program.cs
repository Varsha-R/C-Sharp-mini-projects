using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingMigrationPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            //RegistryKey registryKey = Registry.Users;
            //RegistryKey mouseKey = registryKey.OpenSubKey(Path.Combine("S-1-5-21-1958457135-445119708-1134724429-4778\\Control Panel\\Cursors"));
            //string person = null;
            //string[] valueNames = mouseKey.GetValueNames();
            //if(valueNames.Contains("Person"))
            //{
            //    person = mouseKey.GetValue("Person").ToString();
            //    string sourcePath = person;
            //    string dest = sourcePath.Split('\\').Last();
            //    string environmentPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Microsoft\\Windows\\Cursors";
            //    string destinationPath = "C:\\Users\\Varsha.Ravindra\\Documents\\Registry";
            //    string setValuePath = Path.Combine(environmentPath, dest);
            //    mouseKey.SetValue("Person", setValuePath);
            //    if(!System.IO.Directory.Exists(destinationPath))
            //    {
            //        Console.WriteLine("Directory doesn't exist");
            //        System.IO.Directory.CreateDirectory(destinationPath);
            //    }
            //    string destinationFile = Path.Combine(destinationPath, dest);
            //    if(System.IO.File.Exists(destinationFile))
            //    {
            //        Console.WriteLine("FILE EXISTS");
            //        System.IO.File.Delete(destinationFile);
            //    }
            //    System.IO.File.Copy(sourcePath, destinationFile, true);
            //}
            //else
            //{
            //    Console.WriteLine("Key doesn't exist");
            //}

            //RegistryKey registryKey = Registry.Users;
            //RegistryKey mouseKey = registryKey.OpenSubKey(Path.Combine("S-1-5-21-1958457135-445119708-1134724429-4778\\Control Panel\\Cursors"));
            //string[] regKeyValues = mouseKey.GetValueNames();
            //Dictionary<object, object> map = new Dictionary<object, object>();
            //foreach(string value in regKeyValues)
            //{
            //    string regKey = value;
            //    string regValue = mouseKey.GetValue(regKey).ToString();
            //    if (regValue.Contains("\\Microsoft\\Windows\\Cursors"))
            //    {
            //        string sourcePath = regValue;
            //        string destPath = "C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\CursorFiles";
            //        string fileName = sourcePath.Split('\\').Last();
            //        string destinationPath = Path.Combine(destPath, fileName);
            //        System.IO.File.Copy(sourcePath, destinationPath);
            //        map.Add(regKey, mouseKey.GetValue(regKey));
            //    }
            //}
            //string json = JsonConvert.SerializeObject(map);
            //File.WriteAllText("C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\test.json", json);
            //Console.WriteLine("COMPLETE");

            string spiJson = "C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\test.json";
            Dictionary<object, object> importJson = new Dictionary<object, object>();
            importJson = JsonConvert.DeserializeObject<Dictionary<object, object>>(File.ReadAllText(spiJson, Encoding.UTF8));
            if(importJson.Any())
            {
                List<string> keys = new List<string>();
                foreach(KeyValuePair<object, object> val in importJson)
                {
                    keys.Add(val.Key.ToString());
                    Console.WriteLine(val.Key.ToString());
                }
            }
            Console.ReadKey();
        }
    }
}
