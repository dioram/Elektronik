using System;
using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.Plugins.Common.DataDiff
{
    public struct SlamMarkerDiff : ICloudItemDiff<SlamMarkerDiff, SlamMarker>
    {
        public int Id { get; }
        public Vector3? Position;
        public Quaternion? Rotation;
        public Vector3? Scale;
        public Color? Color;
        public string? Message;
        public SlamMarker.MarkerType? Type;

        public SlamMarkerDiff(int id, Vector3? position, Quaternion? rotation, Vector3? scale, Color? color,
                              string? message, SlamMarker.MarkerType? type)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Color = color;
            Message = message;
            Type = type;
        }

        public SlamMarkerDiff(SlamMarker marker)
        {
            Id = marker.Id;
            Position = marker.Position;
            Rotation = marker.Rotation;
            Scale = marker.Scale;
            Color = marker.Color;
            Message = marker.Message;
            Type = marker.Type;
        }

        public SlamMarker Apply()
        {
            return new SlamMarker(Id,
                                  Position ?? Vector3.zero,
                                  Rotation ?? Quaternion.identity,
                                  Scale ?? Vector3.one,
                                  Color ?? UnityEngine.Color.black,
                                  string.IsNullOrEmpty(Message) ? "" : Message,
                                  Type ?? SlamMarker.MarkerType.Sphere);
        }

        public SlamMarker Apply(SlamMarker item)
        {
            item.Position = Position ?? item.Position;
            item.Rotation = Rotation ?? item.Rotation;
            item.Scale = Scale ?? item.Scale;
            item.Color = Color ?? item.Color;
            item.Type = Type ?? item.Type;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            return item;
        }

        public SlamMarkerDiff Apply(SlamMarkerDiff item)
        {
            if (Id != item.Id) throw new Exception("Ids must be identical!");
            return new SlamMarkerDiff(Id,
                                      item.Position ?? Position,
                                      item.Rotation ?? Rotation,
                                      item.Scale ?? Scale,
                                      item.Color ?? Color,
                                      string.IsNullOrEmpty(item.Message) ? Message : item.Message,
                                      item.Type ?? Type);
        }

        public bool Equals(SlamMarkerDiff other)
        {
            return Nullable.Equals(Position, other.Position) 
                    && Nullable.Equals(Rotation, other.Rotation) 
                    && Nullable.Equals(Scale, other.Scale) 
                    && (!Color.HasValue && !Color.HasValue || Color.Value.Equals((Color32)other.Color!.Value))
                    && (string.IsNullOrEmpty(Message) && string.IsNullOrEmpty(other.Message) || Message == other.Message)
                    && Type == other.Type 
                    && Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is SlamMarkerDiff other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ Scale.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                hashCode = (hashCode * 397) ^ Id;
                return hashCode;
            }
        }
    }
}