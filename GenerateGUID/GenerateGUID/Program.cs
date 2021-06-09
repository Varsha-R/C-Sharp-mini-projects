using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenerateGUID
{
    class Program
    {
        static void Main(string[] args)
        {
            //Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-";
            uint maxValue = (uint)((((ulong)uint.MaxValue) + 1) / (uint)characters.Length * (uint)characters.Length);
            int length = 12;
            StringBuilder result = new StringBuilder(12);
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < length; i++)
                {
                    byte[] buffer = new byte[sizeof(uint)];
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    result.Append(characters[(int)(num % (uint)characters.Length)]);
                }
            }
            //for (int i = 0; i < 12; i++)
            //{
            //    result.Append(characters[random.Next(characters.Length)]);
            //}
            Console.WriteLine(result.ToString());
            Console.ReadKey();
        }
    }
}
