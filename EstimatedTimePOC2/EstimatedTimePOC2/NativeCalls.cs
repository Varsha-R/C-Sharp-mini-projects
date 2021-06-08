using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace EstimatedTimePOC2
{
    public static class NativeCalls
    {
        public const uint GENERIC_ALL = 0x10000000;
        public const uint CREATE_NEW = 1;
        public const uint OPEN_EXISTING = 3;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        public const uint FILE_FLAG_NO_BUFFERING = 0x20000000;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            string FileName,
            uint DesiredAccess,
            uint ShareMode,
            IntPtr SecurityAttributes,
            uint CreationDisposition,
            uint FlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(SafeFileHandle hHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe bool WriteFile(
            SafeFileHandle hFile,
            byte* pBuffer,
            uint NumberOfBytesToWrite,
            uint* pNumberOfBytesWritten,
            IntPtr Overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe bool ReadFile(
            SafeFileHandle hFile,
            byte* pBuffer,
            uint NumberOfBytesToRead,
            uint* pNumberOfBytesRead,
            IntPtr Overlapped);
    }
}
