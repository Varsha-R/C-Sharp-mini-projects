using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;


namespace SecureEntitlementPOC
{
    class Program
    {
        static readonly string entitlementTestString = "rutf-4954fjskcn-#$@#Ofdsk){DSsdDET^U";
        static readonly byte[] initializationVector = new byte[] { 162, 244, 37, 135, 24, 63, 34, 247, 57, 183, 218, 183, 210, 234, 240, 91 };

        static void Main(string[] args)
        {
            Console.WriteLine("Original string: {0}", entitlementTestString);
            long stringHash = GetHash(entitlementTestString);
            Console.WriteLine("Hash: {0}", stringHash);

            string uuid = GetUUID();
            uuid = uuid.Replace("-", "");
            Console.WriteLine("UUID: {0}", uuid);

            var encryptedString = Crypt.EncryptString(uuid, entitlementTestString, initializationVector);
            Console.WriteLine("Encrypted string: {0}", encryptedString);

            uuid = uuid.Replace("4", "5");
            Console.WriteLine("UUID AFTER tampering: {0}", uuid);

            var decryptedString = Crypt.DecryptString(uuid, encryptedString, initializationVector);
            Console.WriteLine("Decrypted string: {0}", decryptedString);

            #region Storing details in DB

            SQLiteConnection.CreateFile("TestDatabase.sqlite");

            SQLiteConnection openDBConnection = new SQLiteConnection("Data Source=TestDatabase.sqlite");
            openDBConnection.Open();

            string sql = "create table Entitlement (entitlementString varchar(100), entitlementHash long)";
            SQLiteCommand command = new SQLiteCommand(sql, openDBConnection);
            command.ExecuteNonQuery();

            string insert = "insert into Entitlement (entitlementString, entitlementHash) values (@encryptedString, @stringHash)";
            SQLiteCommand insertCommand = openDBConnection.CreateCommand();
            insertCommand.CommandText = insert;
            insertCommand.Parameters.AddWithValue("@encryptedString", encryptedString);
            insertCommand.Parameters.AddWithValue("@stringHash", stringHash);
            insertCommand.ExecuteNonQuery();
            insertCommand.Dispose();
            openDBConnection.Close();

            #endregion

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
            
            if(uuid.Equals("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"))
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
                RegistryKey cryptography = root.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", true);
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
