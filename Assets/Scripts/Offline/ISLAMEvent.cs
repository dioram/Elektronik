using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Offline
{
    interface ISlamEvent
    {
        SlamEventType EventType { get; set; }
    }
}
