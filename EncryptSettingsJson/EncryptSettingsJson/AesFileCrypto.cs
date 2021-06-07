using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptSettingsJson
{
    public class AesFileCrypto
    {
        private static readonly Type classType = typeof(AesFileCrypto);

        public byte[] CalculateHMAC(byte[] key, byte[] data)
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

        public byte[] Encrypt(byte[] key, byte[] IV, string fileName, out string encryptedFile)
        {
            try
            {
                if (key == null || key.Length <= 0)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (IV == null || IV.Length <= 0)
                {
                    throw new ArgumentNullException(nameof(IV));
                }

                byte[] data = File.ReadAllBytes(fileName);

                byte[] encrytedData = EncryptFileUsingAes(data, key, IV);
                byte[] authCode = CalculateHMAC(key, encrytedData);
                encryptedFile = @"C:\Users\Varsha.Ravindra\Documents\Registry\Empty\FinalEncrypted\SettingsExport.json";
                File.WriteAllBytes(encryptedFile, encrytedData);
                return authCode;
            }
            catch (FileNotFoundException fne)
            {
                encryptedFile = "";
                throw;

            }
            catch (ArgumentException ae)
            {
                encryptedFile = "";
                throw;

            }
            catch (PathTooLongException pe)
            {
                encryptedFile = "";
                throw;
            }
            catch (DirectoryNotFoundException de)
            {
                encryptedFile = "";
                throw;
            }
            catch (IOException oe)
            {
                encryptedFile = "";
                throw;
            }
            catch (UnauthorizedAccessException ue)
            {
                encryptedFile = "";
                throw;
            }
            catch (System.Security.SecurityException se)
            {
                encryptedFile = "";
                throw;
            }
            catch (NotSupportedException nse)
            {
                encryptedFile = "";
                throw;
            }
        }
        private byte[] EncryptFileUsingAes(byte[] plainData, byte[] key, byte[] iV)
        {

            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), iV));
            byte[] encrypted = cipher.DoFinal(plainData);
            return encrypted;
        }

        public string Decrypt(byte[] key, byte[] IV, string fileName)
        {
            try
            {
                byte[] cipherText = File.ReadAllBytes(fileName);
                string decryptedFile = DecryptUsingAes(cipherText, key, IV);
                return decryptedFile;
            }

            catch (FileNotFoundException fne)
            {
                throw;
            }
            catch (ArgumentException ae)
            {
                throw;
            }
            catch (PathTooLongException pe)
            {
                throw;
            }
            catch (DirectoryNotFoundException de)
            {
                throw;
            }
            catch (IOException oe)
            {
                throw;
            }
            catch (UnauthorizedAccessException ue)
            {
                throw;
            }
            catch (System.Security.SecurityException se)
            {
                throw;
            }
            catch (NotSupportedException nse)
            {
                throw;
            }
        }

        private string DecryptUsingAes(byte[] cipherText, byte[] key, byte[] IV)
        {

            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), IV));

            byte[] plainText = null;
            try
            {
                plainText = cipher.DoFinal(cipherText);
                string decryptedFile = "C:\\Users\\Varsha.Ravindra\\Documents\\Registry\\SettingsInfo.json";
                File.WriteAllBytes(decryptedFile, plainText);
                return Encoding.ASCII.GetString(plainText, 0, plainText.Length);
            }
            catch (InvalidCipherTextException e)
            {
                throw new CryptoException("Decryption failed", e);
            }
            catch (FileNotFoundException fne)
            {
                throw;

            }
            catch (ArgumentException ae)
            {
                throw;

            }
            catch (PathTooLongException pe)
            {
                throw;
            }
            catch (DirectoryNotFoundException de)
            {
                throw;
            }
            catch (IOException oe)
            {
                throw;
            }
            catch (UnauthorizedAccessException ue)
            {
                throw;
            }
            catch (System.Security.SecurityException se)
            {
                throw;
            }
            catch (NotSupportedException nse)
            {
                throw;
            }
        }
    }
}
