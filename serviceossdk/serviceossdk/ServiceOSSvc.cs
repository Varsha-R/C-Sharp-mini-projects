using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace serviceossdk
{
    public class ServiceOSSvc : IServiceOSSvc
    {
        string eraseFlow;
        string exceptionHandler;

        public ServiceOSSvc()
        {
            bool isDellManufacturer = GetManufacturer();
            bool isValidOS = ValidateOS();
            if (!isDellManufacturer || !isValidOS) throw new NotSupportedSystemException();

            string path = @"C:\Program Files (x86)\Dell\SARemediation\agent\serviceossdk\serviceossdk.dll";
            Configuration config = ConfigurationManager.OpenExeConfiguration(path);

            eraseFlow = GetAppSetting(config, "EraseFlow");
            exceptionHandler = GetAppSetting(config, "ExceptionHandler");

            switch(exceptionHandler)
            {
                case "NotSupportedSystemException": throw new NotSupportedSystemException();
                case "UnauthorizedAccessException": throw new UnauthorizedAccessException();
                case "FeatureNotAvailableException": throw new FeatureNotAvailableException();
                case "NotSupportedMethodException": throw new NotSupportedMethodException();
                case "ServiceCriticalFailureException": throw new ServiceCriticalFailureException();
                case "ServiceNotAvailableException": throw new ServiceNotAvailableException();
                case "None": return;
            }
        }

        string GetAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }

        private bool GetManufacturer()
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
                    manufacturer = process["Manufacturer"].ToString();
                }
            }
            if (manufacturer.Contains("Dell"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ValidateOS()
        {
            string windowsName = GetWindowsName();
            return windowsName.Contains("Windows 10");
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


        public bool IsSecureResetSupported()
        {
            bool eraseSupported = false;
            switch(eraseFlow)
            {
                case "Excalibur": eraseSupported = true;
                    break;
                case "WindowsReset": eraseSupported = false;
                    break;
            }
            return eraseSupported;
        }
    }

    class FeatureNotAvailableException : Exception
    {
        public FeatureNotAvailableException()
        {
        }
    }

    class NotSupportedMethodException : Exception
    {
        public NotSupportedMethodException()
        {
        }
    }

    class ServiceCriticalFailureException : Exception
    {
        public ServiceCriticalFailureException()
        {
        }
    }

    class NotSupportedSystemException : Exception
    {
        public NotSupportedSystemException()
        {
        }
    }

    class UnauthorizedAccessException : Exception
    {
        public UnauthorizedAccessException()
        {
        }
    }

    class ServiceNotAvailableException : Exception
    {
        public ServiceNotAvailableException()
        {
        }
    }
}
