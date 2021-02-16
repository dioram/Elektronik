using Elektronik.Common.Clouds;
using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamTrackedObject: ICloudItem
    {
        public int id;
        public Color color;
        public Vector3 position;
        public Quaternion rotation;

        public SlamTrackedObject(int id, Color color = default, Vector3 position = default, Quaternion rotation = default)
        {
            this.id = id;
            this.color = color;
            this.position = position;
            this.rotation = rotation;
        }

        public int Id
        {
            get => id;
            set => id = value;
        }
    }
}
