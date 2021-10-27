using System;
using System.Diagnostics.CodeAnalysis;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataObjects
{
    /// <summary> Colored point in 3d space.  </summary>
    [Serializable]
    public struct SlamPoint : ICloudItem, ILookableDataSource
    {
        /// <inheritdoc />
        public int Id { get; set; }

        /// <summary> Position in 3d space. </summary>
        public Vector3 Position;

        public Color Color;

        /// <inheritdoc />
        public string Message { get; set; }


        /// <inheritdoc />
        public SlamPoint ToPoint() => this;

        public SlamPoint(int id, Vector3 position = default, Color color = default, string message = "")
        {
            Id = id;
            Position = position;
            Color = color;
            Message = message;
        }

        public Pose Look(Transform transform)
        {
            var startPos = Position;
            var endPos = transform.position;
            return new Pose(startPos + (endPos - startPos).normalized, Quaternion.LookRotation(startPos - endPos));
        }

        public bool Equals(SlamPoint other)
        {
            return Position.Equals(other.Position)
                    && ((Color32)Color).Equals((Color32)other.Color)
                    && Id == other.Id && Message == other.Message;
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