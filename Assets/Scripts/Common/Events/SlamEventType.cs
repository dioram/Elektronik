using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.Events
{
    public enum SlamEventType : byte
    {
        Invalid = 0xFF,
        MainThreadEvent = 0x00,
        GlobalMap = 0x01,
        LMPointsRemoval = 0x10,
        LMObservationRemoval = 0x11,
        LMPointsFusion = 0x12,
        LMLBA = 0x13,
        LCPointsFusion = 0x20,
        LCOptimizeEssentialGraph = 0x21,
        LCGBA = 0x22,
        LCLoopClosingTry = 0x23,
    }
}
