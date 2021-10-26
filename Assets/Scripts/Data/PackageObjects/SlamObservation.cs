using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    [Serializable]
    public struct SlamObservation : ICloudItem
    {
        public int Id { get; set; }
        public Vector3 Position;
        public Color Color;
        public Quaternion Rotation;
        public string Message { get; set; }
        public SlamPoint AsPoint() => new SlamPoint(Id, Position, Color);

        public string FileName;
        public HashSet<int> ObservedPoints;

        public SlamObservation(int id, Vector3 position, Color color, Quaternion orientation, string message,
                               string fileName,
                               IList<int> observedPoints = null)
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

        public override bool Equals(object obj)
        {
            return obj is SlamObservation other && Equals(other);
        }

        public override int GetHashCode() => Id;
    }
}