using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MaY;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            StartClient();
            Console.WriteLine("Task is finished !!");
        }

        static void StartClient()
        {
            Peer peer = new OpenElectionPeer();
            Console.WriteLine("Start connection ...");
            _ = peer.StartTask(async () =>
              {
                  await peer.ConnectAsync(IPAddress.Parse("127.0.0.1"), 49000);
              });
            peer.Run();
        }

        private static async Task SendResponse(NetworkStream stream)
        {
            var packet = new PeerPacket<Vote>();
            Vote vote = new Vote()
            {
                Ogranization = OrganizationType.Country,
                VoteType = (int) CountryVoteType.President
            };
            packet.Payload = vote;
            await stream.CopyToAsync(packet.SerializePeerPacket());
            await stream.FlushAsync();
        }
    }
}
