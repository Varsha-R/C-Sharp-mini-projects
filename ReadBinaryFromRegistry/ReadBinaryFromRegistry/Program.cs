using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBinaryFromRegistry
{
    class Program
    {
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes)
            {
                result += b.ToString("x2").ToUpperInvariant();
            }
            return result;
        }
        public static string HextoString(string InputText)
        {

            byte[] bb = Enumerable.Range(0, InputText.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(InputText.Substring(x, 2), 16))
                             .ToArray();
            return System.Text.Encoding.ASCII.GetString(bb);
            // or System.Text.Encoding.UTF7.GetString
            // or System.Text.Encoding.UTF8.GetString
            // or System.Text.Encoding.Unicode.GetString
            // or etc.
        }
        static void Main(string[] args)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband", true);
            var value = (byte[])registryKey.GetValue("Favorites");
            
            string hex = BytesToString(value);
            foreach (var c in value) { Console.Write(c); }
            string s = HextoString(hex);

            Console.WriteLine(registryKey.GetValue("Favorites").GetType());
            var valueAsString = BitConverter.ToString(value);
            string str = Encoding.UTF8.GetString(value, 0, value.Length);
            Console.WriteLine(str);

            Console.ReadKey();
        }
    }
}
