using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamPoint : ICloudItem
    {
        public int Id { get; set; }
        public Vector3 Position;
        public Color Color;
        public string Message { get; set; }

        public SlamPoint(int id, Vector3 position, Color color, string message = null)
        {
            Id = id;
            Position = position;
            Color = color;
            Message = message;
        }

        public SlamPoint(SlamPoint point)
        {
            Id = point.Id;
            Position = point.Position;
            Color = point.Color;
            Message = point.Message;
        }

        public override string ToString()
        {
            return Message ?? "SlamPoint";
        }
    }
}
