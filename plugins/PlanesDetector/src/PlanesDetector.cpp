#include "PlanesDetector.h"

#include "PointCloud.h"
#include "Octree.h"

std::vector<Plane> FilterPlanes(const std::vector<Plane>& planes, const Preferences& preferences)
{
    std::vector<Plane> res;
    for (const auto& plane: planes) {
        auto normal = plane.getNormal();
        auto dot = std::abs(normal * preferences.Gravity) / normal.norm() / preferences.Gravity.norm();
        if (dot < preferences.GravityDCos || dot > 1 - preferences.GravityDCos) res.emplace_back(plane);
    }
    return res;
}

std::vector<std::vector<int>>
PlanesDetector::FindPlanes(const std::vector<Vector3d>& points, const Preferences& preferences) const
{
    auto cloud = PointCloud();
    cloud.loadVector(points);
    auto octree = Octree(cloud, 30);

    auto planes = octree.detectPlanes(preferences.DepthThreshold, preferences.Epsilon,
                                      preferences.NumStartPoints, preferences.NumPoints,
                                      preferences.Steps, preferences.CountRatio, preferences.DCos);
    if (preferences.UseGravity) planes = FilterPlanes(planes, preferences);

    std::vector<std::vector<int>> res(points.size(), std::vector<int>());

    for (int j = 0; j < points.size(); j++) {
        for (int i = 0; i < planes.size(); i++) {
            if (planes[i].accept(points[j])) res[j].emplace_back(i);
        }
    }

    return res;
}

Preferences::Preferences(int depthThreshold, double epsilon, int numStartPoints, int numPoints, int steps,
                         double countRatio, float dCos, const Vector3<double>& gravity, bool useGravity,
                         float gravityDCos)
        : DepthThreshold(depthThreshold), Epsilon(epsilon), NumStartPoints(numStartPoints), NumPoints(numPoints),
          Steps(steps), CountRatio(countRatio), DCos(dCos), Gravity(gravity), UseGravity(useGravity),
          GravityDCos(gravityDCos)
{ }
