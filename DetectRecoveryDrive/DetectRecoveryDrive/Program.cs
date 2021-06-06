using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace DetectRecoveryDrive
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Checking if recovery partition exists .....\n");
            bool isRecoveryPartitionPresent = DetectRecoveryPartition();
            if(isRecoveryPartitionPresent)
            {
                Console.WriteLine("Recovery partition exists!!!");
            }
            else
            {
                Console.WriteLine("Recovery partition does not exist :(");
            }
            Console.ReadKey();
        }

        private static bool DetectRecoveryPartition()
        {
            string query = "Select * from Win32_DiskDrive where MediaType = 'Fixed Hard Disk Media'";
            ManagementObjectSearcher diskSearcher = new ManagementObjectSearcher(query);
            foreach (var disk in diskSearcher.Get())
            {
                uint diskIndex = (uint)disk["Index"];

                ArrayList listOfPartitions = new ArrayList();
                Process diskPartProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "diskpart",
                        RedirectStandardInput = true
                    }
                };
                diskPartProcess.Start();
                diskPartProcess.StandardInput.WriteLine("select disk " + diskIndex);
                diskPartProcess.StandardInput.WriteLine("list partition");
                diskPartProcess.StandardInput.WriteLine("exit");

                while (!diskPartProcess.StandardOutput.EndOfStream)
                {
                    string line = diskPartProcess.StandardOutput.ReadLine();
                    string[] lines = line.Split(' ');
                    line.TrimStart();
                    line.TrimEnd();
                    if (line.Contains("Partition"))
                    {
                        if (!line.Contains("#"))
                        {
                            listOfPartitions.Add(line.ElementAtOrDefault(12));
                        }
                    }
                }

                ArrayList typeList = new ArrayList();
                string output = null;
                for (int i = 0; i < listOfPartitions.Count; i++)
                {
                    diskPartProcess.Start();
                    diskPartProcess.StandardInput.WriteLine("select disk " + diskIndex);
                    diskPartProcess.StandardInput.WriteLine("list partition");
                    diskPartProcess.StandardInput.WriteLine("select partition " + listOfPartitions[i]);
                    diskPartProcess.StandardInput.WriteLine("detail partition");
                    diskPartProcess.StandardInput.WriteLine("exit");
                    while (!diskPartProcess.StandardOutput.EndOfStream)
                    {
                        output = diskPartProcess.StandardOutput.ReadLine();
                        if (output.StartsWith("Type"))
                        {
                            string typeID = output.Split(':')[1].Trim();
                            if (typeID.Equals("de94bba4-06d1-4d40-a16a-bfd50179d6ac"))
                            {
                                return true;
                            }
                        }
                    }
                }                
            }
            return false;
        }
    }
}
