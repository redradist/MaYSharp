using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MaY.Networking
{
    public static class RawPacketExtensions
    {
        public static byte[] SerializeToBuffer(this in RawPacket rawPacket)
        {
            var stream = new MemoryStream();
            // Serialize an object into the storage medium referenced by 'stream' object.
            BinaryFormatter formatter = new BinaryFormatter();
            // Serialize multiple objects into the stream
            formatter.Serialize(stream, rawPacket);
            return stream.GetBuffer();
        }

        public static RawPacket DeserializeRawPacket(this List<byte> data)
        {
            Stream stream = new MemoryStream(data.ToArray());
            // Serialize an object into the storage medium referenced by 'stream' object.
            BinaryFormatter formatter = new BinaryFormatter();
            // Serialize multiple objects into the stream
            return (RawPacket) formatter.Deserialize(stream);
        }
    }
}