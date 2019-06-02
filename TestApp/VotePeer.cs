using System;
using System.Threading.Tasks;
using ICCSharp;
using MaY;

namespace TestClient
{
    public class OpenElectionPeer : Peer
    {
        protected override async Task OnClientDataReceived(IPeerProxy client, byte[] data, int bytesRead)
        {
            PeerPacketHeader packetHeader =  data.DeserializePeerPacketHeader();
            switch ((MessageType) packetHeader.Type)
            {
                case MessageType.SYSTEM:
                {
                    
                }
                    break;
                case MessageType.MSG:
                {
                    
                }
                    break;
                case MessageType.VOTE:
                {
                    PeerPacket<Vote> votePacket = data.DeserializePeerPacket<Vote>();
                    Vote vote = votePacket.Payload;
                    switch (vote.Ogranization)
                    {
                        case OrganizationType.Union:
                            break;
                        case OrganizationType.Company:
                            break;
                        case OrganizationType.Village:
                            break;
                        case OrganizationType.City:
                            break;
                        case OrganizationType.Government:
                            break;
                        case OrganizationType.Country:
                            break;
                        case OrganizationType.Anonymous:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                    break;
            }
        }
    }
}