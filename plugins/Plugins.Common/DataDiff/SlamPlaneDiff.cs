using System;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Plugins.Common.DataDiff
{
    public struct SlamPlaneDiff : ICloudItemDiff<SlamPlaneDiff, SlamPlane>
    {
        public int Id { get; }
        public Vector3? Offset;
        public Vector3? Normal;
        public Color? Color;
        public string? Message;

        public SlamPlaneDiff(int id, Vector3? offset = null, Vector3? normal = null, Color? color = null,
                             string? message = null)
        {
            Id = id;
            Offset = offset;
            Normal = normal;
            Color = color;
            Message = message;
        }

        public SlamPlaneDiff(SlamPlane plane)
        {
            Id = plane.Id;
            Offset = plane.Offset;
            Normal = plane.Normal;
            Color = plane.Color;
            Message = plane.Message;
        }

        public SlamPlane Apply()
        {
            return new SlamPlane(Id,
                                 Offset ?? Vector3.zero,
                                 Normal ?? Vector3.zero,
                                 Color ?? UnityEngine.Color.black,
                                 string.IsNullOrEmpty(Message) ? "" : Message);
        }

        public SlamPlane Apply(SlamPlane item)
        {
            item.Offset = Offset ?? item.Offset;
            item.Normal = Normal ?? item.Normal;
            item.Color = Color ?? item.Color;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            return item;
        }

        public SlamPlaneDiff Apply(SlamPlaneDiff right)
        {
            if (Id != right.Id) throw new Exception("Ids must be identical!");
            return new SlamPlaneDiff(Id,
                                     right.Offset ?? Offset,
                                     right.Normal ?? Normal,
                                     right.Color ?? Color,
                                     string.IsNullOrEmpty(right.Message) ? Message : right.Message);
        }

        public bool Equals(SlamPlaneDiff other)
        {
            return Nullable.Equals(Offset, other.Offset)
                    && Nullable.Equals(Normal, other.Normal)
                    && (!Color.HasValue && !Color.HasValue || Color.Value.Equals((Color32)other.Color!.Value))
                    && ((string.IsNullOrEmpty(Message) && string.IsNullOrEmpty(other.Message)) ||
                        Message == other.Message)
                    && Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is SlamPlaneDiff other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Offset.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Id;
                return hashCode;
            }
        }
    }
}