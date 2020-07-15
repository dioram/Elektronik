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

        public SlamPoint Point { get; set; }
        public Quaternion Orientation { get; set; }
        public Stats Statistics { get; set; }

        public SlamObservation(SlamPoint pt, Quaternion orientation, Stats stats = new Stats())
        {
            Point = pt;
            Orientation = orientation;
            Statistics = stats;
        }

        public static implicit operator SlamPoint(SlamObservation obs) => obs.Point;

        public override string ToString() => Point.message ?? "SlamObservation";
    }
}
