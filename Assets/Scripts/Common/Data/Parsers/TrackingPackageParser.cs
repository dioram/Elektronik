using UnityEngine;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Converters;

namespace Elektronik.Common.Data.Parsers
{
    public class TrackingPackageParser : DataParser
    {
        public TrackingPackageParser(ICSConverter converter) : base(converter) { }

        private void Convert(ref TrackingPackage pkg)
        {
            for (int trackIdx = 0; trackIdx < pkg.Tracks.Count; ++trackIdx)
            {
                Vector3 trackPos = pkg.Tracks[trackIdx].position;
                Quaternion trackRot = pkg.Tracks[trackIdx].rotation;
                m_converter.Convert(ref trackPos, ref trackRot);
                var unit = new TrackingPackage.TrackingUnit()
                {
                    position = trackPos,
                    rotation = trackRot,
                    id = pkg.Tracks[trackIdx].id,
                    color = pkg.Tracks[trackIdx].color,
                };
                pkg.Tracks[trackIdx] = unit;
            }
        }

        public override IPackage Parse(byte[] data, int startIdx, ref int offset)
        {
            if ((PackageType)data[startIdx] != PackageType.TrackingPackage)
            {
                return m_successor?.Parse(data, startIdx, ref offset);
            }
            int readBytes = TrackingPackage.Parse(data, startIdx, out TrackingPackage trackingPackage);
            offset += readBytes;
            Convert(ref trackingPackage);
            return trackingPackage;
        }
    }
}
