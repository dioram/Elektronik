using System;
using UnityEngine;

namespace Elektronik.DataObjects
{
    /// <summary> Colored line between two points in 3d space. </summary>
    public struct SimpleLine : ICloudItem
    {
        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public string Message { get; set; }

        /// <summary> Position of line start. </summary>
        public Vector3 BeginPos;

        /// <summary> Position of line end. </summary>
        public Vector3 EndPos;

        /// <summary> Color of line start. </summary>
        public Color BeginColor;

        /// <summary> Color of line end. </summary>
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

        /// <inheritdoc />
        public SlamPoint ToPoint() => throw new InvalidCastException("Cannot get line as point");

        /// <inheritdoc cref="Equals(object)"/>
        public bool Equals(SimpleLine other)
        {
            return BeginPos.Equals(other.BeginPos) && EndPos.Equals(other.EndPos)
                    && ((Color32)BeginColor).Equals((Color32)other.BeginColor)
                    && ((Color32)EndColor).Equals((Color32)other.EndColor)
                    && Id == other.Id && Message == other.Message;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SimpleLine other && Equals(other);
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        /// <inheritdoc />
        public override int GetHashCode() => Id;
    }
}