using System;
using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.Plugins.Common.DataDiff
{
    public struct SlamPointDiff : ICloudItemDiff<SlamPointDiff, SlamPoint>
    {
        public int Id { get; }
        public Vector3? Position;
        public Color? Color;
        public string? Message;

        public SlamPointDiff(int id, Vector3? position = null, Color? color = null, string? message = null)
        {
            Id = id;
            Position = position;
            Color = color;
            Message = message;
        }

        public SlamPointDiff(SlamPoint point)
        {
            Id = point.Id;
            Position = point.Position;
            Color = point.Color;
            Message = point.Message;
        }

        public SlamPoint Apply()
        {
            return new SlamPoint(Id,
                                 Position ?? Vector3.zero,
                                 Color ?? UnityEngine.Color.black,
                                 string.IsNullOrEmpty(Message) ? "" : Message);
        }

        public SlamPoint Apply(SlamPoint item)
        {
            item.Position = Position ?? item.Position;
            item.Color = Color ?? item.Color;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            return item;
        }

        public SlamPointDiff Apply(SlamPointDiff right)
        {
            if (Id != right.Id) throw new Exception("Ids must be identical!");
            return new SlamPointDiff(Id,
                                     right.Position ?? Position,
                                     right.Color ?? Color,
                                     string.IsNullOrEmpty(right.Message) ? Message : right.Message);
        }

        public bool Equals(SlamPointDiff other)
        {
            return Nullable.Equals(Position, other.Position) 
                    && (!Color.HasValue && !Color.HasValue || Color.Value.Equals((Color32)other.Color!.Value))
                    && ((string.IsNullOrEmpty(Message) && string.IsNullOrEmpty(other.Message)) || Message == other.Message)
                    && Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is SlamPointDiff other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Id;
                return hashCode;
            }
        }
    }
}