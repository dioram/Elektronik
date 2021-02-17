using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamTrackedObject: ICloudItem
    {
        public int Id { get; set; }
        public Color Color;
        public Vector3 Position;
        public Quaternion Rotation;

        public SlamTrackedObject(int id, Color color = default, Vector3 position = default, Quaternion rotation = default)
        {
            Id = id;
            Color = color;
            Position = position;
            Rotation = rotation;
        }

    }
}
