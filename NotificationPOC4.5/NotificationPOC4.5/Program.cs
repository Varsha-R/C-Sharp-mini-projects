using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace NotificationPOC4._5
{
    class Program
    {
        /// <summary>
        /// Application Model User Id to be associated with the toast notifcation application
        /// </summary>
        private static readonly string APP_ID = ConfigurationManager.AppSettings["AppModelUserID"];
        public static string dismissPath = @"{0}\Assets\Dismiss.png";
        public static string snoozePath = @"{0}\Assets\Snooze.png";
        public static string startErasePath = @"{0}\Assets\Start-Erase.png";

        /// <summary>
        /// Send out customized toast notifications
        /// </summary>
        /// <param name="notificationCaption">Caption that has to be given to the toast notification</param>
        /// <param name="notificationText">Text that should be displayed in the toast notification</param>
        /// <param name="notificationImagePath">Image that has to be displayed on the toast notification such as error, warning, or information image</param>
        /// <param name="toastTag">Tag that has to be assigned to the toast notification</param>
        /// <param name="toastGroup">Group to which the toast notification has to be associated with</param>
        public static bool SendNewToastNotification(string notificationCaption, string notificationText, string notificationImagePath, string toastTag, string toastGroup)
        {

            dismissPath = string.Format(dismissPath, Directory.GetCurrentDirectory());
            snoozePath = string.Format(snoozePath, Directory.GetCurrentDirectory());
            startErasePath = string.Format(startErasePath, Directory.GetCurrentDirectory());

            try
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode("Hello World!"));
                XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");

                ((XmlElement)toastImageAttributes[0]).SetAttribute("src", dismissPath);
                ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "red graphic");
                IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                ((XmlElement)toastNode).SetAttribute("duration", "long");
                ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"12345\",\"param2\":\"67890\"}");
                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
                Console.WriteLine("Toast sent");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to send new toast notifications due to Exception {0}", ex.StackTrace);
            }

            return true;
            //bool isElevated;
            //using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            //{
            //    WindowsPrincipal principal = new WindowsPrincipal(identity);
            //    isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            //}
            //Console.WriteLine("Running as admin: {0}", isElevated);

            //dismissPath = string.Format(dismissPath, Directory.GetCurrentDirectory());
            //snoozePath = string.Format(snoozePath, Directory.GetCurrentDirectory());
            //startErasePath = string.Format(startErasePath, Directory.GetCurrentDirectory());

            //// A shortcut in the start menu is required for sending out toast notifications using a console application
            //bool shortcutCreated = TryCreateShortcut();

            //bool newToastNotificationSent = false;
            //if (shortcutCreated)
            //{
            //    try
            //    {
            //        // Describes all of the properties and elements within the toast content XML payload
            //        string Toast = "<toast launch=\"developer-defined-string\">"
            //                     + "<visual>"
            //                     + "<binding template =\"ToastGeneric\">"
            //                     + "<text>" + notificationCaption + "</text>"
            //                     + "<image placement=\"appLogoOverride\" src=\"" + notificationImagePath + "\" />"
            //                     + "<text>" + notificationText+ "</text>"
            //                     + "</binding>"
            //                     + "</visual>"
            //                     + "<actions>"
            //                     + "<input id=\"inputTime\" type=\"selection\" defaultInput=\"15\">"
            //                     + "<selection id =\"15\" content=\"15 minutes\" />"
            //                     + "<selection id =\"60\" content=\"1 hour\" />"
            //                     + "<selection id =\"240\" content=\"4 hours\" />"
            //                     + "<selection id =\"1440\" content=\"1 day\" />"
            //                     + "</input>"
            //                     + "<action content=\"Snooze\" imageUri=\"" + snoozePath + "\" arguments=\"snooze\" activationType=\"background\"/>"
            //                     + "<action content=\"Erase now\" imageUri=\"" + startErasePath + "\" arguments=\"launch-app\" activationType=\"foreground\"/>"
            //                     + "<action content=\"Dismiss\" imageUri=\"" + dismissPath + "\" arguments=\"dismiss\" activationType=\"background\"/>"
            //                     + "</actions>"
            //                     + "</toast>";

            //        //string xml = XDocument.Load("/toast.xml").ToString();
            //        // Laoding XML into DOM for programatically reading, modifying, or removing XML in the document
            //        XmlDocument tileXml = new XmlDocument();
            //        //tileXml.LoadXml(XDocument.Load("toast.xml").ToString());
            //        tileXml.LoadXml(Toast);
            //        var toast = new ToastNotification(tileXml);

            //        // Attaching the event handlers
            //        toast.Activated += ToastActivated;
            //        toast.Dismissed += ToastDismissed;
            //        toast.Failed += ToastFailed;

            //        toast.ExpirationTime = DateTime.Now + TimeSpan.FromSeconds(3);

            //        // Assigning a tag to the toast using which it can be removed from action center
            //        toast.Tag = toastTag;

            //        // Assigning a group to the toast using which all the toasts under a group can be removed from action center
            //        toast.Group = toastGroup;

            //        // Tries to open a registry key for the AppModelUserId of the CurrentUser
            //        RegistryKey root = Registry.CurrentUser.OpenSubKey(ConfigurationManager.AppSettings["RegistryPath"] + APP_ID, false);

            //        // It creates a registry key if one does not already exists
            //        if (root == null)
            //        {
            //            Console.WriteLine("Creating registry key to persist notifications in Action Center");

            //            // Creates the registry key for the AppModelUserId
            //            Registry.CurrentUser.CreateSubKey(ConfigurationManager.AppSettings["RegistryPath"] + APP_ID);

            //            // Persists the notification in the action center
            //            Registry.CurrentUser.OpenSubKey(ConfigurationManager.AppSettings["RegistryPath"] + APP_ID, true).SetValue("ShowInActionCenter", 1, RegistryValueKind.DWord);
            //        }

            //        // Sends out the toast notification
            //        ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
            //        Console.WriteLine("New Toast Sent with notificationCaption={0}, notificationText={1}, notificationImagePath={2}, toastTag={3}, and toastGroup={4})", notificationCaption, notificationText, notificationImagePath, toastTag, toastGroup);
            //        newToastNotificationSent = true;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Failed to send new toast notifications due to Exception {0}", e.StackTrace);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Shortcut could not be created in the start menu");
            //}

            //Console.WriteLine("End Implementation: SendNewToastNotification(notificationCaption={0}, notificationText={1}, notificationImagePath={2}, toastTag={3}, toastGroup={4})", notificationCaption, notificationText, notificationImagePath, toastTag, toastGroup);
            //return newToastNotificationSent;
        }

        /// <summary>
        /// Creates a shortcut in the start menu of the exe file
        /// </summary>
        /// <returns>true if the shortcut is created and false if the shortcut is already present</returns>
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

        /// <summary>
        /// Creates a link file to the current executable and installs the shortcut at the path generated
        /// </summary>
        /// <param name="shortcutPath"> Shortcut path for installation of the shortcut</param>
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

        /// <summary>
        /// Event handler when the toast notifcation is activated
        /// </summary>
        /// <param name="e"> Holds the node structure for the entire toast </param>
        /// <param name="source"> Holds the details of the toast being activated</param>
        private static void ToastActivated(ToastNotification e, object source)
        {
            Console.WriteLine("Start Toast Activated\n");

            // Checks if source is of type ToastActivatedEventArgs
            if (source is ToastActivatedEventArgs selectedToast)
            {
                //ValueSet keyValues = selectedToast.UserInput;
                //foreach(KeyValuePair<string, object> key in keyValues)
                //{
                //    Console.WriteLine("User input: \t{0}", key.Value);
                //}
                // Checks for the argument property to give out details as to which action button was selected and logs the same
                Console.WriteLine("Toast Activated with action {0}", selectedToast.Arguments);
                switch(selectedToast.Arguments)
                {
                    case "launch-app":
                        Process.Start(@"C:\Users\Varsha.Ravindra\Downloads\3b3f9ec3_3332\ci-scripts\M2DEXE\Dell Data Assistant.exe");
                        Console.WriteLine("Process launched");
                        return;
                    case "snooze":
                        selectedToast.UserInput.TryGetValue("inputTime", out object userInput);
                        Console.WriteLine("User chose: {0}", userInput.ToString());
                        return;
                    case "dismiss":
                        return;
                }
            }
            Console.WriteLine("End Toast Activated");
        }

        /// <summary>
        /// Event handler when the toast notifcation is dismissed
        /// </summary>
        /// <param name="source"> Holds the details of the toast being dismissed </param>
        /// <param name="e"> Holds the details of how was the toast dismissed </param>
        private static void ToastDismissed(object source, ToastDismissedEventArgs e)
        {
            Console.WriteLine("Start Toast Dismissed");

            // Logs the reason for the dismissal of the toast
            Console.WriteLine("Toast Dismissed with reason {0}", e.Reason);
            switch (e.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    break;
                case ToastDismissalReason.UserCanceled:
                    break;
                case ToastDismissalReason.TimedOut:
                    break;
            }
            Console.WriteLine("End Toast Dismissed");
        }

        /// <summary>
        /// Event handler when the toast notifcation fails
        /// </summary>
        /// <param name="source"> Holds the details of the toast failing </param>
        /// <param name="e"> Holds the error code for the failed toast </param>
        private static void ToastFailed(object source, ToastFailedEventArgs e)
        {
            Console.WriteLine("Start Toast Failed");

            // Logs the errorcode of the failed toast
            Console.WriteLine("Toast Failed with ErrorCode {0}", e.ErrorCode);

            Console.WriteLine("End Toast Failed");

        }

        ///// <summary>
        ///// Remove toast notifications by tag from action center
        ///// </summary>
        ///// <param name="toastTag"> Tag assigned to the toast notification </param>
        ///// <param name="toastGroup"> Group associated with the toast notification</param>
        ///// <returns>true if notifications by tag were removed from action center, false otherwise</returns>
        public static bool RemoveToastNotificationByTag(string toastTag, string toastGroup)
        {
            Console.WriteLine("Start Implementation: RemoveToastNotificationByTag(toastTag={0}, toastGroup={1})", toastTag, toastGroup);

            bool toastNotificationRemovedByTag = false;

            try
            {
                // Gets all the notifications in the action center with the AppModelUserId
                var actionCenterToastNotifications = ToastNotificationManager.History.GetHistory(APP_ID);

                Console.WriteLine("Found {0} action center toast notifications", actionCenterToastNotifications.Count);

                if (actionCenterToastNotifications.Count > 0)
                {
                    // Removes the toast notification with this tag and group from the action center
                    ToastNotificationManager.History.Remove(toastTag, toastGroup, APP_ID);
                    Console.WriteLine("Removed notifications from action center with toastTag={0}, toastGroup={1}, and appModelUserID={2}", toastTag, toastGroup, APP_ID);
                }
                else
                {
                    Console.WriteLine("No notifications available in the action center with appModelUserID={0}", APP_ID);
                }
                toastNotificationRemovedByTag = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Removing Toast notifications by tag and group failed due to Exception={0}", e.StackTrace);
            }

            Console.WriteLine("End Implementation: RemoveToastNotificationByTag(toastTag={0}, toastGroup={1})", toastTag, toastGroup);
            return toastNotificationRemovedByTag;
        }

        ///// <summary>
        ///// Remove toast notifications by group from action center
        ///// </summary>
        ///// <param name="toastGroup">Group associated with the toast notification</param>
        ///// <returns>true if notifications by group were removed from action center, false otherwise</returns>
        public bool RemoveToastNotificationByGroup(string toastGroup)
        {
            Console.WriteLine("Start Implementation: RemoveToastNotificationByGroup(toastGroup={0})", toastGroup);

            bool toastNotificationRemovedByGroup = false;

            try
            {
                // Gets all the notifications in the action center with the AppModelUserId
                var actionCenterToastNotifications = ToastNotificationManager.History.GetHistory(APP_ID);

                Console.WriteLine("Found {0} action center toast notifications", actionCenterToastNotifications.Count);

                if (actionCenterToastNotifications.Count > 0)
                {
                    // Removes all the toast notification associated with this group from the action center
                    ToastNotificationManager.History.RemoveGroup(toastGroup, APP_ID);
                    Console.WriteLine("Removed notifications from action center with toastGroup={0}, and appModelUserID={1}", toastGroup, APP_ID);
                }
                else
                {
                    Console.WriteLine("No notifications available in the action center with appModelUserID={0}", APP_ID);
                }
                toastNotificationRemovedByGroup = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Removing Toast notifications by group failed due to Exception={0}", e.StackTrace);
            }

            Console.WriteLine("End Implementation: RemoveToastNotificationByGroup(toastGroup={0})", toastGroup);
            return toastNotificationRemovedByGroup;
        }

        static void Main(string[] args)
        {
            SendNewToastNotification("Erase your PC", "Would you like to erase your PC?", @"D:\Wallpapers\photo-1508970057347-0524a45ebdff.jfif", "Tag", "Group");
            Console.ReadKey();
        }
    }
}
