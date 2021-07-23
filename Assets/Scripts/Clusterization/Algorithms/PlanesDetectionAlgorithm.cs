using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.Mesh;
using Elektronik.NativeMath;

namespace Elektronik.Clusterization.Algorithms
{
    public class PlanesDetectionAlgorithm : IClusterizationAlgorithm
    {
        public PlanesDetectionAlgorithm(Preferences settings)
        {
            _settings = settings;
        }

        private readonly Preferences _settings;

        public List<List<SlamPoint>> Compute(IList<SlamPoint> items)
        {
            var detector = new PlanesDetector();
            var indexes = detector.FindPlanes(new vectorv(items.Select(i => i.ToNative())), _settings);
            return items.Zip(indexes, (point, i) => (point, i))
                    .GroupBy(v => v.i)
                    .Select(g => g.Select(v => v.point).ToList())
                    .ToList();
        }
    }
}