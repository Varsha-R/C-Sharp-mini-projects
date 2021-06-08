using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace EstimatedTimePOC2
{
    class Program
    {
        private static SafeFileHandle handleValueWrite = null;
        private static SafeFileHandle handleValueRead = null;

        static void Main()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string name = "dummy.txt";
            string fileName = Path.Combine(currentDirectory, name);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            try
            {
                PerformReadWrite(fileName);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: \n", ex);
            }
            
            Console.ReadKey();
        }

        static unsafe void PerformReadWrite(string fileName)
        {
            uint sectorSize = 32768;
            uint loopSize = 32768;
            ulong totalSectorSize = (ulong)sectorSize * loopSize;
            byte[] writeBuffer = new byte[sectorSize];
            new Random().NextBytes(writeBuffer);
            byte[] readBuffer = new byte[sectorSize];
            uint BytesWritten = 0;
            uint BytesRead = 0;
            ulong bytesWritten = 0;
            ulong bytesRead = 0;
            ulong totalBytesToWrite = 511476787200;

            Stopwatch stopwatch = new Stopwatch();
            handleValueWrite = NativeCalls.CreateFile(fileName, NativeCalls.GENERIC_ALL, NativeCalls.FILE_SHARE_WRITE | NativeCalls.FILE_SHARE_READ, IntPtr.Zero, NativeCalls.CREATE_NEW, NativeCalls.FILE_ATTRIBUTE_NORMAL , IntPtr.Zero);
            if (handleValueWrite.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            fixed (byte* writeBufferPointer = writeBuffer)
            {
                stopwatch.Start();
                for (int i = 0; i < loopSize; i++)
                {
                    if (!NativeCalls.WriteFile(handleValueWrite, writeBufferPointer, sectorSize, &BytesWritten, IntPtr.Zero))
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    bytesWritten += BytesWritten;
                }
                stopwatch.Stop();
                Console.WriteLine("Write of {0} bytes successful", bytesWritten);
            }
            if(!NativeCalls.CloseHandle(handleValueWrite))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            TimeSpan timeSpanTestWrite = stopwatch.Elapsed;
            double timeForTestWrite = timeSpanTestWrite.TotalMilliseconds;
            PrintTimeformat(timeSpanTestWrite, "Time taken for writing "+ bytesWritten);
            double totalTestWrite = (timeForTestWrite * totalBytesToWrite) / (float)totalSectorSize;
            PrintTimeformat(TimeSpan.FromMilliseconds(totalTestWrite), "Total time taken for writing ");

            handleValueRead = NativeCalls.CreateFile(fileName, NativeCalls.GENERIC_ALL, NativeCalls.FILE_SHARE_WRITE | NativeCalls.FILE_SHARE_READ, IntPtr.Zero, NativeCalls.OPEN_EXISTING, NativeCalls.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            if (handleValueRead.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            fixed (byte* readBufferPointer = readBuffer)
            {
                bool verifyStatus = false;
                stopwatch.Restart();
                for (int i = 0; i < loopSize; i++)
                {
                    if (!NativeCalls.ReadFile(handleValueRead, readBufferPointer, sectorSize, &BytesRead, IntPtr.Zero))
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    verifyStatus = UnsafeCompare(readBuffer, writeBuffer);
                    bytesRead += BytesRead;
                }
                stopwatch.Stop();
                Console.WriteLine("\nRead of {0} bytes successful", bytesRead);
                //Console.WriteLine("Verify status: {0}", verifyStatus);
            }
            if(!NativeCalls.CloseHandle(handleValueRead))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            TimeSpan timeSpanTestRead = stopwatch.Elapsed;
            double timeForTestRead = timeSpanTestRead.TotalMilliseconds;
            PrintTimeformat(timeSpanTestRead, "Time taken for reading "+ bytesRead);
            double totalTestRead = (timeForTestRead * totalBytesToWrite) / (float)totalSectorSize;
            PrintTimeformat(TimeSpan.FromMilliseconds(totalTestRead), "Total time taken for reading ");

            //double totalTimeRequired = ((timeForTestWrite + timeForTestRead) * totalBytesToWrite) / (sectorSize * sectorSize);
            TimeSpan timeSpanTotal = TimeSpan.FromMilliseconds(totalTestWrite + totalTestRead);
            PrintTimeformat(timeSpanTotal, "\nTOTAL TIME REQUIRED FOR ERASE");
            Console.WriteLine("\nMilliseconds: {0}", totalTestRead + totalTestWrite);
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
