using Elektronik.Containers;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SlamPoint : ICloudItem, ILookable
    {
        public int Id { get; set; }
        public Vector3 Position;
        public Color Color;
        public string Message { get; set; }
        public SlamPoint AsPoint() => this;

        public SlamPoint(int id, Vector3 position, Color color, string message = null)
        {
            Id = id;
            Position = position;
            Color = color;
            Message = message;
        }

        public override string ToString()
        {
            return Message ?? "SlamPoint";
        }

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            return (Position + (transform.position - Position).normalized,
                    Quaternion.LookRotation(Position - transform.position));
        }
    }
}