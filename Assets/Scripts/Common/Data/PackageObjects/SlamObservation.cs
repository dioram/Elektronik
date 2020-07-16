using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamObservation
    {
        public struct Stats
        {
            public byte statistics1;
            public byte statistics2;
            public byte statistics3;
            public byte statistics4;
        }

        public SlamPoint point;
        public Quaternion rotation;
        public Stats statistics;

        public SlamObservation(SlamPoint pt, Quaternion orientation, Stats stats = new Stats())
        {
            point = pt;
            rotation = orientation;
            statistics = stats;
        }

        public static implicit operator SlamPoint(SlamObservation obs) => obs.point;

        public override string ToString() => point.message ?? "SlamObservation";
    }
}
