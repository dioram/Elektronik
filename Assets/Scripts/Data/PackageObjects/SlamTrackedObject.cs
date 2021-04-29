using System;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    [Serializable]
    public struct SlamTrackedObject : ICloudItem
    {
        public int Id { get; set; }
        public Color Color;
        public Vector3 Position;
        public Quaternion Rotation;
        public string Message { get; set; }

        public SlamPoint AsPoint() => new SlamPoint(Id, Position, Color);

        public SlamTrackedObject(int id, Vector3 position, Quaternion rotation, Color color = default)
        {
            Id = id;
            Color = color;
            Position = position;
            Rotation = rotation;
            Message = "";
        }
    }
}