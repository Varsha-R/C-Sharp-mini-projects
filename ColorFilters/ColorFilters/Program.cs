using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorFilters
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryKey colorFilter = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\ColorFiltering", true);
            string filter = colorFilter.GetValue("Active").ToString();
            colorFilter.SetValue("Active", "0", RegistryValueKind.DWord);
            string cmdPath = Environment.GetEnvironmentVariable("ComSpec");
            Console.WriteLine(filter);
            Console.ReadKey();
        }
    }    
}
