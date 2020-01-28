using Elektronik.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Data.Packages
{
    public class TrackingPackage : IPackage
    {
        public struct TrackingUnit
        {
            public Vector3 position;
            public Quaternion rotation;
            public int id;
            public Color color;
        }
        public PackageType PackageType { get => PackageType.TrackingPackage; }
        public int Timestamp { get; private set; }
        public bool IsKey { get; private set; }
        public IList<TrackingUnit> Tracks { get; private set; }
        private TrackingPackage() =>
            Tracks = new List<TrackingUnit>();
        public static int Parse(byte[] data, int startIdx, out TrackingPackage result)
        {
            result = null;
            int offset = startIdx;
            PackageType type = (PackageType)data[offset++];
            if (type != PackageType.TrackingPackage)
                return 0;
            result = new TrackingPackage();
            result.Timestamp = BitConverterEx.ToInt32(data, offset, ref offset);
            result.IsKey = BitConverterEx.ToBoolean(data, offset, ref offset);
            int countOfTracks = BitConverterEx.ToInt32(data, offset, ref offset);
            for (int i = 0; i < countOfTracks; ++i)
            {
                var unit = new TrackingUnit()
                {
                    id = BitConverterEx.ToInt32(data, offset, ref offset),
                    position = BitConverterEx.ToVector3(data, offset, ref offset),
                    rotation = BitConverterEx.ToQuaternion(data, offset, ref offset),
                    color = BitConverterEx.ToRGBColor(data, offset, ref offset),
                };
                result.Tracks.Add(unit);
            }
            return offset - startIdx;
        }
    }
}
