using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Elektronik.DataObjects
{
    /// <summary> Visualization marker. </summary>
    public struct SlamMarker : ICloudItem
    {
        /// <inheritdoc />
        public int Id { get; set; }
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public Color Color;

        /// <inheritdoc />
        public string Message { get; set; }

        /// <summary> Primitives for rendering. </summary>
        public enum MarkerType
        {
            Cube,
            Sphere,
            Crystal,
            SemitransparentCube,
            SemitransparentSphere,
        }

        /// <summary> Primitive that will be rendered. </summary>
        public MarkerType Type;

        public SlamMarker(int id, Vector3 position, Quaternion rotation, Vector3 scale, Color color, string message,
                          MarkerType type)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Color = color;
            Message = message;
            Type = type;
        }

        /// <inheritdoc />
        public SlamPoint ToPoint()
        {
            return new SlamPoint(Id, Position, Color, Message);
        }

        public bool Equals(SlamMarker other)
        {
            return Position.Equals(other.Position) && Rotation.Equals(other.Rotation) &&
                    Scale.Equals(other.Scale) && Color.Equals(other.Color) && Id == other.Id &&
                    Message == other.Message && Type == other.Type;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SlamMarker other && Equals(other);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ Scale.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ Id;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}