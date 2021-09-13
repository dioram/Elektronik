using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    [Serializable]
    public struct SlamInfinitePlane : ICloudItem
    {
        public int Id { get; set; }
        public Vector3 Offset;
        public Vector3 Normal;
        public Color Color;
        public string Message { get; set; }
        public SlamPoint AsPoint() => new SlamPoint(Id, Offset, Color);

        public SlamInfinitePlane(int id, Vector3 offset, Vector3 normal, Color color, string message = "")
        {
            Id = id;
            Offset = offset;
            Normal = normal;
            Color = color;
            Message = message;
        }

        public bool Equals(SlamInfinitePlane other)
        {
            return Offset.Equals(other.Offset) && Normal.Equals(other.Normal) &&
                    ((Color32) Color).Equals((Color32) other.Color) && Id == other.Id && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            return obj is SlamInfinitePlane other && Equals(other);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Offset.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ Id;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}