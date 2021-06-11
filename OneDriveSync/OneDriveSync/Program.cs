using Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneDriveSync
{
    class Program
    {
        public static bool running = false;
        public static Timer timer = new Timer(Timer_Elapsed, null, Timeout.Infinite, Timeout.Infinite);
        public static Process currentRunningProcess = Process.GetCurrentProcess();
        public static OneDriveUtil oneDriveUtil = new OneDriveUtil();
        public static int timeout = 2000;
        public static int maxRetries = 30;
        public static int countRetries = 1;
        public static int maxTimeOutInSeconds = 60000;
        public static DateTime onsetTime;

        [STAThread]
        static void Main(string[] args)
        {
            oneDriveUtil.ShutdownOneDrive();
            Console.WriteLine("Shutdown complete");
            oneDriveUtil.StartOneDrive();
            Console.WriteLine("Startup complete");
            //timer.Change(5000, timeout);
            //onsetTime = DateTime.Now;
            //Console.WriteLine("You're done!");
            //DateTime currentTime = DateTime.Now;
            //var timeDifference = (currentTime - onsetTime).TotalSeconds;
            Console.ReadKey();
            //TODO: Variable to avoid it from being run multiple times in prechecks
        }

        static void MinimizeWindow(IntPtr handle)
        {
            const int SW_MINIMIZE = 6;
            ShowWindow(handle, SW_MINIMIZE);
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void Timer_Elapsed(object state)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite); // It will stop the timer
            //TODO: Start time - current time = configurableTime
            if (countRetries >= maxRetries)
            {
                timer.Dispose();
                Console.WriteLine("Timer disposed");
                //User not logged in
                return;
            }
            countRetries++;
            Console.WriteLine("HAH! TIME OUT!");
            bool loggedIn = false;
            bool signInError = false;
            WindowHelper windowHelper = new WindowHelper();

            Process[] processes = Process.GetProcesses();
            foreach (var proc in processes)
            {
                if (proc.ProcessName.Equals("OneDrive"))
                {
                    //windowHelper.BringProcessToFront(currentRunningProcess);
                    if (!string.IsNullOrEmpty(proc.MainWindowTitle)) // OneDrive log in screen is open
                    {
                        Console.WriteLine("Log in screen displayed: " + DateTime.Now);
                        windowHelper.BringProcessToFront(proc); //TODO: Bring login screen to front only once
                        timer.Change(timeout, timeout); 
                        break;
                    }
                    else
                    {
                        loggedIn = oneDriveUtil.IsOneDriveLoggedIn();
                        signInError = oneDriveUtil.IsOneDriveSignInError();
                        if (loggedIn && !signInError)
                        {
                            Console.WriteLine("Logged in " + DateTime.Now);
                            windowHelper.BringProcessToFront(currentRunningProcess);
                            //TODO: Windows Explorer close OneDrive if possible OR minimize
                            timer.Change(Timeout.Infinite, Timeout.Infinite); //Dispose timer probably?
                            // Intimate DDA
                        }
                        else
                        {
                            Console.WriteLine("Not signed in " + DateTime.Now);
                            windowHelper.BringProcessToFront(proc);
                            timer.Change(timeout, timeout);
                            //1 minu
                        }
                        break;
                    }
                }
            }
        }
    }
}
