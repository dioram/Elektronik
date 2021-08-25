using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SlamPointDiff : ICloudItemDiff<SlamPoint>
    {
        public int Id { get; set; }
        public Vector3? Position;
        public Color? Color;
        [CanBeNull] public string Message;
        
        public SlamPoint Apply()
        {
            return new SlamPoint(Id, 
                                 Position ?? Vector3.zero, 
                                 Color ?? UnityEngine.Color.black,
                                 string.IsNullOrEmpty(Message) ? "" : Message);
        }

        public SlamPoint Apply(SlamPoint item)
        {
            item.Position = Position ?? item.Position;
            item.Color = Color ?? item.Color;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            return item;
        }
    }
}