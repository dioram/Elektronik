using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamObservation : ICloudItem
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
        public string message;
        public string fileName;

        public SlamObservation(SlamPoint pt, Quaternion orientation, string message, string fileName, Stats stats = new Stats())
        {
            point = pt;
            rotation = orientation;
            statistics = stats;
            this.message = message;
            this.fileName = fileName;
        }

        public static implicit operator SlamPoint(SlamObservation obs) => obs.point;

        public override string ToString() => point.message ?? "SlamObservation";

        public int Id
        {
            get => point.Id;
            set => point.Id = value;
        }
    }
}
