using System;
using System.Management;

namespace BitlockEncrypted
{
    class Program
    {
        private static void TestBitLockerMenuItem_Click()
        {
            var path = new ManagementPath(@"\ROOT\CIMV2\Security\MicrosoftVolumeEncryption")
            { ClassName = "Win32_EncryptableVolume" };
            var scope = new ManagementScope(path);
            path.Server = Environment.MachineName;
            var objectSearcher = new ManagementClass(scope, path, new ObjectGetOptions());
            foreach (var item in objectSearcher.GetInstances())
            {
                Console.WriteLine(item["DriveLetter"].ToString() + " | | " + item["ProtectionStatus"].ToString());
            }
        }

        static void Main(string[] args)
        {
            TestBitLockerMenuItem_Click();
            Console.ReadKey();
        }
    }
}
