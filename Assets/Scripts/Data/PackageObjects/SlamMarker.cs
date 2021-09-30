using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SlamMarker : ICloudItem
    {
        public int Id { get; set; }
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public Color Color;
        public string Message { get; set; }

        public enum MarkerType
        {
            Cube,
            Sphere,
            Crystal,
            SemitransparentCube,
            SemitransparentSphere,
        }

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

        public SlamPoint AsPoint()
        {
            return new SlamPoint(Id, Position, Color, Message);
        }

        public bool Equals(SlamMarker other)
        {
            return Position.Equals(other.Position) && Rotation.Equals(other.Rotation) &&
                    Scale.Equals(other.Scale) && Color.Equals(other.Color) && Id == other.Id &&
                    Message == other.Message && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is SlamMarker other && Equals(other);
        }

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