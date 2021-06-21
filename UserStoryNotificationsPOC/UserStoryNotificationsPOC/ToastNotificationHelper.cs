using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace UserStoryNotificationsPOC
{
    public class ToastNotificationHelper
    {
        private static System.Timers.Timer terminateToastNotification;
        private static readonly string APP_ID = ConfigurationManager.AppSettings["AppModelUserID"];
        //public static string dismissPath = @"C:\Users\Varsha.Ravindra\Downloads\Notification tray\Notification tray\png\Dismiss.png";
        //public static string snoozePath = @"C:\Users\Varsha.Ravindra\Downloads\Notification tray\Notification tray\png\Snooze.png";
        //public static string startErasePath = @"C:\Users\Varsha.Ravindra\Downloads\Notification tray\Notification tray\png\Start-Erase.png";

        public static string dismissPath = @"{0}\Assets\Dismiss.png";
        public static string snoozePath = @"{0}\Assets\Snooze.png";
        public static string startErasePath = @"{0}\Assets\Start-Erase.png";

        private static bool toastUsed = false;

        public static bool SendNewToastNotification(string notificationCaption, string notificationText, string notificationImagePath)
        {
            bool shortcutCreated = TryCreateShortcut();

            dismissPath = string.Format(dismissPath, Directory.GetCurrentDirectory());
            snoozePath = string.Format(snoozePath, Directory.GetCurrentDirectory());
            startErasePath = string.Format(startErasePath, Directory.GetCurrentDirectory());

            bool newToastNotificationSent = false;
            if (shortcutCreated)
            {
                try
                {
                    // Describes all of the properties and elements within the toast content XML payload
                    //string Toast = "<toast launch=\"developer-defined-string\">"
                    //             + "<visual>"
                    //             + "<binding template =\"ToastGeneric\">"
                    //             + "<text>" + notificationCaption + "</text>"
                    //             + "<image placement=\"appLogoOverride\" src=\"" + notificationImagePath + "\" />"
                    //             + "<text>" + notificationText + "</text>"
                    //             + "</binding>"
                    //             + "</visual>"
                    //             + "<actions>"
                    //             + "<input id=\"inputTime\" type=\"selection\" defaultInput=\"1\">"
                    //             + "<selection id =\"1\" content=\"1 hour\" />"
                    //             + "<selection id =\"6\" content=\"6 hours\" />"
                    //             + "<selection id =\"12\" content=\"12 hours\" />"
                    //             + "<selection id =\"24\" content=\"1 day\" />"
                    //             + "<selection id =\"72\" content=\"3 days\" />"
                    //             //+ "<selection id =\"168\" content=\"1 week\" />"
                    //             + "</input>"
                    //             + "<action content=\"Snooze\" imageUri=\"" + snoozePath + "\" arguments=\"snooze\" activationType=\"background\"/>"
                    //             + "<action content=\"Erase now\" imageUri=\"" + startErasePath + "\" arguments=\"start-erase\" activationType=\"foreground\"/>"
                    //             + "<action content=\"Dismiss\" imageUri=\"" + dismissPath + "\" arguments=\"dismiss\" activationType=\"background\"/>"
                    //             + "</actions>"
                    //             + "</toast>";

                    string notificationTextMultipleDDA = "Looks like you have DDA open. This is your reminder to erase your PC. You can set another reminder if you like.";
                    string Toast = "<toast launch=\"dda-launch\">"
                                    + "<visual>"
                                    + "<binding template =\"ToastGeneric\">"
                                    + "<text>" + notificationCaption + "</text>"
                                    + "<image placement=\"appLogoOverride\" src=\"" + notificationImagePath + "\" />"
                                    + "<text>" + notificationTextMultipleDDA + "</text>"
                                    + "</binding>"
                                    + "</visual>"
                                    + "</toast>";

                    // Laoding XML into DOM for programatically reading, modifying, or removing XML in the document
                    XmlDocument tileXml = new XmlDocument();
                    tileXml.LoadXml(Toast);
                    var toast = new ToastNotification(tileXml);

                    //toast.ExpirationTime = DateTime.Now + TimeSpan.FromSeconds(5);

                    // Attaching the event handlers
                    toast.Activated += ToastActivated;
                    toast.Dismissed += ToastDismissed;
                    toast.Failed += ToastFailed;

                    // Assigning a tag to the toast using which it can be removed from action center
                    toast.Tag = "TestNotify";

                    // Assigning a group to the toast using which all the toasts under a group can be removed from action center
                    toast.Group = "TestNotifyGroup";

                    // Tries to open a registry key for the AppModelUserId of the CurrentUser
                    RegistryKey root = Registry.CurrentUser.OpenSubKey(ConfigurationManager.AppSettings["RegistryPath"] + APP_ID, false);

                    // It creates a registry key if one does not already exists
                    if (root == null)
                    {
                        Console.WriteLine("Creating registry key to persist notifications in Action Center");

                        // Creates the registry key for the AppModelUserId
                        Registry.CurrentUser.CreateSubKey(ConfigurationManager.AppSettings["RegistryPath"] + APP_ID);

                        // Persists the notification in the action center
                        Registry.CurrentUser.OpenSubKey(ConfigurationManager.AppSettings["RegistryPath"] + APP_ID, true).SetValue("ShowInActionCenter", 0, RegistryValueKind.DWord);
                    }

                    // Sends out the toast notification
                    ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);

                    double notificationExpirationTime = Convert.ToDouble(ConfigurationManager.AppSettings["NotificationExpirationTimeInMinutes"]);
                    terminateToastNotification = new System.Timers.Timer(notificationExpirationTime * 60 * 1000);
                    terminateToastNotification.Elapsed += TerminateToastNotification_Elapsed;
                    terminateToastNotification.AutoReset = false;
                    terminateToastNotification.Enabled = true;

                    Console.WriteLine("New Toast Sent with notificationCaption={0}, notificationText={1}, notificationImagePath={2})", notificationCaption, notificationText, notificationImagePath);
                    newToastNotificationSent = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to send new toast notifications due to Exception {0}", e.StackTrace);
                }
            }
            else
            {
                Console.WriteLine("Shortcut could not be created in the start menu");
            }

            Console.WriteLine("End Implementation: SendNewToastNotification(notificationCaption={0}, notificationText={1}, notificationImagePath={2}", notificationCaption, notificationText, notificationImagePath);
            return newToastNotificationSent;
        }

        private static void TerminateToastNotification_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(!toastUsed)
            {
                RemoveToastNotificationByTag("TestNotify", "TestNotifyGroup");
                Console.WriteLine("SChedule toast on every system boot");
            }
            terminateToastNotification.Stop();
            terminateToastNotification.Dispose();
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

        private static void ToastActivated(ToastNotification e, object source)
        {
            Console.WriteLine("Start Toast Activated\n");
            toastUsed = true;
            // Checks if source is of type ToastActivatedEventArgs
            if (source is ToastActivatedEventArgs selectedToast)
            {
                ValueSet keyValues = selectedToast.UserInput;
                foreach (KeyValuePair<string, object> key in keyValues)
                {
                    Console.WriteLine("User input: \t{0}", key.Value);
                }
                // Checks for the argument property to give out details as to which action button was selected and logs the same
                Console.WriteLine("Toast Activated with action {0}", selectedToast.Arguments);
                switch (selectedToast.Arguments)
                {
                    //case "start-erase":
                    //    //Delete scheduled task
                    //    string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    //    Console.WriteLine(path);
                    //    Process.Start(path);
                    //    Console.WriteLine("Process launched");
                    //    return;
                    case "snooze":
                        selectedToast.UserInput.TryGetValue("inputTime", out object userInput);
                        Console.WriteLine("User chose: {0}", userInput.ToString());
                        new TaskScheduler().ScheduleTask(userInput.ToString());
                        return;
                    case "dismiss":   
                        //Notify on every system boot
                        return;
                    default:
                        //Delete scheduled task
                        string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        Console.WriteLine(path);
                        Process.Start(path);
                        Console.WriteLine("Process launched");
                        return;
                }
            }
            Console.WriteLine("End Toast Activated");
        }

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
                    toastUsed = true;
                    break;
            }
            Console.WriteLine("End Toast Dismissed");
        }

        private static void ToastFailed(object source, ToastFailedEventArgs e)
        {
            Console.WriteLine("Start Toast Failed");

            // Logs the errorcode of the failed toast
            Console.WriteLine("Toast Failed with ErrorCode {0}", e.ErrorCode);

            Console.WriteLine("End Toast Failed");

        }

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
    }
}
