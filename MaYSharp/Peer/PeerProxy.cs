using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Threading.Tasks;
using ICCSharp.Networking;

namespace MaY
{
    public class PeerProxy : IPeerProxy
    {
        private readonly ITcpClient _client;
        public BigInteger PeerId { get; internal set; }

        public event Action<byte[]> DataReceived;
        
        public PeerProxy()
        {
            _client = new ICCSharp.Networking.TcpClient();
        }
        
        public PeerProxy(ITcpClient client)
        {
            _client = client;
            _client.DataReceived += bytes => { OnDataReceived(bytes); };
        }

        public Task ConnectAsync(IPAddress parse, int port)
        {
            return _client.ConnectAsync(parse, port);
        }

        public Task SendAsync(byte[] buffer)
        {
            return _client.SendAsync(buffer);
        }

        public Task ReadAsync()
        {
            return _client.ReadAsync();
        }

        protected virtual void OnDataReceived(byte[] obj)
        {
            DataReceived?.Invoke(obj);
        }
    }
}