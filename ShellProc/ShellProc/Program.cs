using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace closeCtrlPanel
{
    class Program
    {
        private static void CloseExplorerWindows()
        {
            Shell32.Shell shell = new Shell32.Shell();
            // ref: Shell.Windows method
            // https://msdn.microsoft.com/en-us/library/windows/desktop/bb774107(v=vs.85).aspx
            System.Collections.IEnumerable windows = shell.Windows() as System.Collections.IEnumerable;
            int count = 0;
           
            Console.WriteLine(count);
            if (windows != null)
            {
                // ref: ShellWindows object
                // https://msdn.microsoft.com/en-us/library/windows/desktop/bb773974(v=vs.85).aspx
                int i = 0;
                foreach (SHDocVw.InternetExplorer window in windows)
                {
                    //Console.WriteLine(window.LocationName);
                    object doc = window.Document;
                    //Console.WriteLine(doc);
                    if (doc != null && doc is Shell32.ShellFolderView)
                    {
                        Console.WriteLine(window.GetHashCode());
                        Console.WriteLine(window.LocationName);
                        Console.WriteLine();
                        //Console.WriteLine(window.FullName);
                        if (window.LocationName == "Control Panel")
                        {
                            Console.WriteLine("CONTROL PANEL");
                            //window.Quit();
                        }   // closes the window
                    }
                }
                Console.ReadKey();
            }
        }
        static void Main()
        {
            Thread staThread = new Thread(CloseExplorerWindows);
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
    }
}
