using Elektronik.Common.Data.Pb;
using System;
using System.Collections.Generic;
using System.Text;

namespace csharp
{
    public interface ITest
    {
        IEnumerable<PacketPb> Create();
        IEnumerable<PacketPb> Update();
        IEnumerable<PacketPb> Remove();
        IEnumerable<PacketPb> Clear();
    }
}
