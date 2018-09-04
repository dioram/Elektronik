using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.Events
{
    public enum SlamEventType : byte
    {
        Invalid = 0xFF,
        MainThreadEvent = 0x00,
        LMPointsRemoval = 0x10,
        LMPointsFusion = 0x11,
        LMObservationRemoval = 0x12,
        LMLBA = 0x13,
        LCPointsFusion = 0x20,
        LCOptimizeEssentialGraph = 0x21,
        LCGBA = 0x22,
        LCLoopClosingTry = 0x23,
    }
}
