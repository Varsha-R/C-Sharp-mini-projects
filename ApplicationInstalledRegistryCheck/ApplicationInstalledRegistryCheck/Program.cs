using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationInstalledRegistryCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Checking if OneDrive is installed");
            bool isOneDriveInstalled = IsOneDriveInstalled();
            Console.WriteLine("Is it? {0}", isOneDriveInstalled);
            Console.ReadKey();
        }

        public static bool IsOneDriveInstalled()
        {
            string displayName;
            string OneDriveDisplayName = "Microsoft OneDrive";
            try
            {
                Console.WriteLine("Checking if OneDrive is present in user's system");
                RegistryKey currentUserKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);

                using (var localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    var localMachine64Key = localMachine64.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);
                    using (var localMachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                    {
                        var localMachine32Key = localMachine32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);
                        RegistryKey localMachine6432Nodekey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", false);

                        if (currentUserKey != null)
                        {
                            Console.WriteLine("Looking into CurrentUser Registry Key");
                            foreach (String keyName in currentUserKey.GetSubKeyNames())
                            {
                                bool isPresent = false;
                                RegistryKey subkey = currentUserKey.OpenSubKey(keyName);
                                if (subkey != null && subkey.GetValueNames().Contains("DisplayName"))
                                {
                                    displayName = subkey.GetValue("DisplayName").ToString();
                                    if (string.IsNullOrEmpty(displayName))
                                        continue;
                                    isPresent = displayName.Contains(OneDriveDisplayName);
                                    if (isPresent)
                                    {
                                        Console.WriteLine("OneDrive is installed");
                                        return true;
                                    }
                                }
                            }
                        }

                        if (localMachine64Key != null)
                        {
                            Console.WriteLine("Looking into LocalMachine64 Registry Key");
                            foreach (String keyName in localMachine64Key.GetSubKeyNames())
                            {
                                bool isPresent = false;
                                RegistryKey subkey = localMachine64Key.OpenSubKey(keyName);
                                if (subkey != null && subkey.GetValueNames().Contains("DisplayName"))
                                {
                                    displayName = subkey.GetValue("DisplayName").ToString();
                                    if (string.IsNullOrEmpty(displayName))
                                        continue;
                                    isPresent = displayName.Contains(OneDriveDisplayName);
                                    if (isPresent)
                                    {
                                        Console.WriteLine("OneDrive is installed");
                                        return true;
                                    }
                                }
                            }
                        }

                        if (localMachine32Key != null)
                        {
                            Console.WriteLine("Looking into LocalMachine32 Registry Key");
                            foreach (String keyName in localMachine32Key.GetSubKeyNames())
                            {
                                bool isPresent = false;
                                RegistryKey subkey = localMachine32Key.OpenSubKey(keyName);
                                if (subkey != null && subkey.GetValueNames().Contains("DisplayName"))
                                {
                                    displayName = subkey.GetValue("DisplayName").ToString();
                                    if (string.IsNullOrEmpty(displayName))
                                        continue;
                                    isPresent = displayName.Contains(OneDriveDisplayName);
                                    if (isPresent)
                                    {
                                        Console.WriteLine("OneDrive is installed");
                                        return true;
                                    }
                                }
                            }
                        }

                        if (localMachine6432Nodekey != null)
                        {
                            Console.WriteLine("Looking into LocalMachine6432 Registry Key");
                            foreach (String keyName in localMachine6432Nodekey.GetSubKeyNames())
                            {
                                bool isPresent = false;
                                RegistryKey subkey = localMachine6432Nodekey.OpenSubKey(keyName);
                                if (subkey != null && subkey.GetValueNames().Contains("DisplayName"))
                                {
                                    displayName = subkey.GetValue("DisplayName").ToString();
                                    if (string.IsNullOrEmpty(displayName))
                                        continue;
                                    isPresent = displayName.Contains(OneDriveDisplayName);
                                    if (isPresent)
                                    {
                                        Console.WriteLine("OneDrive is installed");
                                        return true;
                                    }
                                }
                            }
                        }

                    }
                }

            }

            catch (ObjectDisposedException ob)
            {
                Console.WriteLine("The exception is caught {0}", ob);
            }
            catch (ArgumentNullException ob)
            {
                Console.WriteLine("The exception is caught {0}", ob);
            }
            catch (UnauthorizedAccessException ob)
            {
                Console.WriteLine("The exception is caught {0}", ob);
            }
            catch (IOException ob)
            {
                Console.WriteLine("The exception is caught {0}", ob);
            }
            catch (Exception e)
            {
                Console.WriteLine("The exception is caught {0}", e);
            }
            Console.WriteLine("Could not fetch OneDrive is installed or not, returing false");
            return false;
        }
    }
}
