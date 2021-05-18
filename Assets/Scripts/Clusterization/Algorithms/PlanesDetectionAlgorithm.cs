using System.Collections.Generic;
using System.Linq;
using Elektronik.Clusterization.Algorithms.PlanesDetectionNative;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clusterization.Algorithms
{
    public class PlanesDetectionAlgorithm : IClusterizationAlgorithm
    {
        public struct Settings
        {
            public int DepthThreshold;
            public double Epsilon;
            public int NumStartPoints;
            public int NumPoints;
            public int Steps;
            public double CountRatio;
            public float DCos;
            public Vector3 Gravity;
            public bool UseGravity;
            public float GravityDCos;
        }

        public PlanesDetectionAlgorithm(Settings settings)
        {
            _settings = settings;
        }

        private readonly Settings _settings;


        public List<List<SlamPoint>> Compute(IList<SlamPoint> items)
        {
            using var cloud = new PointCloud();
            cloud.loadVector(new PointsList(items.Select(Convert)));

            using var octree = new Octree(cloud, 30);
            using var res = octree.detectPlanes(_settings.DepthThreshold, _settings.Epsilon,
                                                _settings.NumStartPoints, _settings.NumPoints,
                                                _settings.Steps, _settings.CountRatio, _settings.DCos);

            return res
                    .Where(plane => !_settings.UseGravity
                                   || FilterPlanes(Convert(plane.getNormal()), _settings.Gravity, _settings.GravityDCos))
                    .Select(plane => items.Where(point => plane.accept(Convert(point))).ToList())
                    .ToList();
        }

        private static Vec3d Convert(SlamPoint point)
        {
            return new Vec3d(point.Position.x, point.Position.y, point.Position.z);
        }

        private static Vector3 Convert(Vec3d vector)
        {
            return new Vector3((float) vector.x, (float) vector.y, (float) vector.z);
        }

        private static bool FilterPlanes(Vector3 normal, Vector3 gravity, float maxDCos)
        {
            var dot = Mathf.Abs(Vector3.Dot(normal, gravity));
            return dot < maxDCos || dot > 1 - maxDCos;
        }
    }
}