using System;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Plugins.Common.DataDiff
{
    public struct SlamTrackedObjectDiff : ICloudItemDiff<SlamTrackedObjectDiff, SlamTrackedObject>
    {
        public int Id { get; }
        public Color? Color;
        public Vector3? Position;
        public Quaternion? Rotation;
        public string? Message;

        public SlamTrackedObjectDiff(int id, Vector3? position = null, Quaternion? rotation = null, Color? color = null,
                                     string? message = null)
        {
            Id = id;
            Color = color;
            Position = position;
            Rotation = rotation;
            Message = message;
        }

        public SlamTrackedObjectDiff(SlamTrackedObject obj)
        {
            Id = obj.Id;
            Color = obj.Color;
            Position = obj.Position;
            Rotation = obj.Rotation;
            Message = obj.Message;
        }

        public SlamTrackedObject Apply()
        {
            return new SlamTrackedObject(Id,
                                         Position ?? Vector3.zero,
                                         Rotation ?? Quaternion.identity,
                                         Color ?? UnityEngine.Color.black,
                                         string.IsNullOrEmpty(Message) ? "" : Message);
        }

        public SlamTrackedObject Apply(SlamTrackedObject item)
        {
            item.Position = Position ?? item.Position;
            item.Rotation = Rotation ?? item.Rotation;
            item.Color = Color ?? item.Color;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            return item;
        }

        public SlamTrackedObjectDiff Apply(SlamTrackedObjectDiff right)
        {
            if (Id != right.Id) throw new Exception("Ids must be identical!");
            return new SlamTrackedObjectDiff(Id,
                                             Position = right.Position ?? Position,
                                             Rotation = right.Rotation ?? Rotation,
                                             Color = right.Color ?? Color,
                                             Message = string.IsNullOrEmpty(right.Message) ? Message : right.Message);
        }

        public bool Equals(SlamTrackedObjectDiff other)
        {
            return ((!Color.HasValue && !Color.HasValue) || ((Color32)Color).Equals((Color32)other.Color))
                    && Nullable.Equals(Position, other.Position)
                    && Nullable.Equals(Rotation, other.Rotation)
                    && ((string.IsNullOrEmpty(Message) && string.IsNullOrEmpty(other.Message)) ||
                        Message == other.Message)
                    && Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is SlamTrackedObjectDiff other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Color.GetHashCode();
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Id;
                return hashCode;
            }
        }
    }
}