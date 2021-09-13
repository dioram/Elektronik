using System;
using System.Diagnostics.CodeAnalysis;
using Elektronik.Containers.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    [Serializable]
    public struct SlamPoint : ICloudItem, ILookable
    {
        public int Id { get; set; }
        public Vector3 Position;
        public Color Color;
        public string Message { get; set; }
        public SlamPoint AsPoint() => this;

        public SlamPoint(int id, Vector3 position, Color color, string message = "")
        {
            Id = id;
            Position = position;
            Color = color;
            Message = message;
        }

        public override string ToString()
        {
            return Message ?? "SlamPoint";
        }

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            return (Position + (transform.position - Position).normalized,
                    Quaternion.LookRotation(Position - transform.position));
        }

        public bool Equals(SlamPoint other)
        {
            return Position.Equals(other.Position) && ((Color32)Color).Equals((Color32)other.Color) && Id == other.Id && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            return obj is SlamPoint other && Equals(other);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ Id;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}