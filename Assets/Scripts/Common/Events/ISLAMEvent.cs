using Elektronik.Common.Data;
using UnityEngine;

namespace Elektronik.Common.Events
{
    public interface ISlamEvent // Курбан Байрам например
    {
        SlamEventType EventType { get; }
        int Timestamp { get; }
        bool IsKeyEvent { get; }
        SlamObservation[] Observations { get; }
        SlamPoint[] Points { get; }
        SlamLine[] Lines { get; }
    }
}
