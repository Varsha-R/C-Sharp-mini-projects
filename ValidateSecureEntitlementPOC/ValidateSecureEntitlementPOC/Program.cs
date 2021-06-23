using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ValidateSecureEntitlementPOC
{
    class Program
    {
        static readonly byte[] initializationVector = new byte[] { 162, 244, 37, 135, 24, 63, 34, 247, 57, 183, 218, 183, 210, 234, 240, 91 };

        static void Main(string[] args)
        {
            string entitlementString = null;
            long entitlementHash = 0;

            string dbPath = @"C:\Users\Varsha.Ravindra\source\repos\SecureEntitlementPOC\SecureEntitlementPOC\bin\Debug\TestDatabase.sqlite";
            SQLiteConnection openDBConnection = new SQLiteConnection("Data Source=" + dbPath);
            openDBConnection.Open();

            string getEntitlementDetails = "select * from Entitlement";
            SQLiteCommand getEntitlementDetailsCommand = openDBConnection.CreateCommand();
            getEntitlementDetailsCommand.CommandText = getEntitlementDetails;
            SQLiteDataReader sqReader = getEntitlementDetailsCommand.ExecuteReader();
            while(sqReader.Read())
            {
                entitlementString = sqReader.GetValue(sqReader.GetOrdinal("entitlementString")).ToString();
                entitlementHash = long.Parse(sqReader.GetValue(sqReader.GetOrdinal("entitlementHash")).ToString());
            }
            Console.WriteLine(entitlementString);
            Console.WriteLine(entitlementHash.ToString());
            getEntitlementDetailsCommand.Dispose();
            openDBConnection.Close();

            string uuid = GetUUID();
            uuid = uuid.Replace("-", "");
            string decryptedString = DecryptString(uuid, entitlementString, initializationVector);
            Console.WriteLine("Decrypted string: {0}", decryptedString);

            long hash = GetHash(decryptedString);
            Console.WriteLine("Decrypted string hash: {0}", hash);

            if(hash == entitlementHash)
            {
                Console.WriteLine("\nUser is entitled to use Erase");
            }
            else
            {
                Console.WriteLine("\nUser is not entitled");
            }

            Console.ReadKey();
        }

        static string GetUUID()
        {
            string uuid = string.Empty;

            ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                uuid = mo.Properties["UUID"].Value.ToString();
                break;
            }

            if (uuid.Equals("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"))
            {
                RegistryKey root;
                if (Environment.Is64BitOperatingSystem)
                {
                    root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                }
                else
                {
                    root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                }
                RegistryKey cryptography = root.OpenSubKey(@"\SOFTWARE\Microsoft\Cryptography", true);
                if (cryptography.GetValueNames().Contains("MachineGuid"))
                {
                    uuid = cryptography.GetValue("MachineGuid").ToString();
                }
                else
                {
                    Console.WriteLine("Could not fetch machine guid from registry");
                }
            }
            return uuid;
        }

        public static string DecryptString(string key, string cipherText, byte[] iv)
        {
            //byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        static long GetHash(string text)
        {
            var hash = 23;
            foreach (char c in text)
            {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }
}
