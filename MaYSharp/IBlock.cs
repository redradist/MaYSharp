using System;
using System.Collections.Generic;
using System.Text;

namespace MaY
{
    public interface IBlock
    {
        ulong Version { get; }
        byte[] Buffer { get; }
        byte[] Hash { get; }
    }
}
