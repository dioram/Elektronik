// #if !NO_PLANES_DETECTION
// using System.Collections.Generic;
// using System.Linq;
// using Elektronik.Clusterization.Algorithms.PlanesDetectionNative;
// using Elektronik.Data.PackageObjects;
//
// namespace Elektronik.Clusterization.Algorithms
// {
//     public class PlanesDetectionAlgorithm : IClusterizationAlgorithm
//     {
//         public PlanesDetectionAlgorithm(Preferences settings)
//         {
//             _settings = settings;
//         }
//
//         public List<List<SlamPoint>> Compute(IList<SlamPoint> items)
//         {
//             var detector = new PlanesDetector();
//             var indexes = detector.FindPlanes(new PointsList(items.Select(ToNative)), _settings);
//             return items.Zip(indexes, (point, i) => (point, i))
//                     .Where(z => z.i != 0)
//                     .GroupBy(v => v.i)
//                     .Select(g => g.Select(v => v.point).ToList())
//                     .ToList();
//         }
//
//         #region Private
//
//         private readonly Preferences _settings;
//
//         private static Vector3d ToNative(SlamPoint point) =>
//                 new Vector3d(point.Position.x, point.Position.y, point.Position.z);
//
//         #endregion
//     }
// }
// #endif