using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptSettingsJson
{
    public class CryptoSecureRandom : IDisposable
    {
        private readonly RNGCryptoServiceProvider provider;

        public CryptoSecureRandom()
        {
            provider = new RNGCryptoServiceProvider();
        }

        public byte[] NextByte(int numberOfBytes)
        {
            return GetCryptographicallySecureRandomBytes(numberOfBytes);
        }

        public uint NextUnsignedInt(int numberOfDigits)
        {
            if (numberOfDigits < 0)
            {
                throw new ArgumentException("numberOfDigits can not be negative", nameof(numberOfDigits));
            }

            if (numberOfDigits == 0)
            {
                throw new ArgumentException("numberOfDigits can not be zero", "numberOfDigits");
            }
            char[] result = new char[numberOfDigits];
            SecureRandomGenerator.fillRandomChars(result, numberOfDigits, SecureRandomGenerator.NUMERIC, provider);
            uint randomNum = UInt32.Parse(new string(result));
            return randomNum;
        }

        private byte[] GetCryptographicallySecureRandomBytes(int numberOfBytes)
        {
            byte[] randomBytes = new byte[numberOfBytes];
            provider.GetBytes(randomBytes);
            return randomBytes;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    provider?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
