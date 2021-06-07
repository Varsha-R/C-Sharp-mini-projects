using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptSettingsJson
{
    public class SecureRandomGenerator
    {
        public const string ALLCHARS = ALPHANUMERIC + SpecialChars;
        public const string ALPHANUMERIC = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        public const string NUMERIC = "1234567890";
        public const string SpecialChars = "!@#$%^&*()+-_'?:,.{}[]~";

        public static string GenerateNumberString(int maxDigits)
        {
            return Generate(maxDigits, NUMERIC);
        }

        public static int GenerateNumber(int maxDigits)
        {
            return Convert.ToInt32(GenerateNumberString(maxDigits));
        }


        public static string Generate(int length, string charsetForRandom = ALLCHARS)
        {
            char[] result = new char[length];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                fillRandomChars(result, length, charsetForRandom, crypto);
            }
            return new string(result);

        }

        public static void fillRandomChars(char[] result, int length, string charsetForRandom, RNGCryptoServiceProvider crypto)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (charsetForRandom == null)
                throw new ArgumentNullException("charsetForRandom");
            char[] characterArray = charsetForRandom.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("charsetForRandom must not be empty", "characterSet");

            byte[] bytes = new byte[length * 8];
            crypto.GetBytes(bytes);
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
        }
    }
}
