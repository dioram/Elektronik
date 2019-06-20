using UnityEngine;

namespace Elektronik.Common.Data
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

        public override int Parse(byte[] data, int startIdx, out IPackage result)
        {
            result = null;
            if ((PackageType)data[startIdx] != PackageType.TrackingPackage)
            {
                return m_successor?.Parse(data, startIdx, out result) ?? 0;
            }
            int readBytes = TrackingPackage.Parse(data, startIdx, out TrackingPackage trackingPackage);
            Convert(ref trackingPackage);
            result = trackingPackage;
            return readBytes;
        }
    }
}
