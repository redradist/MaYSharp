using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MaY
{
    public class SHA512Hasher : IHasher
    {
        private SHA512 hasher;

        public SHA512Hasher()
        {
            hasher = SHA512.Create();
        }

        public byte[] ComputeHash(Stream inputStream)
        {
            return hasher.ComputeHash(inputStream);
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            return hasher.ComputeHash(buffer, offset, count);
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            return hasher.ComputeHash(buffer);
        }

        public void Dispose()
        {
            hasher.Dispose();
        }
    }
}
