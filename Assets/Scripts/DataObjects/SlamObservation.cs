using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.DataObjects
{
    /// <summary>
    /// Observation or key frame.
    /// Contains position and orientation from where it was taken,
    /// set of points ids it sees, message and path to image.
    /// </summary>
    [Serializable]
    public struct SlamObservation : ICloudItem
    {
        /// <inheritdoc />
        public int Id { get; set; }

        public Vector3 Position;
        public Color Color;
        public Quaternion Rotation;

        /// <summary> Path to image taken at this observation. </summary>
        public string FileName;

        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public SlamPoint ToPoint() => new SlamPoint(Id, Position, Color);

        /// <summary> Ids of points observed in this frame. </summary>
        public HashSet<int> ObservedPoints;

        public SlamObservation(int id, Vector3 position, Color color, Quaternion orientation, string message,
                               string fileName, IList<int> observedPoints = null)
        {
            Id = id;
            Position = position;
            Color = color;
            Rotation = orientation;
            Message = message;
            FileName = fileName;
            ObservedPoints = new HashSet<int>(observedPoints ?? Array.Empty<int>());
        }

        public bool Equals(SlamObservation other)
        {
            return Id.Equals(Id)
                    && Position.Equals(other.Position)
                    && ((Color32)Color).Equals((Color32)other.Color)
                    && Rotation.Equals(other.Rotation)
                    && FileName == other.FileName
                    && Message == other.Message;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is SlamObservation other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Id;
    }
}