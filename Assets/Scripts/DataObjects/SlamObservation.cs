using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.DataObjects
{
    /// <summary>
    /// Observation or key frame.
    /// Contains position and orientation from where it was taken,
    /// set of points it sees and path to image.
    /// </summary>
    [Serializable]
    public struct SlamObservation : ICloudItem
    {
        public SlamPoint Point;
        public Quaternion Rotation;

        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public SlamPoint ToPoint() => Point;

        public string FileName;
        
        /// <summary> Ids of points observed in this frame. </summary>
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

        public int Id
        {
            get => Point.Id;
            set => Point.Id = value;
        }

        public bool Equals(SlamObservation other)
        {
            return Point.Equals(other.Point) && Rotation.Equals(other.Rotation) 
                    && FileName == other.FileName && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            return obj is SlamObservation other && Equals(other);
        }

        public override int GetHashCode() => Id;
    }
}