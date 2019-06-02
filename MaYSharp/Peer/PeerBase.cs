using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading.Tasks;

using ICCSharp;
using ICCSharp.Networking;
using MaY.Networking;
using TcpClient = ICCSharp.Networking.TcpClient;

namespace MaY
{
    public abstract class PeerBase : Component, IPeer
    {
        private const uint BUFFER_SIZE = 4096;

        protected readonly TcpServer? Server;
        protected readonly HashAlgorithm Hasher;
        protected readonly List<IPeerProxy> ConnectedPeersProxy = new List<IPeerProxy>();
        protected readonly Dictionary<IPeerProxy, List<byte>> ReadPeersProxy = new Dictionary<IPeerProxy, List<byte>>();
        
        public BigInteger PeerId { get; }
        
        public PeerBase(string ipAddress, string port, SHA512 hasher)
        {
            Server = new TcpServer(this);
            Hasher = hasher;
            Server.ClientConnected += OnClientConnected;
            Server.StartServer(ipAddress, port);
        }
        
        public void OnClientConnected(TcpClient client)
        {
            PeerProxy peer = new PeerProxy(client);
            StartClient(peer);
        }

        public Task<PeerProxy> ConnectAsync(string parse, string port)
        {
            return ConnectAsync(IPAddress.Parse(parse), int.Parse(port));
        }
        
        public async Task<PeerProxy> ConnectAsync(IPAddress parse, int port)
        {
            PeerProxy peer = new PeerProxy();
            await peer.ConnectAsync(parse, port);
            StartClient(peer);
            return peer;
        }

        private void StartClient(PeerProxy peer)
        {
            ConnectedPeersProxy.Add(peer);
            ReadPeersProxy[peer] = new List<byte>();
            peer.DataReceived += bytes =>
            {
                StartTask(async () => { await OnDataReceived(peer, bytes); });
            };
            StartTask(async () =>
            {
                await peer.ReadAsync();
            });
        }

        protected async Task OnDataReceived(IPeerProxy peer, byte[] data)
        {
            var readMessage = ReadPeersProxy[peer];
            readMessage.AddRange(new List<byte>(data));
            try
            {
                RawPacket rawPacket = readMessage.DeserializeRawPacket();
                if (IsMacEquals(Hasher.ComputeHash(rawPacket.Payload), rawPacket.MAC))
                {
                    Console.WriteLine("RawPacket is valid. Let's process it ...");
                    await OnClientDataReceived(peer, rawPacket.Payload);
                }
                readMessage.Clear();
            }
            catch (SerializationException ex)
            {
                Console.WriteLine($"SerializationException ex: {ex}");
            }
        }
        
        public Task WriteTo<T>(IPeerProxy peer, T data)
        {
            if (!ConnectedPeersProxy.Contains(peer))
            {
                throw new ArgumentException($"Unknown peerClient: {peer}");
            }

            RawPacket rawPacket = new RawPacket();
            rawPacket.Payload = PreparePeerData(peer, data);
            rawPacket.MAC = Hasher.ComputeHash(rawPacket.Payload);
            byte[] serializedPacket = rawPacket.SerializeToBuffer();
            return peer.SendAsync(serializedPacket);
        }

        protected static bool IsMacEquals(byte[] computedMac, byte[] verifyMac)
        {
            byte[] newVerifyMac = PrepareVerifyMac(computedMac, verifyMac);
            bool isEqual = newVerifyMac.Length == verifyMac.Length;
            for (int i = 0; i < computedMac.Length && i < newVerifyMac.Length; ++i)
            {
                if (computedMac[i] != newVerifyMac[i])
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }

        private static byte[] PrepareVerifyMac(in byte[] computedMac, in byte[] verifyMac)
        {
            byte[] newVerifyMac = new byte[computedMac.Length];
            if (computedMac.Length > verifyMac.Length)
            {
                Array.Copy(verifyMac, 0, newVerifyMac, 0, verifyMac.Length);
                byte[] paddingMac = new byte[computedMac.Length - verifyMac.Length];
                Array.Copy(paddingMac, 0, newVerifyMac, 0, paddingMac.Length);
            }
            else if (computedMac.Length < verifyMac.Length)
            {
                Array.Copy(verifyMac, 0, newVerifyMac, 0, verifyMac.Length - computedMac.Length);
            }
            else
            {
                newVerifyMac = verifyMac;
            }

            return newVerifyMac;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T">
        /// Type marked with attributes System.SerializableAttribute and OpenElection.Shared.Networking.PacketTypeAttribute
        /// </typeparam>
        /// <returns>Serialized packet structure</returns>
        protected abstract byte[] PreparePeerData<T>(IPeerProxy peer, T data);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract Task OnClientDataReceived(IPeerProxy client, byte[] data);
    }
}
