using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchBrowserInHidden
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pInfo.CreateNoWindow = true;
            pInfo.FileName = @"C:\Program Files\Mozilla Firefox\firefox.exe";
            pInfo.WorkingDirectory = @"C:\Program Files\Mozilla Firefox";
            pInfo.Arguments = "";
            Process p = Process.Start(pInfo);
            Console.WriteLine("Launched");
            p.Kill();
            Console.WriteLine("Killed");
            Console.ReadKey();
        }
    }
}
