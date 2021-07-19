using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.Data.PackageObjects
{
    public struct SlamObservationDiff : ICloudItemDiff<SlamObservation>
    {
        public int Id => Point.Id;
        public SlamPointDiff Point;
        public Quaternion? Rotation;
        [CanBeNull] public int[] ObservedPoints;
        [CanBeNull] public string Message;
        [CanBeNull] public string FileName;
        
        public SlamObservation Apply()
        {
            return new SlamObservation(Point.Apply(),
                                       Rotation ?? Quaternion.identity,
                                       string.IsNullOrEmpty(Message) ? "" : Message,
                                       string.IsNullOrEmpty(FileName) ? "" : FileName,
                                       ObservedPoints ?? new int[0]);
        }

        public SlamObservation Apply(SlamObservation item)
        {
            item.Point = Point.Apply(item.Point);
            item.Rotation = Rotation ?? item.Rotation;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            item.FileName = string.IsNullOrEmpty(FileName) ? item.FileName : FileName;
            if (ObservedPoints != null && ObservedPoints.Length > 0)
            {
                item.ObservedPoints = new HashSet<int>(ObservedPoints);
            }
            return item;
        }
    }
}