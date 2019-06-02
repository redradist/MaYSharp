using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TestClient;

namespace TestServer
{
    class TcpAcceptConnectionHelper
    {
        private static TcpListener listener { get; set; }
        private static bool accept { get; set; } = false;

        public static void StartServer(in string ipAddress, in int port)
        {
            IPAddress address = IPAddress.Parse(ipAddress);
            listener = new TcpListener(address, port);

            listener.Start();
            accept = true;

            Console.WriteLine($"Server started. Listening to TCP clients at 127.0.0.1:{port}");
        }

        static T Deserialize<T>(byte[] buffer)
        {
            Stream stream = new MemoryStream(buffer);
            // Serialize an object into the storage medium referenced by 'stream' object.
            BinaryFormatter formatter = new BinaryFormatter();
            // Serialize multiple objects into the stream
            return (T) formatter.Deserialize(stream);
        }
        
        public static async Task ListenAsync()
        {
            if (listener != null && accept)
            {
                // Continue listening.  
                while (true)
                {
                    Console.WriteLine("Waiting for client...");
                    var client = await listener.AcceptTcpClientAsync(); // Get the client  
                    if (client != null)
                    {
                        Console.WriteLine("Client connected. Waiting for data.");
                        string message = "";

                        while (message != null && !message.StartsWith("quit"))
                        {
//                            byte[] data = Encoding.UTF8.GetBytes("Send next data: [enter 'quit' to terminate] ");
//                            client.GetStream().Write(data, 0, data.Length);

                            byte[] buffer = new byte[1024];
                            await client.GetStream().ReadAsync(buffer, 0, buffer.Length);

                            var vote = Deserialize<Vote>(buffer);
                            Console.WriteLine(value: $"vote(organizationType = {vote.Ogranization}, voteType = {vote.VoteType}, voteResult = {vote.VoteResult})");
                        }
                        Console.WriteLine("Closing connection.");
                        client.GetStream().Dispose();
                    }
                }
            }
        }
    }
}
