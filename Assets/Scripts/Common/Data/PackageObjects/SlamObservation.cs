using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamObservation : ICloudItem
    {
        public struct Stats
        {
            public byte Statistics1;
            public byte Statistics2;
            public byte Statistics3;
            public byte Statistics4;
        }

        public SlamPoint Point;
        public Quaternion Rotation;
        public Stats Statistics;
        public string Message { get; set; }
        public SlamPoint AsPoint() => Point;

        public string FileName;

        public SlamObservation(SlamPoint pt, Quaternion orientation, string message, string fileName, Stats stats = new Stats())
        {
            Point = pt;
            Rotation = orientation;
            Statistics = stats;
            Message = message;
            FileName = fileName;
        }

        public static implicit operator SlamPoint(SlamObservation obs) => obs.Point;

        public override string ToString() => Point.Message ?? "SlamObservation";

        public int Id
        {
            get => Point.Id;
            set => Point.Id = value;
        }
    }
}
