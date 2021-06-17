using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApps
{
    class Program
    {
        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcesses();
            // until max timeout delay
            // loop based on timer till <configurable seconds> OR till login status is known
            foreach (var proc in processes)
            {
                if (proc.ProcessName.Equals("OneDrive"))
                {
                    Console.WriteLine("OneDrive");
                    //if (!string.IsNullOrEmpty(proc.MainWindowTitle))
                    //{
                    //    // login required
                    //    Console.WriteLine(proc.MainWindowTitle); //Microsoft OneDrive
                    //}
                    //else // if (registry entry)
                    //{
                    //    // logged in
                    //}
                    //else 
                    //{
                    //    // login required 
                    //    // add delay and continue loop
                    //}
                }
            }
            // Delay
            Console.Read();
        }
    }
}
