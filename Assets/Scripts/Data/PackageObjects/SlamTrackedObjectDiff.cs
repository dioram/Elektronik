using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SlamTrackedObjectDiff : ICloudItemDiff<SlamTrackedObjectDiff, SlamTrackedObject>
    {
        public int Id { get; set; }
        public Color? Color;
        public Vector3? Position;
        public Quaternion? Rotation;
        [CanBeNull] public string Message;

        public SlamTrackedObject Apply()
        {
            return new SlamTrackedObject(Id,
                                         Position ?? Vector3.zero,
                                         Rotation ?? Quaternion.identity,
                                         Color ?? UnityEngine.Color.black,
                                         string.IsNullOrEmpty(Message) ? "" : Message);
        }

        public SlamTrackedObject Apply(SlamTrackedObject item)
        {
            item.Position = Position ?? item.Position;
            item.Rotation = Rotation ?? item.Rotation;
            item.Color = Color ?? item.Color;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            return item;
        }

        public SlamTrackedObjectDiff Apply(SlamTrackedObjectDiff right)
        {
            if (Id != right.Id) throw new Exception("Ids must be identical!");
            return new SlamTrackedObjectDiff
            {
                Id = Id,
                Position = right.Position ?? Position,
                Rotation = right.Rotation ?? Rotation,
                Color = right.Color ?? Color,
                Message = string.IsNullOrEmpty(right.Message) ? Message : right.Message,
            };
        }
    }
}