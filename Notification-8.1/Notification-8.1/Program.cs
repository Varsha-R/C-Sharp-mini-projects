using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Notification_8._1
{
    class Program
    {
        private static readonly string APP_ID = ConfigurationManager.AppSettings["AppModelUserID"];
        private static string notificationLogoPath = @"{0}\Assets\logo.PNG";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            notificationLogoPath = string.Format(notificationLogoPath, Directory.GetCurrentDirectory());
            Console.WriteLine(notificationLogoPath);
            bool shortcutCreated = TryCreateShortcut();

            var template = ToastTemplateType.ToastImageAndText02;
            var toastXml = ToastNotificationManager.GetTemplateContent(template);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("Dell Data Assistant"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode("Time to erase your PC"));

            var toastImageElements = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageElements[0]).SetAttribute("src", "/Assets/logo.PNG");
            //((Windows.Data.Xml.Dom.XmlElement)toastImageElements[0]).SetAttribute("alt", "red graphic");

            var toastNode = toastXml.GetElementsByTagName("toast");
            ((XmlElement)toastNode[0]).SetAttribute("launch", "{\"type\":\"toast\"}");

            var toast = new ToastNotification(toastXml);
            toast.Activated += ToastActivated;
            var toastNotifier = ToastNotificationManager.CreateToastNotifier(APP_ID);
            toastNotifier.Show(toast);

            //string Toast = "<toast launch=\" \">"
            //                     + "<visual>"
            //                     + "<binding template =\"ToastImageAndText01\">"
            //                     + "<image id=\"1\" src=\"" + notificationLogoPath + "\" />"
            //                     + "<text id=\"1\">Hola!</text>"
            //                     + "</binding>"
            //                     + "</visual>"
            //                     + "</toast>";
            //XmlDocument tileXml = new XmlDocument();
            //tileXml.LoadXml(Toast);
            //var toast = new ToastNotification(tileXml);
            //ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);

            Console.ReadKey();
        }

        private static void ToastActivated(ToastNotification e, object source)
        {
            Console.WriteLine("Toast activated");
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Console.WriteLine(path);
            //Process.Start(path);
            Console.WriteLine("Process launched");
        }

        public static bool TryCreateShortcut()
        {
            Console.WriteLine("Trying to create a shortcut of the application on the start menu");

            try
            {
                // Generates a shortcut path for the app
                String shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + ConfigurationManager.AppSettings["PathToProgramsInStartMenu"] + ConfigurationManager.AppSettings["AppName"];
                if (!File.Exists(shortcutPath))
                {
                    Console.WriteLine("Shortcut of the application does not exists on the start menu bearing path={0}", shortcutPath);

                    // Installs the shorcut at the path by creating the link to the current executable
                    bool shortcutInstalled = InstallShortcut(shortcutPath);

                    if (shortcutInstalled)
                    {
                        if (File.Exists(shortcutPath))
                        {
                            Console.WriteLine("Shortcut of the application successfully created on the start menu bearing path={0}", shortcutPath);
                        }
                        else
                        {
                            Console.WriteLine("Shortcut  of the application not created on the start menu bearing path={0}", shortcutPath);
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Shortcut  of the application not installed on the start menu bearing path={0}", shortcutPath);
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Shortcut of the application already exists on the start menu bearing path={0}", shortcutPath);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Trying to create shortcut failed due to Exception={0}", e.StackTrace);
            }
            return false;
        }

        private static bool InstallShortcut(String shortcutPath)
        {
            Console.WriteLine("Installing a shortcut of the application on the start menu with path={0}", shortcutPath);

            try
            {
                // Finds the path to the current executable
                String exePath = Process.GetCurrentProcess().MainModule.FileName;
                IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

                Console.WriteLine("Current executable path={0}", exePath);

                // Create a shortcut to the executable
                ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
                ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

                // Opens the shortcut property store and sets the AppUserModelId property
                IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

                using (PropVariant appId = new PropVariant(APP_ID))
                {
                    ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                    ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
                }

                // Commits the shortcut to disk
                IPersistFile newShortcutSave = (IPersistFile)newShortcut;

                ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));

                Console.WriteLine("Installation of the shortcut of the application on the start menu with path={0} completed", shortcutPath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Insalling the shortcut failed due to Exception={0}", e.StackTrace);
            }
            return false;
        }
    }
}
