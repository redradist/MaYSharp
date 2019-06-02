using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TestClient
{
    [Serializable]
    public class Vote
    {
        public OrganizationType Ogranization;
        public int VoteType;
        public int VoteResult;
    }
}
