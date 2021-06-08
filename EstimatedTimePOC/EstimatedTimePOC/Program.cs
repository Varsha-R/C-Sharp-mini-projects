using EstimatedTimePOC.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace EstimatedTimePOC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting total bytes to erase.....");
            ulong totalBytesToWrite = GetTotalBytesToWrite();
            //ulong totalBytesToWrite = 511476787200;
            Console.WriteLine("Total bytes to erase: {0}\n", totalBytesToWrite);
            Console.WriteLine("Calculating estimated time .....");
            CalculateEstimatedTimeToWrite(totalBytesToWrite);
            Console.ReadKey();
        }

        private static ulong GetTotalBytesToWrite()
        {
            ulong totalSize = 0; 
            string query = "Select * from Win32_DiskDrive where MediaType = 'Fixed Hard Disk Media'";
            ManagementObjectSearcher diskSearcher = new ManagementObjectSearcher(query);
            foreach (var disk in diskSearcher.Get())
            {
                Disk newDisk = new Disk
                {
                    Index = (uint)disk["Index"],
                    BytesPerSector = (uint)disk["BytesPerSector"],
                    Size = (ulong)disk["Size"]
                };

                ArrayList listOfPartitions = new ArrayList();
                Process diskPartProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = @"C:\Windows\Sysnative\diskpart.exe",
                        RedirectStandardInput = true
                    }
                };
                diskPartProcess.Start();
                diskPartProcess.StandardInput.WriteLine("select disk " + newDisk.Index);
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

                ArrayList offsetList = new ArrayList();
                ArrayList typeList = new ArrayList();
                string output = null;
                for (int i = 0; i < listOfPartitions.Count; i++)
                {
                    diskPartProcess.Start();
                    diskPartProcess.StandardInput.WriteLine("select disk " + newDisk.Index);
                    diskPartProcess.StandardInput.WriteLine("list partition");
                    diskPartProcess.StandardInput.WriteLine("select partition " + listOfPartitions[i]);
                    diskPartProcess.StandardInput.WriteLine("detail partition");
                    diskPartProcess.StandardInput.WriteLine("exit");
                    while (!diskPartProcess.StandardOutput.EndOfStream)
                    {
                        output = diskPartProcess.StandardOutput.ReadLine();
                        if (output.StartsWith("Offset"))
                        {
                            offsetList.Add(output.Split(':')[1].Trim());
                        }
                        if (output.StartsWith("Type"))
                        {
                            typeList.Add(output.Split(':')[1].Trim());
                        }
                    }
                }

                var dictionary = new Dictionary<int, object>();
                var offsetDict = new Dictionary<int, object>();
                for (int j = 0; j < listOfPartitions.Count; j++)
                {
                    offsetDict.Add(int.Parse(listOfPartitions[j].ToString()), offsetList[j]);
                    dictionary.Add(int.Parse(listOfPartitions[j].ToString()), typeList[j]);
                }

                ulong sectorEnd = 0;
                ulong sectorStart = 0;
                for (int i = 0; i < offsetDict.Count; i++)
                {
                    var item = offsetDict.ElementAt(i);
                    sectorStart = ulong.Parse(item.Value.ToString()) / 512;
                    ulong secondItem = 0;
                    if (i == offsetDict.Count - 1)
                        secondItem = newDisk.Size;
                    else
                        secondItem = ulong.Parse(offsetDict.ElementAt(i + 1).Value.ToString());
                    sectorEnd = secondItem / 512;

                    Partition newPartition = new Partition
                    {
                        Index = int.Parse(item.Key.ToString()),
                        StartSector = sectorStart,
                        EndSector = sectorEnd
                    };
                    newDisk.Partition.Add(newPartition);
                }

                foreach (Partition part in newDisk.Partition)
                {
                    if (dictionary.TryGetValue(part.Index, out object actualValue))
                        if (actualValue.Equals("ebd0a0a2-b9e5-4433-87c0-68b6b72699c7") || actualValue.Equals("e3c9e316-0b5c-4db8-817d-f92df00215ae"))
                            part.Erasable = true;
                        else
                            part.Erasable = false;
                }

                List<Partition> normalPartitionCollection = newDisk.Partition.Where(x => x.Erasable == true).ToList();
                foreach (Partition part in normalPartitionCollection)
                {
                    totalSize += ((part.EndSector - part.StartSector) * newDisk.BytesPerSector);
                }
            }
            return totalSize;
        }

        private static void CalculateEstimatedTimeToWrite(ulong totalBytesToWrite)
        {
            // string fileName = @"D:\Rebit - Wipe\Dummy\dummy.txt";
            string currentDirectory = Directory.GetCurrentDirectory();
            string name = "dummy.txt";
            string fileName = Path.Combine(currentDirectory, name);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            int numberOfBytes = 1073741824; //1 GB
            byte[] byteAray = new byte[numberOfBytes]; 
            new Random().NextBytes(byteAray);

            Stopwatch stopwatch = new Stopwatch();
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {                                
                stopwatch.Start();
                fileStream.Write(byteAray, 0, byteAray.Length);
                //for (int i = 0; i < byteAray.Length; i++)
                //{
                //    fileStream.WriteByte(byteAray[i]);
                //}
                stopwatch.Stop();
            }
            TimeSpan timeSpanTestWrite = stopwatch.Elapsed;
            double timeForTestWrite = timeSpanTestWrite.TotalMilliseconds;
            PrintTimeformat(timeSpanTestWrite, "Time taken for writing");

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                byte[] testByteAray = new byte[numberOfBytes];
                stopwatch.Restart();
                fileStream.Read(testByteAray, 0, testByteAray.Length);
                UnsafeCompare(testByteAray, byteAray);
                //for (int i = 0; i < byteAray.Length; i++)
                //{
                //    int b = fileStream.ReadByte();
                //if (byteAray[i] != fileStream.ReadByte())
                //{
                //    continue;
                //}
                //}
                stopwatch.Stop();
            }
            TimeSpan timeSpanTestRead = stopwatch.Elapsed;
            double timeForTestRead = timeSpanTestRead.TotalMilliseconds;
            PrintTimeformat(timeSpanTestRead, "Time taken for reading");

            double totalTimeRequired = ((timeForTestWrite + timeForTestRead) * totalBytesToWrite) / numberOfBytes;
            TimeSpan timeSpanTotal = TimeSpan.FromMilliseconds(totalTimeRequired);
            PrintTimeformat(timeSpanTotal, "\nTOTAL TIME REQUIRED FOR ERASE");
        }

        static unsafe bool UnsafeCompare(byte[] a1, byte[] a2)
        {
            if (a1 == a2) return true;
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2)) return false;
                if ((l & 4) != 0) { if (*((int*)x1) != *((int*)x2)) return false; x1 += 4; x2 += 4; }
                if ((l & 2) != 0) { if (*((short*)x1) != *((short*)x2)) return false; x1 += 2; x2 += 2; }
                if ((l & 1) != 0) if (*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }

        static void PrintTimeformat(TimeSpan timeSpan, string outputString)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}",
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            Console.WriteLine("{0} - {1}", outputString, elapsedTime);
        }
    }
}
