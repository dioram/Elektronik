using System.Collections.Generic;
using System.Linq;
using Elektronik.Clusterization.Algorithms.PlanesDetectionNative;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;

namespace Clusterization.PlanesDetection
{
    public class PlanesDetectionAlgorithm : ClusterizationAlgorithmBase<PlanesDetectionSettings>
    {
        public PlanesDetectionAlgorithm(PlanesDetectionSettings typedSettings, string displayName) : base(typedSettings)
        {
            DisplayName = displayName;
        }

        protected override IList<IList<SlamPoint>> Compute(IList<SlamPoint> points, PlanesDetectionSettings settings)
        {
            var detector = new PlanesDetector();
            var indexes = detector.FindPlanes(new PointsList(points.Select(ToNative)), settings.ToPrefs());
            return points.Zip(indexes, (point, i) => (point, i))
                .Where(z => z.i != 0)
                .GroupBy(v => v.i)
                .Select(g => (IList<SlamPoint>)g.Select(v => v.point).ToList())
                .ToList();
        }

        public override string DisplayName { get; }

        private static Vector3d ToNative(SlamPoint point) =>
            new Vector3d(point.Position.x, point.Position.y, point.Position.z);
    }
}