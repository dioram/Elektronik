using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.Plugins.Common.DataDiff
{
    public struct SlamObservationDiff : ICloudItemDiff<SlamObservationDiff, SlamObservation>
    {
        public int Id { get; }
        public Vector3? Position;
        public Color? Color;
        public Quaternion? Rotation;
        public IList<int>? ObservedPoints;
        public string? Message;
        public string? FileName;

        public SlamObservationDiff(int id, Vector3? position = null, Color? color = null, Quaternion? rotation = null,
                                   IList<int>? observedPoints = null,
                                   string? message = null, string? fileName = null)
        {
            Id = id;
            Position = position;
            Color = color;
            Rotation = rotation;
            ObservedPoints = observedPoints;
            Message = message;
            FileName = fileName;
        }


        public SlamObservation Apply()
        {
            return new SlamObservation(Id,
                                       Position ?? Vector3.zero,
                                       Color ?? UnityEngine.Color.black,
                                       Rotation ?? Quaternion.identity,
                                       string.IsNullOrEmpty(Message) ? "" : Message,
                                       string.IsNullOrEmpty(FileName) ? "" : FileName,
                                       ObservedPoints);
        }

        public SlamObservation Apply(SlamObservation item)
        {
            item.Position = Position ?? item.Position;
            item.Color = Color ?? item.Color;
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
            return new SlamObservationDiff(Id,
                                           right.Position ?? Position,
                                           right.Color ?? Color,
                                           right.Rotation ?? Rotation,
                                           observedPoints,
                                           string.IsNullOrEmpty(right.Message) ? Message : right.Message,
                                           string.IsNullOrEmpty(right.FileName) ? FileName : right.FileName);
        }

        public bool Equals(SlamObservationDiff other)
        {
            return Id == other.Id
                   && Nullable.Equals(Position, other.Position)
                   && (!Color.HasValue && !Color.HasValue || Color.Value.Equals((Color32)other.Color!.Value))
                   && Nullable.Equals(Rotation, other.Rotation)
                   && IsObservedEquals(ObservedPoints, other.ObservedPoints)
                   && ((string.IsNullOrEmpty(Message) && string.IsNullOrEmpty(other.Message)) ||
                       Message == other.Message)
                   && ((string.IsNullOrEmpty(FileName) && string.IsNullOrEmpty(other.FileName)) ||
                       FileName == other.FileName);
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
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ Id;
                hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ (ObservedPoints != null ? ObservedPoints.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FileName != null ? FileName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}