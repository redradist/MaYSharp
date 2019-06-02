using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MaY
{
    public interface IHasher : IDisposable
    {
        byte[] ComputeHash(Stream inputStream);
        byte[] ComputeHash(byte[] buffer, int offset, int count);
        byte[] ComputeHash(byte[] buffer);
    }
}
