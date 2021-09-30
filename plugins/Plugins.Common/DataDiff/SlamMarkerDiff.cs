using System;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Plugins.Common.DataDiff
{
    public struct SlamMarkerDiff : ICloudItemDiff<SlamMarkerDiff, SlamMarker>
    {
        public int Id { get; set; }
        public Vector3? Position;
        public Quaternion? Rotation;
        public Vector3? Scale;
        public Color? Color;
        public string? Message;
        public SlamMarker.MarkerType? Type;

        public SlamMarker Apply()
        {
            return new SlamMarker(Id, 
                                  Position ?? Vector3.zero, 
                                  Rotation ?? Quaternion.identity, 
                                  Scale ?? Vector3.one,
                                  Color ?? UnityEngine.Color.black, 
                                  string.IsNullOrEmpty(Message) ? "" : Message, 
                                  Type ?? SlamMarker.MarkerType.Sphere);
        }

        public SlamMarker Apply(SlamMarker item)
        {
            item.Position = Position ?? item.Position;
            item.Rotation = Rotation ?? item.Rotation;
            item.Scale = Scale ?? item.Scale;
            item.Color = Color ?? item.Color;
            item.Type = Type ?? item.Type;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            return item;
        }

        public SlamMarkerDiff Apply(SlamMarkerDiff item)
        {
            if (Id != item.Id) throw new Exception("Ids must be identical!");
            return new SlamMarkerDiff
            {
                Id = Id,
                Position = item.Position ?? Position,
                Rotation = item.Rotation ?? Rotation,
                Scale = item.Scale ?? Scale,
                Color = item.Color ?? Color,
                Type = item.Type ?? Type,
                Message = string.IsNullOrEmpty(item.Message) ? Message : item.Message,
            };
        }
    }
}