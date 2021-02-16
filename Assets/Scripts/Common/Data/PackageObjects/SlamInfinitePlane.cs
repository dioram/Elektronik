using Elektronik.Common.Clouds;
using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamInfinitePlane : ICloudItem
    {
        public int id;
        public Vector3 offset;
        public Vector3 normal;
        public Color color;
        public string message;

        public int Id
        {
            get => id;
            set => id = value;
        }
    }
}