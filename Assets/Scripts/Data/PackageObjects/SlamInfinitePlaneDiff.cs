using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SlamInfinitePlaneDiff : ICloudItemDiff<SlamInfinitePlane>
    {
        public int Id { get; set; }
        public Vector3? Offset;
        public Vector3? Normal;
        public Color? Color;
        [CanBeNull] public string Message;

        public SlamInfinitePlane Apply()
        {
            return new SlamInfinitePlane(Id,
                                         Offset ?? Vector3.zero,
                                         Normal ?? Vector3.zero,
                                         Color ?? UnityEngine.Color.black,
                                         string.IsNullOrEmpty(Message) ? "" : Message);
        }

        public SlamInfinitePlane Apply(SlamInfinitePlane item)
        {
            item.Offset = Offset ?? item.Offset;
            item.Normal = Normal ?? item.Normal;
            item.Color = Color ?? item.Color;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            return item;
        }
    }
}