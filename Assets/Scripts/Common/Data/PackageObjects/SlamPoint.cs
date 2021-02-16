using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamPoint : ICloudItem
    {
        public int Id { get; set; }
        public Vector3 position;
        public Color color;
        public string message;
        public Color defaultColor;

        public SlamPoint(int id, Vector3 position, Color color, Color defaultColor = default, string message = null)
        {
            Id = id;
            this.position = position;
            this.color = color;
            this.defaultColor = defaultColor;
            this.message = message;
        }

        public SlamPoint(SlamPoint point)
        {
            Id = point.Id;
            position = point.position;
            color = point.color;
            message = point.message;
            defaultColor = point.defaultColor;
        }

        public override string ToString()
        {
            return message ?? "SlamPoint";
        }
    }
}
