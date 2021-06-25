using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace WipePOC
{

    /// <summary>
    /// Wipe class contains the Trim approach and overwriting sectors using the disk information methods.
    /// Wipe class inherits from DeviceIO class.
    /// </summary>
    class Wipe : DeviceIO
    {
        private static List<String> PhysicalDrives = new List<String>();
        private static Dictionary<String, Dictionary<String, Object>> PhysicalDriveInfo = new Dictionary<String, Dictionary<string, Object>>();
        private static Dictionary<int, List<Dictionary<String, Object>>> VolumesInfo = new Dictionary<int, List<Dictionary<string, object>>>();
        private static byte[] writeBuffer;
        private static byte[] readBuffer;
        private static int BytesPerSector;
        private static int sectorOffset;
        public static int sectorWriteSize;
        public static int sectorReadSize;
        public static ulong sectorStart;
        public static ulong sectorEnd;
        public static int sectorErrorCount = 0;

        /// <summary>
        /// The PerformTrim method locates and consolidates fragmented files on local volumes.
        /// </summary>
        /// <param name="driveLetter">The Volume label</param>
        /// <returns>
        /// true on successful defragmentation else false.
        /// </returns>
        private static bool PerformTrim(string driveLetter)
        {
            try
            {
                Process p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        Verb = "runas",
                        FileName = "Defrag.exe",
                        Arguments = driveLetter + " /L",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardErrorEncoding = Encoding.UTF8,
                        StandardOutputEncoding = Encoding.UTF8,
                        WindowStyle = ProcessWindowStyle.Normal,
                        RedirectStandardInput = true,
                    }
                };
                p.Start();
                //p.WaitForExit();
                string result = p.StandardOutput.ReadToEnd();
                Console.WriteLine(result);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
                return false;
            }
        }

        /// <summary>
        /// CheckTrim Checks for trim descriptor
        /// </summary>
        /// <param name="DiskIndex">Physical disk index</param>
        /// <returns>
        /// true if trim is enabled else false.
        /// </returns>
        private static bool CheckTrim(int DiskIndex)
        {
            DEVICE_TRIM_DESCRIPTOR TrimDesc = new DEVICE_TRIM_DESCRIPTOR();
            STORAGE_PROPERTY_QUERY StorageQuery = new STORAGE_PROPERTY_QUERY();
            uint BytesReturned = 0;

            StorageQuery.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceTrimProperty;
            StorageQuery.QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery;

            IntPtr StorageQueryPtr = Marshal.AllocHGlobal(Marshal.SizeOf(StorageQuery));
            IntPtr TrimDescPtr = Marshal.AllocHGlobal(Marshal.SizeOf(TrimDesc));

            Marshal.StructureToPtr(StorageQuery, StorageQueryPtr, false);
            Marshal.StructureToPtr(TrimDesc, TrimDescPtr, false);

            SafeFileHandle physicalDisk = DeviceIO.CreateFile(
                    "\\\\.\\PhysicalDrive" + DiskIndex,
                    GENERIC_READ,
                    FILE_SHARE_READ,
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    FILE_FLAG_NO_BUFFERING | FILE_FLAG_WRITE_THROUGH,
                    IntPtr.Zero);

            bool rs = DeviceIoControl(physicalDisk, IOCTL_STORAGE_QUERY_PROPERTY, StorageQueryPtr,
                                     (uint)Marshal.SizeOf(StorageQuery), TrimDescPtr, (uint)Marshal.SizeOf(TrimDesc),
                                     ref BytesReturned, IntPtr.Zero);
            if (rs == false)
            {
                throw new Exception();
            }
            TrimDesc = (DEVICE_TRIM_DESCRIPTOR)Marshal.PtrToStructure(TrimDescPtr, typeof(DEVICE_TRIM_DESCRIPTOR));
            return TrimDesc.TrimEnabled;
        }

        /// <summary>
        /// GetFreePartitionSpace gets the remaining space in each of the drives.
        /// </summary>
        /// <param name="PartName"></param>
        /// <returns>
        /// The percentage of the remaining space.
        /// </returns>
        private static string GetFreePartitionSpace(string PartName)
        {
            try
            {
                //return " ";
                String totalspace = "", freespace = "", freepercent = "";
                Double sFree = 0, sTotal = 0, sEq = 0;
                if (!PartName.Equals(""))
                {
                    WqlObjectQuery querySpace = new WqlObjectQuery("Select DeviceID,FreeSpace,Size, " +
                                                                                 "from Win32_LogicalDisk Where DeviceID='" +
                                                                                 PartName + "'");
                    ManagementClass mgmt = new ManagementClass("Win32_LogicalDisk");
                    //ManagementObjectSearcher getspace = new ManagementObjectSearcher(querySpace);
                    ManagementObjectCollection collection = mgmt.GetInstances();
                    ManagementObjectCollection.ManagementObjectEnumerator enumerate = collection.GetEnumerator();
                    //foreach(ManagementObject drive in getspace.Get())
                    {
                        while (enumerate.MoveNext())
                            if (enumerate.Current["DeviceID"].ToString().Equals(PartName))
                            {
                                freespace = enumerate.Current["FreeSpace"].ToString();
                                totalspace = enumerate.Current["Size"].ToString();
                                sFree = Convert.ToDouble(freespace);
                                sTotal = Convert.ToDouble(totalspace);
                                sEq = sFree * 100 / sTotal;
                                freepercent = (Math.Round((sTotal - sFree) / 1073741824, 2)).ToString() + " (" + Math.Round(sEq, 0).ToString() + " %)";
                                return freepercent;
                            }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "";
        }

        /// <summary>
        /// GetPartitionName gets the name of each volumes on the disk.
        /// </summary>
        /// <param name="DeviceID">Drive name</param>
        /// <returns>
        /// Volume names.
        /// </returns>
        private static String GetPartitionName(String DeviceID)
        {
            String Dependent = "", ret = "";
            ManagementObjectSearcher LogicalDisk = new ManagementObjectSearcher("Select * from Win32_LogicalDiskToPartition");
            foreach (ManagementObject drive in LogicalDisk.Get())
            {
                if (drive["Antecedent"].ToString().Contains(DeviceID))
                {
                    Dependent = drive["Dependent"].ToString();
                    ret = Dependent.Substring(Dependent.Length - 3, 2);
                    break;
                }

            }
            return ret;

        }
        /// <summary>
        /// RetriveDiskInfo gets all the disk information to calculate offset values of the volumes,
        /// to exclude the bootable partition and if trim is supported or not.
        /// </summary>
        private static void RetrieveDiskInfo()
        {
            WqlObjectQuery QueriedDrives = new WqlObjectQuery("Select Model,DeviceID,MediaType,BytesPerSector," +
                                                              "Size,Partitions,InterfaceType,Index,TotalSectors" +
                                                              " from Win32_DiskDrive");

            ManagementClass mgmtDisk = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection mgmtDiskCollection = mgmtDisk.GetInstances();
            ManagementObjectCollection.ManagementObjectEnumerator diskEnumerator = mgmtDiskCollection.GetEnumerator();

            ManagementObjectSearcher mgmtDiskSearch = new ManagementObjectSearcher(QueriedDrives);
            //foreach (ManagementObject managementObj in mgmtDiskSearch.Get())
            while (diskEnumerator.MoveNext())
            {
                if (diskEnumerator.Current["MediaType"].Equals("Fixed hard disk media"))
                {
                    PhysicalDrives.Add(diskEnumerator.Current["DeviceID"].ToString());
                    Dictionary<String, Object> tmp = new Dictionary<string, object>();
                    List<Dictionary<String, Object>> Volumes = new List<Dictionary<String, Object>>();

                    /*foreach (PropertyData property in diskEnumerator.Current.Properties)
                    {
                        tmp.Add(property.Name, property.Value);
                    }*/
                    tmp.Add("Model", diskEnumerator.Current["Model"]);
                    tmp.Add("DeviceID", diskEnumerator.Current["DeviceID"]);
                    tmp.Add("MediaType", diskEnumerator.Current["MediaType"]);
                    tmp.Add("BytesPerSector", diskEnumerator.Current["BytesPerSector"]);
                    tmp.Add("Size", diskEnumerator.Current["Size"]);
                    tmp.Add("Partitions", diskEnumerator.Current["Partitions"]);
                    tmp.Add("InterfaceType", diskEnumerator.Current["InterfaceType"]);
                    tmp.Add("Index", diskEnumerator.Current["Index"]);
                    tmp.Add("TotalSectors", diskEnumerator.Current["TotalSectors"]);

                    try
                    {
                        tmp.Add("TrimSupported", CheckTrim(int.Parse(diskEnumerator.Current["Index"].ToString())));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unable to execute DeviceIOControl call : " + e.Message);
                        Environment.Exit(-1);
                    }
                    PhysicalDriveInfo.Add(diskEnumerator.Current["DeviceID"].ToString(), tmp);

                    ManagementObjectSearcher partitions = new ManagementObjectSearcher(
                                                           "Select BootPartition,Bootable,DiskIndex,DeviceID," +
                                                           "PrimaryPartition,Size,StartingOffset,Type,Index" +
                                                           " from Win32_DiskPartition where DiskIndex ='" +
                                                           diskEnumerator.Current["Index"] + "'");

                    ManagementClass partitionMgt = new ManagementClass("Win32_DiskPartition");
                    ManagementObjectCollection partitionCollection = partitionMgt.GetInstances();
                    ManagementObjectCollection.ManagementObjectEnumerator partitionEnum = partitionCollection.GetEnumerator();

                    //foreach (ManagementObject part in partitions.Get())
                    while (partitionEnum.MoveNext())
                    {
                        String PartitionID = partitionEnum.Current["DeviceID"].ToString();
                        String PartName = GetPartitionName(PartitionID);

                        Dictionary<String, Object> tmpPartInfo = new Dictionary<string, object>();
                        if (partitionEnum.Current["Bootable"].ToString().Equals("True") && partitionEnum.Current["BootPartition"].ToString().Equals("True"))
                        {
                            tmpPartInfo.Add("Name", partitionEnum.Current["Index"]);
                            tmpPartInfo.Add("DiskIndex", partitionEnum.Current["DiskIndex"]);
                            tmpPartInfo.Add("Index", int.Parse(partitionEnum.Current["Index"].ToString().Substring(
                                            partitionEnum.Current["DiskIndex"].ToString().Length - 1)));
                            tmpPartInfo.Add("Type", "Recovery");
                            tmpPartInfo.Add("Size", partitionEnum.Current["Size"]);
                            tmpPartInfo.Add("StartingOffset", partitionEnum.Current["StartingOffset"]);
                            tmpPartInfo.Add("Erasable", false);
                        }
                        else
                        {
                            String FreeSpace = GetFreePartitionSpace(PartName);
                            if (PartName.ToString().Equals(""))
                                PartName = "Partition " + partitionEnum.Current["Index"].ToString();
                            else
                                PartName = "Local Disk ( " + PartName + " )";

                            /*foreach (PropertyData property in partitionEnum.Properties)
                            {
                                tmpPartInfo.Add(property.Name, property.Value);
                            }*/
                            tmpPartInfo.Add("BootPartition", partitionEnum.Current["BootPartition"]);
                            tmpPartInfo.Add("Bootable", partitionEnum.Current["Bootable"]);
                            tmpPartInfo.Add("DeviceID", partitionEnum.Current["DeviceID"]);
                            tmpPartInfo.Add("PrimaryPartition", partitionEnum.Current["PrimaryPartition"]);
                            tmpPartInfo.Add("Size", partitionEnum.Current["Size"]);
                            tmpPartInfo.Add("StartingOffset", partitionEnum.Current["StartingOffset"]);
                            tmpPartInfo.Add("Type", partitionEnum.Current["Type"]);
                            tmpPartInfo.Add("DiskIndex", partitionEnum.Current["DiskIndex"]);
                            tmpPartInfo.Add("Index", int.Parse(partitionEnum.Current["Index"].ToString().Substring(
                                            partitionEnum.Current["DiskIndex"].ToString().Length - 1)));
                            tmpPartInfo.Add("Name", PartName);
                            tmpPartInfo.Add("FreeSpace", FreeSpace);
                            tmpPartInfo.Add("Erasable", true);
                        }
                        Volumes.Add(tmpPartInfo);
                    }
                    VolumesInfo.Add(int.Parse(diskEnumerator.Current["Index"].ToString()), Volumes);
                }
            }
        }

        /// <summary>
        /// Displays all the disk information retrieved.
        /// </summary>
        private static void DisplayDiskVolInfo()
        {

            Console.WriteLine("\n\nPhysical Drives found in system\n");
            foreach (String Drive in PhysicalDrives)
            {
                Console.WriteLine("-> " + Drive + ":");
            }

            Console.WriteLine("\n\nDetailed Info for drives\n");
            foreach (String Key in PhysicalDriveInfo.Keys)
            {
                Console.WriteLine(Key.ToString() + ":\n");
                Dictionary<String, Object> tmpDiskInfo = new Dictionary<string, object>();
                List<Dictionary<String, Object>> tmpVolInfo = new List<Dictionary<string, object>>();
                tmpDiskInfo = PhysicalDriveInfo[Key];
                int index = int.Parse(tmpDiskInfo["Index"].ToString());
                foreach (String Info in tmpDiskInfo.Keys)
                {
                    if (Info.Equals("DeviceID"))
                        continue;
                    Console.WriteLine("\t{0,-15} :: \t{1}", Info.ToString(), tmpDiskInfo[Info].ToString());
                }
                Console.WriteLine("\n\n\tDetailed Volume Info....\n");
                Console.WriteLine("\tPartition\t\tFreeSpace\tTotalSpace\tPartitionType\t  StartingOffset\tShouldErase\n");
                Console.WriteLine("\t" + new String('-', 108) + "\n");
                tmpVolInfo = VolumesInfo[index];
                foreach (Dictionary<String, Object> val in tmpVolInfo)

                {
                    if (val.Count > 7)
                        Console.WriteLine("\t{0,-23}{1,-17}{2,-16}{3,-20}  {4,-22}{5}",
                                          val["Name"], val["FreeSpace"], val["Size"],
                                          val["Type"], val["StartingOffset"], val["Erasable"]);
                    else
                        Console.WriteLine("\tPartition {0,-25}\t{1,-16}{2,-20}  {3,-22}{4}",
                                          val["Name"], val["Size"], val["Type"], val["StartingOffset"], val["Erasable"]);
                }
                Console.WriteLine("\n\n");
            }
        }

        /// <summary>
        /// The Main method, chooses on Trim or overwritting and executes accordingly.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("***************************\n" +
                         "*\t\t\t  *\n*\tWipe Demo\t  *\n*\t\t\t  *\n" +
                              "***************************\n");
            Console.Write("Reading Drive Information");
            int i = 0;
            while (i++ < 4)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }
            RetrieveDiskInfo();
            DisplayDiskVolInfo();
            //BeginErase 
            /* {
                //while(diskIndices in List<Drives>) 
                {
                    //CheckTrim();
                    //GetOffsets();             
                    //UnmountVolumes();
                    //ExecuteTrim();
                    //ExecuteOverwrite();
                 }
               }
            */
            Console.WriteLine("Beginning Erase Procedure....Are you sure you want to continue Y/N ??");
            char input = (char)Console.Read();
            if (input == 'n' || input == 'N')
                Environment.Exit(-1);

            Console.WriteLine("Enter value to write within 0-255");
            int val = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter how many sectors to write at a time");
            sectorOffset = Convert.ToInt32(Console.ReadLine());

            writeBuffer = Enumerable.Repeat((byte)val, writeBuffer.Length).ToArray();

            foreach (Dictionary<String, Object> disk in PhysicalDriveInfo.Values)
            {
                bool doTrim = bool.Parse(disk["TrimSupported"].ToString());
                BytesPerSector = int.Parse(disk["BytesPerSector"].ToString());
                sectorWriteSize = BytesPerSector * sectorOffset;
                writeBuffer = new byte[sectorWriteSize];
                readBuffer = new byte[sectorReadSize];
                uint DiskIndex = (uint)disk["Index"];
                DeviceIO diskObj = new DeviceIO();
                List<Dictionary<String, Object>> volumes = VolumesInfo[(int)DiskIndex];

                diskObj.OpenFileObject(DiskIndex, FileAccess.ReadWrite);

                foreach (Dictionary<String, Object> volumeInfo in volumes)
                {
                    Regex regex = new Regex(@"\((.*?)\)");
                    Match tmpDriveName = regex.Match(volumeInfo["Name"].ToString());
                    String driveLetter = tmpDriveName.Value.Replace("(", "").Replace(")", "").Trim();
                    int volumeIndex = -1;
                    Console.WriteLine("Partition letter : " + driveLetter);
                    if (driveLetter.Equals(""))
                    {
                        volumeIndex = (int)volumeInfo["Index"];
                    }
                    if (bool.Parse(volumeInfo["Erasable"].ToString()) == true)
                    {
                        sectorStart = Convert.ToUInt64(volumeInfo["StartingOffset"]) / 512;
                       // startingOffsets.Add(Convert.ToString(volumeInfo["Name"]), sectorStart);

                        UInt64 sectorStartInBytes = Convert.ToUInt64(volumeInfo["StartingOffset"]);
                        UInt64 totalSpace = Convert.ToUInt64(volumeInfo["Size"]);
                        UInt64 sectorEndInBytes = sectorStartInBytes + totalSpace;
                        sectorEnd = (sectorStartInBytes + totalSpace) / 512;

                        // UInt64 sectorDiff = sectorEnd - sectorStart;
                        //writeBuffer = new Byte[1048576];
                        //writeBuffer = Enumerable.Repeat((byte)val, writeBuffer.Length).ToArray();
                        //readBuffer = new byte[1048576];
                        //endingOffsets.Add(Convert.ToString(volumeInfo["Name"]), sectorEnd);

                        diskObj.LockVolume(driveLetter);
                        diskObj.UnmountVolume(driveLetter);
                        if (doTrim)
                            PerformTrim(driveLetter);
                    }
                    diskObj.Write(writeBuffer, 0, writeBuffer.Length);
                    diskObj.Read(readBuffer, 0, readBuffer.Length);
                }
            }
        }
    }
}
