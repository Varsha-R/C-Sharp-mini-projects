using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace UserStoryNotificationsPOC
{
    public class Program
    {
        private static readonly string notificationCaption = "Erase your PC";
        private static readonly string notificationText = "You can choose to snooze and be reminded at a later time";
        private static string notificationLogoPath = @"{0}\Assets\Dell-Logo.png";

        static void Main(string[] args)
        {
            notificationLogoPath = string.Format(notificationLogoPath, Directory.GetCurrentDirectory());
            ToastNotificationHelper.SendNewToastNotification(notificationCaption, notificationText, notificationLogoPath);

            //Task.Delay(120000);
            //Console.WriteLine("Wait of 2 seconds over");
            //ToastNotificationManager.History.Remove("TestNotify");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
