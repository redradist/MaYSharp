using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace MaY
{
    [Serializable]
    public sealed class Block<THasher> : IBlock
        where THasher : IHasher, new()
    {
        public ulong Version { get; } = 1;
        public byte[] Buffer { get; }
        public byte[] Hash { get; }

        internal Block(byte[] buffer)
        {
            this.Buffer = (byte[]) buffer.Clone();
            using (THasher hasher = new THasher())
            {
                this.Hash = hasher.ComputeHash(this.Buffer);
            }
        }

        public static Block<THasher> CreateBlock(byte[] buffer,
                                                 Block<THasher>? prevBlock = null)
        {
            byte[] dataBuffer;
            if (prevBlock != null)
            {
                var list = new List<byte>();
                list.AddRange(buffer);
                list.AddRange(prevBlock.Buffer);
                dataBuffer = list.ToArray();
            }
            else
            {
                dataBuffer = buffer;
            }
            return new Block<THasher>(dataBuffer);
        }

        public static Block<THasher> CreateBlock<T>(T data,
                                                    Block<THasher>? prevBlock = null)
        {
            return Block<THasher>.CreateBlock(Serialize(data), prevBlock);
        }

        private static byte[] Serialize<T>(T data)
        {
            var stream = new MemoryStream();
            // Serialize an object into the storage medium referenced by 'stream' object.
            BinaryFormatter formatter = new BinaryFormatter();
            // Serialize multiple objects into the stream
            formatter.Serialize(stream, data);
            return stream.ToArray();
        }

        private static T Deserialize<T>(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            // Serialize an object into the storage medium referenced by 'stream' object.
            BinaryFormatter formatter = new BinaryFormatter();
            // Serialize multiple objects into the stream
            return (T) formatter.Deserialize(stream);
        }
    }
}
