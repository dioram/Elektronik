using UnityEngine;

namespace Elektronik.Common.Events
{
    public interface ISlamEvent
    {
        SlamEventType EventType { get; }
        int Timestamp { get; }
        bool IsKeyEvent { get; }
        SlamObservation[] Observations { get; }
        SlamPoint[] Points { get; }
        SlamLine[] Lines { get; }
    }
}
