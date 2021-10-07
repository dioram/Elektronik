using System.Collections.Generic;
using System.Linq;
using Elektronik.Clustering.PlanesDetection.Native;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;

namespace Elektronik.Clustering.PlanesDetection
{
    public class PlanesDetectionAlgorithm : ClusteringAlgorithmBase<PlanesDetectionSettings>
    {
        public PlanesDetectionAlgorithm(PlanesDetectionSettings typedSettings, string displayName) : base(typedSettings)
        {
            DisplayName = displayName;
        }

        protected override IList<IList<SlamPoint>> Compute(IList<SlamPoint> points, PlanesDetectionSettings settings)
        {
            var detector = new PlanesDetector();
            var clustered = detector.FindPlanes(new PointsList(points.Select(ToNative)), settings.ToPrefs());
            var res = new List<IList<SlamPoint>>();
            for (var i = 0; i < clustered.Count; i++)
            {
                foreach (var plane in clustered[i])
                {
                    if (res.Count <= plane)
                    {
                        res.AddRange(Enumerable.Range(0, plane + 1 - res.Count).Select(_ => new List<SlamPoint>()));
                    }

                    res[plane].Add(points[i]);
                }
            }

            return res;
        }

        public override string DisplayName { get; }

        private static Vector3d ToNative(SlamPoint point) =>
                new Vector3d(point.Position.x, point.Position.y, point.Position.z);
    }
}