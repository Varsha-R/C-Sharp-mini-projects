using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace WipePOC
{
    /// <summary>
    /// DeviceIO class contains all disk write/read related functions from kernel32.dll and the 
    /// macros are explicitely defined with their hex values.
    /// </summary>
    class DeviceIO : Stream
    {
        // Read/Write buffers

        static byte[] writeBytes;
        static byte[] readBytes;
        static byte[] toWrite;

        // WINIOCTL.H MACRO VALUES

        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint FILE_SHARE_DELETE = 0x00000004;
        public const uint OPEN_EXISTING = 3;

        public const uint FILE_BEGIN = 0;
        public const uint FILE_CURRENT = 1;
        public const uint FILE_END = 2;

        public const uint GENERIC_READ = (0x80000000);
        public const uint GENERIC_WRITE = (0x40000000);

        public const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        public const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
        public const uint FILE_READ_ATTRIBUTES = (0x0080);
        public const uint FILE_WRITE_ATTRIBUTES = 0x0100;
        public const uint ERROR_INSUFFICIENT_BUFFER = 122;
        public const uint FSCTL_LOCK_VOLUME = 0x00090018;
        public const uint FSCTL_DISMOUNT_VOLUME = 0x00090020;

        public const uint IOCTL_STORAGE_QUERY_PROPERTY = 0x2d1400;


        // IMPORTING WINDOWS FILE HANDLING FUNCTIONS
        // Prototypes to functions defined in windows.h

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static unsafe extern SafeFileHandle CreateFile(
            string FileName,
            uint DesiredAccess,
            uint ShareMode,
            IntPtr SecurityAttributes,
            uint CreationDisposition,
            uint FlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool CloseHandle(SafeFileHandle hHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            [Out] IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern unsafe bool WriteFile(
            SafeFileHandle hFile,
            byte* pBuffer,
            uint NumberOfBytesToWrite,
            uint* pNumberOfBytesWritten,
            IntPtr Overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern unsafe bool ReadFile(
            SafeFileHandle hFile,
            byte* pBuffer,
            uint NumberOfBytesToRead,
            uint* pNumberOfBytesRead,
            IntPtr Overlapped);



        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool SetFilePointerEx(
            SafeFileHandle hFile,
            ulong liDistanceToMove,
            out ulong lpNewFilePointer,
            uint dwMoveMethod);


        [DllImport("kernel32.dll")]
        protected static extern bool FlushFileBuffers(
            SafeFileHandle hFile);

        //ENUMS DEFINED IN WINIOCTL.H
        protected enum STORAGE_PROPERTY_ID
        {
            StorageDeviceProperty = 0,
            StorageAdapterProperty,
            StorageDeviceIdProperty,
            StorageDeviceUniqueIdProperty,
            StorageDeviceWriteCacheProperty,
            StorageMiniportProperty,
            StorageAccessAlignmentProperty,
            StorageDeviceSeekPenaltyProperty,
            StorageDeviceTrimProperty,
            StorageDeviceWriteAggregationProperty,
            StorageDeviceDeviceTelemetryProperty,
            StorageDeviceLBProvisioningProperty,
            StorageDevicePowerProperty,
            StorageDeviceCopyOffloadProperty,
            StorageDeviceResiliencyProperty,
            StorageDeviceMediumProductType,
            StorageAdapterRpmbProperty,
            StorageAdapterCryptoProperty,
            StorageDeviceIoCapabilityProperty = 48,
            StorageAdapterProtocolSpecificProperty,
            StorageDeviceProtocolSpecificProperty,
            StorageAdapterTemperatureProperty,
            StorageDeviceTemperatureProperty,
            StorageAdapterPhysicalTopologyProperty,
            StorageDevicePhysicalTopologyProperty,
            StorageDeviceAttributesProperty,
            StorageDeviceManagementStatus,
            StorageAdapterSerialNumberProperty,
            StorageDeviceLocationProperty,
            StorageDeviceNumaProperty,
            StorageDeviceZonedDeviceProperty,
            StorageDeviceUnsafeShutdownCount,
            StorageDeviceEnduranceProperty,
        };

        protected enum STORAGE_QUERY_TYPE
        {
            PropertyStandardQuery = 0,          // Retrieves the descriptor
            PropertyExistsQuery,                // Used to test whether the descriptor is supported
            PropertyMaskQuery,                  // Used to retrieve a mask of writeable fields in the descriptor
            PropertyQueryMaxDefined             // use to validate the value
        };


        //STRUCTS DEFINED IN WINIOCTL.H
        protected struct DEVICE_TRIM_DESCRIPTOR
        {
            public bool TrimEnabled;
            public ulong Version;
            public ulong Size;
        };

        protected struct STORAGE_PROPERTY_QUERY
        {
            public STORAGE_PROPERTY_ID PropertyId;
            public STORAGE_QUERY_TYPE QueryType;
            public byte AdditionalParameters;
        };


        public const int DEFAULT_SECTOR_SIZE = 512;
        private const int BUFFER_SIZE = 4096;

        private string diskID;
        private FileAccess desiredAccess;
        private SafeFileHandle fileHandle;


        /// <summary>
        /// OpenFileObject opens a safe file handle for the drive with desired access.
        /// (Acts like a constructor)
        /// </summary>
        /// <param name="diskIndex"></param>
        /// <param name="desiredAccess"></param>
        public void OpenFileObject(uint diskIndex, FileAccess desiredAccess)
        {
            this.diskID = "\\\\.\\PhysicalDisk" + diskIndex;
            this.desiredAccess = desiredAccess;

            // if desiredAccess is Write or Read/Write
            //   find volumes on this disk
            //   lock the volumes using FSCTL_LOCK_VOLUME
            //     unlock the volumes on Close() or in destructor


            this.fileHandle = this.OpenFile(this.diskID, desiredAccess);
        }

        /// <summary>
        /// Opens file handle for the required drive.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="desiredAccess"></param>
        /// <returns>
        /// Pointer to the drive
        /// </returns>
        private SafeFileHandle OpenFile(string id, FileAccess desiredAccess)
        {
            uint access;
            switch (desiredAccess)
            {
                case FileAccess.Read:
                    access = DeviceIO.GENERIC_READ;
                    break;
                case FileAccess.Write:
                    access = DeviceIO.GENERIC_WRITE;
                    break;
                case FileAccess.ReadWrite:
                    access = DeviceIO.GENERIC_READ | DeviceIO.GENERIC_WRITE;
                    break;
                default:
                    access = DeviceIO.GENERIC_READ;
                    break;
            }

            SafeFileHandle ptr = DeviceIO.CreateFile(
                id,
                access,
                DeviceIO.FILE_SHARE_READ,
                IntPtr.Zero,
                DeviceIO.OPEN_EXISTING,
                DeviceIO.FILE_FLAG_NO_BUFFERING | DeviceIO.FILE_FLAG_WRITE_THROUGH,
                IntPtr.Zero);

            if (ptr.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            return ptr;
        }

        // Access Control Functions (Not in Scope)
        public override bool CanRead
        {
            get
            {
                return (this.desiredAccess == FileAccess.Read || this.desiredAccess == FileAccess.ReadWrite) ? true : false;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return (this.desiredAccess == FileAccess.Write || this.desiredAccess == FileAccess.ReadWrite) ? true : false;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }
        public override long Length
        {
            get { return (long)this.Length; }
        }


        public override long Position
        {
            get
            {
                return (long)PositionU;
            }
            set
            {
                PositionU = (ulong)value;
            }
        }

        // Set or Retrieve Postition of File Pointers
        public ulong PositionU
        {
            get
            {
                ulong n = 0;
                if (!DeviceIO.SetFilePointerEx(this.fileHandle, 0, out n, (uint)SeekOrigin.Current))
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                return n;
            }
            set
            {
                ulong n = 0;
                if (!DeviceIO.SetFilePointerEx(this.fileHandle, value, out n, (uint)SeekOrigin.Begin))
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        // not required, since FILE_FLAG_WRITE_THROUGH and FILE_FLAG_NO_BUFFERING are used
        public override void Flush()
        {
            //if (!Unmanaged.FlushFileBuffers(this.fileHandle))
            //    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        /// <summary>
        /// Close Opened File Handles for current object
        /// </summary>
        public override void Close()
        {
            if (this.fileHandle != null)
            {
                DeviceIO.CloseHandle(this.fileHandle);
                this.fileHandle.SetHandleAsInvalid();
                this.fileHandle = null;
            }
            base.Close();
        }

        // Not in Scope
        public override void SetLength(long value)
        {
            throw new NotSupportedException("Setting the length is not supported with DiskStream objects.");
        }

        /// <summary>
        /// Overriding Default Stream Read Function
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns>
        /// Number of Bytes returned from ReadFile function.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return (int)Read(buffer, (uint)offset, (uint)count);
        }

        /// <summary>
        /// Function to Read Data into the Buffer from disk
        /// </summary>
        /// <param name="readBuffer">bytes read from the current disk.</param>
        /// <param name="readOffset"> byte offset in readBuffer at which to begin storing the data read from the current stream.</param>
        /// <param name="readCount">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// Number of bytes read into the buffer.
        /// </returns>
        public unsafe uint Read(byte[] readBuffer, uint readOffset, uint readCount)
        {
            uint BytesRead = 0;
            UInt64 newFilePtr = 0;
            uint size = 512;// ((sectorDiff > 32) ? ((sectorDiff > 1024) ? (512 * 2 * 64 ) : (512 * 2 * 4)) : 512); 
            ulong distanceToMove = (ulong)(Wipe.sectorStart * size);

            Console.WriteLine("\nVerifying Write Pattern....\n");

            fixed (byte* readBufferPointer = readBuffer)
            {

                DeviceIO.SetFilePointerEx(this.fileHandle, distanceToMove, out newFilePtr, DeviceIO.FILE_BEGIN);

                for (ulong i = Wipe.sectorStart; i <= Wipe.sectorEnd; i += ((ulong)size / 512))
                {
                    if (!DeviceIO.ReadFile(this.fileHandle, readBufferPointer + readOffset, size, &BytesRead, IntPtr.Zero))
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                    if (!readBuffer.SequenceEqual(toWrite))
                    {
                        Wipe.sectorErrorCount++;
                    }
                    /*else
                    {
                        Console.WriteLine("Could not verify");
                    }*/
                    //Wipe.sectorStart += ((ulong)size / 512);
                }

                Console.WriteLine("Verification Complete.....");
                return BytesRead;
            }
        }

        /// <summary>
        ///  Overriding Default Stream Write Function
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Write(buffer, (uint)offset, (uint)count);
        }

        /// <summary>
        ///  Function to Write Data Pattern that is in buffer to disk sectors
        /// </summary>
        /// <param name="writeBuffer">The buffer that has the pattern of data to write</param>
        /// <param name="writeOffset">The byte offset in writeBuffer from which to begin copying bytes to the stream</param>
        /// <param name="writeCount">The maximum number of bytes to write</param>
        /// <returns></returns>
        public unsafe void Write(byte[] writeBuffer, uint writeOffset, uint writeCount)
        {
            uint BytesWritten = 0;
            UInt64 newFilePtr = 0;
            long sectorStart = -1;
            long sectorEnd = -1;

            while (true)
            {
                Console.WriteLine("Enter start sector to overwrite from : ");
                Wipe.sectorStart = Convert.ToUInt64(Console.ReadLine());
                Console.WriteLine("\nEnter end sector to write till : ");
                Wipe.sectorEnd = Convert.ToUInt64(Console.ReadLine());
                if (sectorStart < sectorEnd)
                    break;
                else
                    Console.WriteLine("\nStart sector cannot be greater than end Sector.....Try again!!!\n");
            }
            long sectorDiff = sectorEnd - sectorStart;
            int size = Wipe.sectorWriteSize;// ((sectorDiff > 32) ? ((sectorDiff > 1024) ? (512 * 2 * 64) : (512 * 2 * 4)) : 512); //512 * 2 * 1024;
            ulong distanceToMove = (ulong)(sectorStart * size);
            Console.WriteLine("\nWriting entered pattern to disk\n");

            try
            {
                fixed (byte* writeBufferPointer = writeBuffer)
                {

                    DeviceIO.SetFilePointerEx(this.fileHandle, distanceToMove, out newFilePtr, DeviceIO.FILE_BEGIN);

                    for (long i = sectorStart; i <= sectorEnd; i += (size / 512))
                    {
                        if (!DeviceIO.WriteFile(this.fileHandle, writeBufferPointer + writeOffset, (uint)size, &BytesWritten, IntPtr.Zero))
                            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                        //sectorStart += (size / 512);
                        if (i % 100 == 0)
                            drawTextProgressBar((int)i, (int)sectorEnd);
                    }
                    Console.WriteLine("After WriteFile");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Stream Class Default Seek Function
        public override long Seek(long offset, SeekOrigin origin)
        {
            return (long)SeekU((ulong)offset, origin);
        }

        // 
        public ulong SeekU(ulong offset, SeekOrigin origin)
        {
            ulong n = 0;
            if (!DeviceIO.SetFilePointerEx(this.fileHandle, offset, out n, (uint)origin))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            return n;
        }

        /// <summary>
        /// Function to Unmount Volumes
        /// </summary>
        /// <param name="drive">Volume label</param>
        /// <returns>
        /// true on successfull unmount of the volume, else false
        /// </returns>
        public bool UnmountVolume(string drive)
        {
            try
            {
                uint BytesWritten = 0;
                Console.WriteLine("\nAttempting to Unmount volume : " + drive + "\n");
                bool unmounted = DeviceIO.DeviceIoControl(this.fileHandle, DeviceIO.FSCTL_DISMOUNT_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, ref BytesWritten, IntPtr.Zero);
                if (unmounted == false) Console.WriteLine("\nCoudln't Dismount  volume\n");
                return unmounted;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }

        /// <summary>
        /// Function to Lock Volumes
        /// </summary>
        /// <param name="drive">Volume label</param>
        /// <returns>
        /// true on successful lock of the volumes,else false
        /// </returns>
        public bool LockVolume(string drive)
        {
            try
            {
                uint bytesWritten = 0;
                bool locked = DeviceIO.DeviceIoControl(this.fileHandle, DeviceIO.FSCTL_LOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, ref bytesWritten, IntPtr.Zero);
                if (locked == false) Console.WriteLine("\nCoudln't lock  volume\n");
                return locked;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Progress bar function to show progress of the overwritting process.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="total"></param>
        public static void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }
}
