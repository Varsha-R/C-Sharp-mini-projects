using Microsoft.Win32;
using System;
using System.Management;

namespace GetManufacturerPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            string manufacturer = GetManufacturer();
            bool validOS = ValidateOS();
            
            Console.ReadKey();
        }

        private static string GetManufacturer()
        {
            SelectQuery query = new SelectQuery(@"Select * from Win32_ComputerSystem");
            string manufacturer = "";
            //initialize the searcher with the query it is supposed to execute
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                //execute the query
                foreach (ManagementObject process in searcher.Get())
                {
                    //print system info
                    process.Get();
                    Console.WriteLine("/*********Operating System Information ***************/");
                    manufacturer = process["Manufacturer"].ToString();
                    Console.WriteLine("{0}{1}", "System Manufacturer:", process["Manufacturer"]);
                    Console.WriteLine("{0}{1}", "System Model:", process["Model"]);
                }
            }
            if (manufacturer.Contains("Dell"))
            {
                Console.WriteLine("\nManufactured by Dell");
            }
            else
            {
                Console.WriteLine("\nNot manufactured by Dell");
            }
            return manufacturer;
        }

        private static bool ValidateOS()
        {
            string windowsName = GetWindowsName();
            Console.WriteLine("Windows Name is: <#{0}#>", windowsName);
            return windowsName.Contains("Windows 10") || windowsName.Contains("Windows 8") || windowsName.Contains("Windows 8.1");
        }

        public static string GetWindowsName()
        {
            return HKLMGetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
        }

        public static string HKLMGetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }
    }
}
