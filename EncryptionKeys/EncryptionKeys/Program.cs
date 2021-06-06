using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionKeys
{
    class Program
    {
        private static readonly RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        static void Main(string[] args)
        {
            // Initialization Vector
            byte[] initializationVector = new byte[16];
            provider.GetBytes(initializationVector);
            Console.WriteLine("Initialization Vector: ");
            foreach(byte b in initializationVector)
            {
                Console.WriteLine(b);
            }

            var hmac = new HMACSHA256();
            var key = Convert.ToBase64String(hmac.Key);
            string getBytes = key.Substring(0, 32);
            Console.WriteLine("\nSECRET KEY: {0}", getBytes);
            Console.ReadKey();
        }
    }
}
