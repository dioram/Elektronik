using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamPoint : ICloudItem
    {
        public int id;
        public Vector3 position;
        public Color color;
        public string message;
        public Color defaultColor;

        public SlamPoint(int id, Vector3 position, Color color, Color defaultColor = default, string message = null)
        {
            this.id = id;
            this.position = position;
            this.color = color;
            this.defaultColor = defaultColor;
            this.message = message;
        }

        public SlamPoint(SlamPoint point)
        {
            id = point.id;
            position = point.position;
            color = point.color;
            message = point.message;
            defaultColor = point.defaultColor;
        }

        public override string ToString()
        {
            return message ?? "SlamPoint";
        }

        public int Id
        {
            get => id;
            set => id = value;
        }
    }
}
