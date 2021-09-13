using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    [Serializable]
    public struct SlamTrackedObject : ICloudItem
    {
        public int Id { get; set; }
        public Color Color;
        public Vector3 Position;
        public Quaternion Rotation;
        public string Message { get; set; }

        public SlamPoint AsPoint() => new SlamPoint(Id, Position, Color);

        public SlamTrackedObject(int id, Vector3 position, Quaternion rotation, Color color = default,
                                 string message = "")
        {
            Id = id;
            Color = color;
            Position = position;
            Rotation = rotation;
            Message = message;
        }

        public bool Equals(SlamTrackedObject other)
        {
            return ((Color32) Color).Equals((Color32) other.Color) && Position.Equals(other.Position) &&
                    Rotation.Equals(other.Rotation) && Id == other.Id && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            return obj is SlamTrackedObject other && Equals(other);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Color.GetHashCode();
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ Id;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}