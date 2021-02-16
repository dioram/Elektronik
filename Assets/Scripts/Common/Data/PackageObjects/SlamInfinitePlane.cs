using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamInfinitePlane : ICloudItem
    {
        public int Id { get; set; }
        public Vector3 offset;
        public Vector3 normal;
        public Color color;
        public string message;
    }
}