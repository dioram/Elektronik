using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public class SlamPackageParser : DataParser
    {
        public SlamPackageParser(ICSConverter converter) : base(converter) { }

        private void Convert(ref SlamPackage pkg)
        {
            List<SlamObservation> observations = pkg.Observations;
            Parallel.For(0, pkg.Observations.Count, (i) =>
            {
                SlamPoint point = observations[i];
                Quaternion rot = observations[i].Orientation;
                m_converter.Convert(ref point.position, ref rot);
                observations[i].Point = point;
                observations[i].Orientation = rot;
            });
            List<SlamPoint> points = pkg.Points;
            Parallel.For(0, pkg.Points.Count, (i) =>
            {
                SlamPoint pt = points[i];
                Quaternion stub = Quaternion.identity;
                m_converter.Convert(ref pt.position, ref stub);
                points[i] = pt;
            });
        }

        public override int Parse(byte[] data, int startIdx, out IPackage result)
        {
            result = null;
            if ((PackageType)data[startIdx] != PackageType.SLAMPackage)
                return m_successor?.Parse(data, startIdx, out result) ?? 0;
            int readBytes = SlamPackage.Parse(data, startIdx, out SlamPackage slamPkg);
            Convert(ref slamPkg);
            result = slamPkg;
            return readBytes;
        }
    }
}
