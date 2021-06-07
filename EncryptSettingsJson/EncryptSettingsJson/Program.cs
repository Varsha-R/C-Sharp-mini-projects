using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptSettingsJson
{
    class Program
    {
        private static readonly AesFileCrypto fileCrypto = new AesFileCrypto();
        private static readonly RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        static void Main(string[] args)
        {
            string getBytes = "eOvVt7zWlNgmPcGoUmanwcDWp3nrjWOQ";
            byte[] initializationVector = new byte[] { 162, 244, 37, 135, 24, 63, 34, 247, 57, 183, 218, 183, 210, 234, 240, 91};
            string authCode;
            byte[] keyBytes = Encoding.UTF8.GetBytes(getBytes);
            byte[] authenticationCode = fileCrypto.Encrypt(keyBytes, initializationVector, "C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\Empty\\SettingsExport.json", out string encryptionFile);
            authCode = Convert.ToBase64String(authenticationCode);
            Console.WriteLine("Authentication Code: {0}", authCode);

            //string secondAuthCode;
            //byte[] secondKeyByte;
            //string fingerPrint = "511C3F5E6DE81A2C052C36C44F635043";
            ////string fingerPrint = finger.Substring(0, 32);
            //secondKeyByte = Encoding.UTF8.GetBytes(fingerPrint);
            //byte[] secondAuthenticationCode = fileCrypto.Encrypt(secondKeyByte, initializationVector, "C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\SettingsInfo.json", out string encryptionnFile);
            //secondAuthCode = Convert.ToBase64String(secondAuthenticationCode);
            //Console.WriteLine("Authentication Code: {0}", secondAuthCode);

            //string archivedFile = "";
            //string secondEncryptFile = "C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\SettingsInfo.json";
            //byte[] secondReceivedData = File.ReadAllBytes(secondEncryptFile);
            //byte[] secondAuthenticationMAC = fileCrypto.CalculateHMAC(secondKeyByte, secondReceivedData);
            //Console.WriteLine("Authentication MAC: {0}", Convert.ToBase64String(secondAuthenticationMAC));
            //if (secondAuthCode == Convert.ToBase64String(secondAuthenticationMAC))
            //{
            //    archivedFile = fileCrypto.Decrypt(secondKeyByte, initializationVector, secondEncryptFile);
            //}

            //string archivedFile = "";
            //byte[] keyByte = Encoding.UTF8.GetBytes(getBytes);
            //string encryptFile = "C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\SettingsInfo.json";
            //byte[] receivedData = File.ReadAllBytes(encryptFile);
            //byte[] authenticationMAC = fileCrypto.CalculateHMAC(keyByte, receivedData);
            //Console.WriteLine("Authentication MAC: {0}", Convert.ToBase64String(authenticationMAC));
            //if (authCode == Convert.ToBase64String(authenticationMAC))
            //{
            //    archivedFile = fileCrypto.Decrypt(keyByte, initializationVector, encryptFile);
            //    Console.WriteLine(archivedFile);
            //}
                        
            Console.ReadKey();
        }
    }
}
