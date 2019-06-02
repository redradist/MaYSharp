using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MaY.Networking
{
    [Serializable]
    public struct RawPacket
    {
        public byte[] Payload;
        public byte[] MAC;
    }
}
