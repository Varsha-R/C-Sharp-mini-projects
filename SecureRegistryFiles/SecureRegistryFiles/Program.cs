using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureRegistryFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\windows\\system32\\cmd.exe",
                    Arguments = @"/C REG QUERY ""HKCU\Software\Microsoft\Windows NT\CurrentVersion\Accessibility"" /s",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    WindowStyle = ProcessWindowStyle.Normal,
                    RedirectStandardInput = true
                }
            };
            p.Start();
            string val = p.StandardOutput.ReadToEnd();
            string[] split = val.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            string required = split[0];
            string exportRegistryHMAC = @"C:\Users\Varsha.Ravindra\Documents\DB Test\ExportReg.txt";
            using (FileStream fsWDT = new FileStream(exportRegistryHMAC, FileMode.Append, FileAccess.Write))
            using (StreamWriter swDT = new StreamWriter(fsWDT))
            {
                swDT.WriteLine(required);
            }
            Console.WriteLine(required);

            string exportReg = @"C:\Users\Varsha.Ravindra\Documents\DB Test\ExportReg.reg";
            byte[] symmetricKey = Encoding.UTF8.GetBytes("eOvVt7zWlNgmPcGoUmanwcDWp3nrjWOQ");
            //byte[] byteArrayForHmac = Encoding.UTF8.GetBytes(required);
            byte[] exportRegArray = File.ReadAllBytes(exportReg);
            byte[] HMAC = CalculateHMAC(symmetricKey, exportRegArray);
            //string getStringFromHMAC = BitConverter.ToString(HMAC);

            Console.ReadKey();
        }

        public static byte[] CalculateHMAC(byte[] key, byte[] data)
        {
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentNullException(nameof(data));
            }
            byte[] hash = null;
            try
            {
                using (HMACSHA256 authCode = new HMACSHA256(key))
                {
                    hash = authCode.ComputeHash(data);
                }
                return hash;
            }
            catch (ObjectDisposedException e)
            {
                throw new ObjectDisposedException("Failed to calculate HMAC", e.StackTrace);
            }
        }
    }
}
