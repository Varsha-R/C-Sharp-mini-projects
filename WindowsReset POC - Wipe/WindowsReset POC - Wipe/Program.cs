using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsReset_POC___Wipe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("STARTING WINDOWS RESET");
            #region SYSRESET
            Process pSysReset = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Windows\Sysnative\cmd.exe",
                    Arguments = @"/C systemreset -factoryreset",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    WindowStyle = ProcessWindowStyle.Normal,
                    RedirectStandardInput = true,

                }
            };
            pSysReset.Start();
            //pSysReset.WaitForExit();
            Console.WriteLine("Triggered System Reset");
            #endregion

            #region Get DriveInfo
            //DriveInfo[] allDrives = DriveInfo.GetDrives();
            //foreach (DriveInfo d in allDrives)
            //{
            //    Console.WriteLine("Drive {0}", d.Name);
            //    Console.WriteLine("  Drive type: {0}", d.DriveType);
            //    if (d.IsReady == true)
            //    {
            //        Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
            //        Console.WriteLine("  File system: {0}", d.DriveFormat);
            //        Console.WriteLine(
            //            "  Available space to current user:{0, 15} bytes",
            //            d.AvailableFreeSpace);

            //        Console.WriteLine(
            //            "  Total available space:          {0, 15} bytes",
            //            d.TotalFreeSpace);

            //        Console.WriteLine(
            //            "  Total size of drive:            {0, 15} bytes ",
            //            d.TotalSize);
            //    }
            //}
            #endregion

            #region Optimize

            //try
            //{
            //    DriveInfo[] drives = DriveInfo.GetDrives();
            //    foreach (DriveInfo drive in drives)
            //    {
            //        string d = drive.ToString().Split('\\')[0];
            //        Console.WriteLine(d);
            //        Process p = new Process
            //        {
            //            StartInfo = new ProcessStartInfo
            //            {
            //                Verb = "runas",
            //                //FileName = @"C:\Windows\Sysnative\cmd.exe",
            //                FileName = "Defrag.exe",
            //                Arguments = "/C /L",
            //                UseShellExecute = false,
            //                CreateNoWindow = true,
            //                RedirectStandardOutput = true,
            //                RedirectStandardError = true,
            //                StandardErrorEncoding = Encoding.UTF8,
            //                StandardOutputEncoding = Encoding.UTF8,
            //                WindowStyle = ProcessWindowStyle.Normal,
            //                RedirectStandardInput = true,
            //            }
            //        };
            //        p.Start();
            //        //p.WaitForExit();
            //        string result = p.StandardOutput.ReadToEnd();
            //        Console.WriteLine(result);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Exception: {0}", ex);
            //}

            #endregion 

            Console.WriteLine("END");
            Console.ReadKey();
        }
    }
}
