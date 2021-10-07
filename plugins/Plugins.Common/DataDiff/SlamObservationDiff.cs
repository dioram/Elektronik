using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Plugins.Common.DataDiff
{
    public struct SlamObservationDiff : ICloudItemDiff<SlamObservationDiff, SlamObservation>
    {
        public int Id => Point.Id;
        public SlamPointDiff Point;
        public Quaternion? Rotation;
        public IList<int>? ObservedPoints;
        public string? Message;
        public string? FileName;

        public SlamObservationDiff(SlamPointDiff point, Quaternion? rotation = null, IList<int>? observedPoints = null,
                                   string? message = null, string? fileName = null)
        {
            Point = point;
            Rotation = rotation;
            ObservedPoints = observedPoints;
            Message = message;
            FileName = fileName;
        }

        public SlamObservation Apply()
        {
            return new SlamObservation(Point.Apply(),
                                       Rotation ?? Quaternion.identity,
                                       string.IsNullOrEmpty(Message) ? "" : Message,
                                       string.IsNullOrEmpty(FileName) ? "" : FileName,
                                       ObservedPoints);
        }

        public SlamObservation Apply(SlamObservation item)
        {
            item.Point = Point.Apply(item.Point);
            item.Rotation = Rotation ?? item.Rotation;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            item.FileName = string.IsNullOrEmpty(FileName) ? item.FileName : FileName;
            if (ObservedPoints is { Count: > 0 })
            {
                item.ObservedPoints = new HashSet<int>(ObservedPoints);
            }

            return item;
        }

        public SlamObservationDiff Apply(SlamObservationDiff right)
        {
            if (Id != right.Id) throw new Exception("Ids must be identical!");
            var observedPoints = right.ObservedPoints is null || right.ObservedPoints.Count == 0
                    ? ObservedPoints
                    : right.ObservedPoints;
            return new SlamObservationDiff(Point.Apply(right.Point),
                                           right.Rotation ?? Rotation,
                                           observedPoints,
                                           string.IsNullOrEmpty(right.Message) ? Message : right.Message,
                                           string.IsNullOrEmpty(right.FileName) ? FileName : right.FileName);
        }

        public bool Equals(SlamObservationDiff other)
        {
            return Point.Equals(other.Point) 
                    && Nullable.Equals(Rotation, other.Rotation) 
                    && IsObservedEquals(ObservedPoints, other.ObservedPoints)
                    && ((string.IsNullOrEmpty(Message) && string.IsNullOrEmpty(other.Message)) || Message == other.Message)
                    && ((string.IsNullOrEmpty(FileName) && string.IsNullOrEmpty(other.FileName)) || FileName == other.FileName);
        }

        private bool IsObservedEquals(IList<int>? left, IList<int>? right)
        {
            var isLeftEmpty = left == null || left.Count == 0;
            var isRightEmpty = right == null || right.Count == 0;
            if (isLeftEmpty && isRightEmpty) return true;
            if (left!.Count != right!.Count) return false;
            return left.Zip(right, (i, i1) => i == i1).All(b => b);
        }

        public override bool Equals(object? obj)
        {
            return obj is SlamObservationDiff other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Point.GetHashCode();
                hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ (ObservedPoints != null ? ObservedPoints.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FileName != null ? FileName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}