using Elektronik.Common.Data.Pb;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elektronik.Common.Data.Pb
{
    public partial class PacketPb
    {
        // add auto increment for timestamp
        private static int m_timestampCounter = 0;

        partial void OnConstruction()
        {
            Timestamp = m_timestampCounter++;
        }
    }
}
