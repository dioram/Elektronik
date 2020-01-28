using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public struct CovisibleInfo
        {
            public int id;
            public int sharedPointsCount;
        }

        public SlamPoint Point { get; set; }
        public Quaternion Orientation { get; set; }
        public Stats Statistics { get; set; }
        public ReadOnlyCollection<CovisibleInfo> CovisibleInfos { get; private set; }

        public SlamObservation(List<CovisibleInfo> covisibleObservationsInfo)
        {
            Point = new SlamPoint();
            Orientation = new Quaternion();
            Statistics = new Stats();
            CovisibleInfos = new ReadOnlyCollection<CovisibleInfo>(covisibleObservationsInfo);
        }

        public SlamObservation(SlamPoint pt, Quaternion orientation, Stats stats = new Stats())
        {
            Point = pt;
            Orientation = orientation;
            Statistics = stats;
            CovisibleInfos = new ReadOnlyCollection<CovisibleInfo>(new List<CovisibleInfo>());
        }

        public static implicit operator SlamPoint(SlamObservation obs) => obs.Point;

        public override string ToString() => Point.message ?? "SlamObservation";
    }
}
