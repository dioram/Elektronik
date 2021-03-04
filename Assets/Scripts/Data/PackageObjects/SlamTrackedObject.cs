using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SlamTrackedObject : ICloudItem
    {
        public int Id { get; set; }
        public Color Color;
        public Vector3 Position;
        public Quaternion Rotation;
        public string Message { get; set; }

        public SlamPoint AsPoint() => new SlamPoint(Id, Position, Color);

        public SlamTrackedObject(int id, Color color = default, Vector3 position = default,
                                 Quaternion rotation = default)
        {
            Id = id;
            Color = color;
            Position = position;
            Rotation = rotation;
            Message = "";
        }
    }
}