using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamInfinitePlane : ICloudItem
    {
        public int Id { get; set; }
        public Vector3 Offset;
        public Vector3 Normal;
        public Color Color;
        public string Message;
    }
}