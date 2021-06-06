using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CreateDirectoryAdminOnlyAccess
{
    class Program
    {
        private const string adminIdentity = "BUILTIN\\Administrators";

        static void Main(string[] args)
        {
            string DirectoryName = @"D:\ZZZZZ\";
            Console.WriteLine(RestrictDirectoryACL(DirectoryName));
            Console.ReadKey();
        }

        public static bool RestrictDirectoryACL(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                DirectoryInfo dInfo = new DirectoryInfo(dirPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.SetAccessRuleProtection(true, false);
                dSecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, FileSystemRights.ReadData, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                dSecurity.AddAccessRule(new FileSystemAccessRule(adminIdentity, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                Directory.SetAccessControl(dirPath, dSecurity);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
