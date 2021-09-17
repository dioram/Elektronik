using System;
using System.Collections.Generic;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Plugins.Common.DataDiff
{
    public struct SlamObservationDiff : ICloudItemDiff<SlamObservationDiff, SlamObservation>
    {
        public int Id => Point.Id;
        public SlamPointDiff Point;
        public Quaternion? Rotation;
        public int[]? ObservedPoints;
        public string? Message;
        public string? FileName;

        public SlamObservation Apply()
        {
            return new SlamObservation(Point.Apply(),
                                       Rotation ?? Quaternion.identity,
                                       string.IsNullOrEmpty(Message) ? "" : Message,
                                       string.IsNullOrEmpty(FileName) ? "" : FileName,
                                       ObservedPoints ?? Array.Empty<int>());
        }

        public SlamObservation Apply(SlamObservation item)
        {
            item.Point = Point.Apply(item.Point);
            item.Rotation = Rotation ?? item.Rotation;
            item.Message = string.IsNullOrEmpty(Message) ? item.Message : Message;
            item.FileName = string.IsNullOrEmpty(FileName) ? item.FileName : FileName;
            if (ObservedPoints is { Length: > 0 })
            {
                item.ObservedPoints = new HashSet<int>(ObservedPoints);
            }

            return item;
        }

        public SlamObservationDiff Apply(SlamObservationDiff right)
        {
            if (Id != right.Id) throw new Exception("Ids must be identical!");
            return new SlamObservationDiff
            {
                Point = Point.Apply(right.Point),
                Rotation = right.Rotation ?? Rotation,
                Message = string.IsNullOrEmpty(right.Message) ? Message : right.Message,
                FileName = string.IsNullOrEmpty(right.FileName) ? FileName : right.FileName,
                ObservedPoints = (right.ObservedPoints is null || right.ObservedPoints.Length == 0)
                        ? ObservedPoints
                        : right.ObservedPoints,
            };
        }
    }
}