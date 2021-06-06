using System;
using System.IO;
using System.Management;

namespace DefragAPI
{
    // Refer System.Management
    // Project Properties -> Build -> Platform Target: Any CPU <uncheck all other options>
    class Program
    {
        static void Main(string[] args)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                string driveType = d.DriveType.ToString();
                if (driveType.Equals("Fixed"))
                {
                    string driveName = d.Name;
                    string drive = driveName.Remove(driveName.Length - 1);
                    Console.WriteLine("Drive Name- {0}", drive);
                    try
                    {
                        ConnectionOptions mgmtConnOptions = new ConnectionOptions { EnablePrivileges = true };
                        ManagementScope scope = new ManagementScope(new ManagementPath(string.Format(@"\\{0}\root\CIMV2", Environment.MachineName)), mgmtConnOptions);
                        ObjectQuery query = new ObjectQuery(string.Format(@"SELECT * FROM Win32_Volume WHERE Name = '{0}\\'", drive));
                        scope.Connect();
                        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                        {
                            object[] outputArgs = new object[2];
                            //if (drive != "C:")
                            //{
                                foreach (ManagementObject moVolume in searcher.Get())
                                {
                                    Console.WriteLine("Defragmenting drive- {0}", drive);
                                    Console.WriteLine("PLEASE WAIT!");
                                    ManagementBaseObject inParams = moVolume.GetMethodParameters("Defrag");
                                    ManagementBaseObject outParams = moVolume.InvokeMethod("Defrag", inParams, null);
                                    Console.WriteLine("Defragmenting {0} complete", drive);
                                    Console.WriteLine("ReturnValue: {0}", outParams["ReturnValue"]);
                                }
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex);
                    }
                }
                else
                {
                    Console.WriteLine("Drive Type- {0}. It is not fixed", driveType);
                }
            }
            Console.ReadKey();
        }
    }
}
