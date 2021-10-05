using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    [Serializable]
    public struct SlamObservation : ICloudItem
    {
        public SlamPoint Point;
        public Quaternion Rotation;
        public string Message { get; set; }
        public SlamPoint AsPoint() => Point;

        public string FileName;
        public HashSet<int> ObservedPoints;

        public SlamObservation(SlamPoint pt, Quaternion orientation, string message, string fileName,
                               IList<int> observedPoints = null)
        {
            Point = pt;
            Rotation = orientation;
            Message = message;
            FileName = fileName;
            ObservedPoints = new HashSet<int>(observedPoints ?? Array.Empty<int>());
        }

        public static implicit operator SlamPoint(SlamObservation obs) => obs.Point;

        public override string ToString() => Point.Message ?? "SlamObservation";

        public int Id
        {
            get => Point.Id;
            set => Point.Id = value;
        }

        public bool Equals(SlamObservation other)
        {
            return Point.Equals(other.Point) && Rotation.Equals(other.Rotation) && FileName == other.FileName && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            return obj is SlamObservation other && Equals(other);
        }

        public override int GetHashCode() => Id;
    }
}