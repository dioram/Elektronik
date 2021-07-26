using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.Mesh;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Elektronik.NativeMath;
#endif

namespace Elektronik.Clusterization.Algorithms
{
    public class PlanesDetectionAlgorithm : IClusterizationAlgorithm
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        public PlanesDetectionAlgorithm(Preferences settings)
        {
            _settings = settings;
        }
        private readonly Preferences _settings;

#else
        public PlanesDetectionAlgorithm()
        {
        }
#endif

        public List<List<SlamPoint>> Compute(IList<SlamPoint> items)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            var detector = new PlanesDetector();
            var indexes = detector.FindPlanes(new vectorv(items.Select(i => i.ToNative())), _settings);
            return items.Zip(indexes, (point, i) => (point, i))
                .GroupBy(v => v.i)
                .Select(g => g.Select(v => v.point).ToList())
                .ToList();
#else
            return new List<List<SlamPoint>>();
#endif
        }
    }
}