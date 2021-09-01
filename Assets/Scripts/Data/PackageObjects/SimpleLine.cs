using System;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SimpleLine : ICloudItem
    {
        public int Id { get; set; }
        public string Message { get; set; }

        public Vector3 BeginPos;
        public Vector3 EndPos;
        public Color BeginColor;
        public Color EndColor;

        public SimpleLine(int id, Vector3 beginPos, Vector3 endPos, Color beginColor, Color endColor) : this()
        {
            Id = id;
            BeginPos = beginPos;
            EndPos = endPos;
            BeginColor = beginColor;
            EndColor = endColor;
        }

        public SimpleLine(int id, Vector3 beginPos, Vector3 endPos, Color color = default) : this()
        {
            Id = id;
            BeginPos = beginPos;
            EndPos = endPos;
            BeginColor = color;
            EndColor = color;
        }

        public SlamPoint AsPoint() => throw new InvalidCastException("Cannot get line as point");

        public bool Equals(SimpleLine other)
        {
            return BeginPos.Equals(other.BeginPos) && EndPos.Equals(other.EndPos) 
                    && ((Color32)BeginColor).Equals((Color32)other.BeginColor) 
                    && ((Color32)EndColor).Equals((Color32)other.EndColor) 
                    && Id == other.Id && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            return obj is SimpleLine other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = BeginPos.GetHashCode();
                hashCode = (hashCode * 397) ^ EndPos.GetHashCode();
                hashCode = (hashCode * 397) ^ BeginColor.GetHashCode();
                hashCode = (hashCode * 397) ^ EndColor.GetHashCode();
                hashCode = (hashCode * 397) ^ Id;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}