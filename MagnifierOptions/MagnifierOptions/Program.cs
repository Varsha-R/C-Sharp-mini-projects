using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnifierOptions
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryKey magnifierOptionsAfterSignIn = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Accessibility", true);
            string[] afterValueNames = magnifierOptionsAfterSignIn.GetValueNames();
            if (magnifierOptionsAfterSignIn.GetValueNames().Contains("Configuration"))
            {
                string afterSignIn = magnifierOptionsAfterSignIn.GetValue("Configuration").ToString();
                Console.WriteLine("BEFORE (After SignIn): " + afterSignIn);
                magnifierOptionsAfterSignIn.SetValue("Bib", "magnifierpane", RegistryValueKind.String);
                Console.WriteLine("AFTER SET: " + magnifierOptionsAfterSignIn.GetValue("Configuration").ToString());
            }

            if (Environment.Is64BitOperatingSystem)
            {
                using (var root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var magnifierOptionsBeforeSignIn = root.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Accessibility", true))
                    {
                        if (magnifierOptionsBeforeSignIn.GetValueNames().Contains("Configuration"))
                        {
                            string beforeSignIn = magnifierOptionsBeforeSignIn.GetValue("Configuration").ToString();
                            Console.WriteLine("BEFORE (After SignIn): " + beforeSignIn);
                            magnifierOptionsBeforeSignIn.SetValue("Configuration", "magnifierpane", RegistryValueKind.String);
                            Console.WriteLine("AFTER SET: " + magnifierOptionsBeforeSignIn.GetValue("Configuration").ToString());
                        }
                    };
                };
            }
            else
            {
                using (var root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (var magnifierOptionsBeforeSignIn = root.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Accessibility", true))
                    {
                        if (magnifierOptionsBeforeSignIn.GetValueNames().Contains("Configuration"))
                        {
                            string beforeSignIn = magnifierOptionsBeforeSignIn.GetValue("Configuration").ToString();
                            Console.WriteLine("BEFORE (After SignIn): " + beforeSignIn);
                            magnifierOptionsBeforeSignIn.SetValue("Configuration", "magnifierpane", RegistryValueKind.String);
                            Console.WriteLine("AFTER SET: " + magnifierOptionsBeforeSignIn.GetValue("Configuration").ToString());
                        }
                    };
                };
            }

            Console.ReadKey();
        }
    }
}
