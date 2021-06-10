using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinimizeProcess
{
    class Program
    {
        private const int SW_MAXIMIZE = 3;
        private const int SW_HIDE = 0;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_RESTORE = 9;

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //static void Main(string[] args)
        //{
        //    Process[] processes = Process.GetProcesses();
        //    foreach (var proc in processes)
        //    {
        //        if (proc.ProcessName.Contains("SupportAssist"))
        //        {
        //            Console.WriteLine("MainWindowTitle", proc.MainWindowTitle);
        //            IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, proc.MainWindowTitle);
        //            //ShowWindow(proc.Handle, SW_MINIMIZE);
        //        }
        //    }
        //    Console.WriteLine("Minimizing SupportAssist window");
        //    //MinimizeWindow();
        //    Console.ReadKey();
        //}

        /// <summary>
        /// filter function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        /// <summary>
        /// check if windows visible
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// return windows text
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpWindowText"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        /// <summary>
        /// enumarator on all desktop windows
        /// </summary>
        /// <param name="hDesktop"></param>
        /// <param name="lpEnumCallbackFunction"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);


        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;


        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);
        /// <summary>
        /// entry point of the program
        /// </summary>
        static void Main()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
            //System.Threading.Thread.Sleep(2000);
            //SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);

            int index = 0;
            var collection = new List<string>();
            var handles = new List<IntPtr>();
            EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
            {
                StringBuilder strbTitle = new StringBuilder(255);
                int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                string strTitle = strbTitle.ToString();

                if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                {
                    collection.Add(strTitle);
                    handles.Add(hWnd);
                }
                return true;
            };
            if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].Contains("SupportAssist") && !collection[i].Contains("CDM") && !collection[i].Contains("SupportAssistAgent"))
                    {
                        Console.WriteLine(collection[i]);
                        Console.WriteLine("SA restoring", handles[i]);
                        ShowWindow(handles[i], SW_SHOWNORMAL);
                    }
                    Console.WriteLine(collection[i] + " " + handles[i]);
                }
            }
            Thread.Sleep(2000);
            if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (string.Equals(collection[i], "Microsoft OneDrive")) 
                    {
                        Console.WriteLine(collection[i]);
                        Console.WriteLine("BEWAREEEEE ", handles[i]);
                        ShowWindow(handles[i], SW_RESTORE);
                        SetActiveWindow(handles[i]);
                        if (!SetForegroundWindow(handles[i]))
                        {
                            Console.WriteLine("Couldn't set foreground window");
                        }
                        break;
                    }
                    Console.WriteLine(collection[i] + " " + handles[i]);
                }
            }
            Console.WriteLine("SECOND  ");
            Console.Read();
        }
    }
}
