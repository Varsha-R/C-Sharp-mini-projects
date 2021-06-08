using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EntitlementPOC
{
    class Program
    {
        private static readonly string entitlementString = "ab45jdg-24/5/2022";
        static readonly byte[] initializationVector = new byte[] { 162, 244, 37, 135, 24, 63, 34, 247, 57, 183, 218, 183, 210, 234, 240, 91 };

        static void Main(string[] args)
        {
            // Will they be sending it in 24 hour format?
            string end = "2021-09-09 14:21:00";
            string now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string[] formats = { "yyyy-MM-ddTHH:mm:ss" };
            DateTime today = DateTime.ParseExact(now, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime universalnow = today.ToUniversalTime();
            DateTime universalend = endDate.ToUniversalTime();
            if (today < endDate)
            {
            }

            //string endDate = str.Substring(str.Length - 10);
            //string serviceLevelCode = str.Substring(0, 2);
            //string today = DateTime.Today.ToString(new CultureInfo("en-US"));
            string decryptedString = "";
            DateTime thisDay = DateTime.Today;
            // Display the date in the default (general) format.
            Console.WriteLine(thisDay);
            Console.WriteLine(thisDay.ToString("MM/dd/yyyy"));
            //string[] formats = { "MM/dd/yyyy hh:mm" };
            string date = "08/23/2020 00:00";
            DateTime thatDay = DateTime.ParseExact(date, formats, new CultureInfo("en-US"), DateTimeStyles.None);
            Console.WriteLine(thatDay);
            Console.WriteLine(thatDay.ToString("MM/dd/yyyy"));
            if(thisDay < thatDay)
            {
                Console.WriteLine("Valid");
            }
            //Get UUID
            Console.WriteLine("Original string: {0}", entitlementString);
            string uuid = GetUUID();
            Console.WriteLine("UUID: {0}", uuid);

            //UUID SHA256 hash as key for encryption decryption
            byte[] uuidHash = GetHash(uuid);

            //h = HMAC(uuid hash, entitlement)
            byte[] hmacInitial = new HMACSHA256(uuidHash).ComputeHash(Encoding.UTF8.GetBytes(entitlementString));
            string hmacInitialString = Encoding.UTF8.GetString(hmacInitial);

            //e = AES256 encryption
            string aesEncryptedString = EncryptString(uuidHash, entitlementString, initializationVector);
            Console.WriteLine("Encrypted string: {0}", aesEncryptedString);

            //Store h and e in DB

            //d = AES256 decryption with uuid hash and e => entitlement
            //string decryptedString = DecryptString(uuidHash, aesEncryptedString, initializationVector);
            Console.WriteLine("Decrypted string: {0}", decryptedString);

            //h2 = HMAC(uuid hash,  d)
            byte[] hmacFinal = new HMACSHA256(uuidHash).ComputeHash(Encoding.UTF8.GetBytes(decryptedString));
            string hmacFinalString = Encoding.UTF8.GetString(hmacFinal);

            //h1 matches h2
            if(hmacInitialString.SequenceEqual(hmacFinalString))
            {
                Console.WriteLine("The HMACs match");
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

        static byte[] GetHash(string stringToHash)
        {
            StringBuilder Sb = new StringBuilder();
            byte[] result;
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                result = hash.ComputeHash(enc.GetBytes(stringToHash));
                //foreach (byte b in result)
                //    Sb.Append(b.ToString("x2"));
            }
            return result;
        }

        static string EncryptString(byte[] key, string plainText, byte[] iv)
        {
            byte[] encryptedArray;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        encryptedArray = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encryptedArray);
        }

        static string DecryptString(byte[] key, string encryptedText, byte[] iv)
        {
            byte[] buffer = Convert.FromBase64String(encryptedText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
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
    }
}
