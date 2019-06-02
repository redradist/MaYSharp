using System.Net.Sockets;
using ICCSharp.Networking;

namespace MaY
{
    public interface IPeerProxy : IPeer, ITcpClient
    {
    }
}