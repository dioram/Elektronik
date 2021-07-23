#include "PlanesDetector.h"

#include "PlaneDetection/PointCloud.h"
#include "PlaneDetection/Octree.h"

std::vector<Plane> PlanesDetector::FilterPlanes(const std::vector<Plane>& planes, const Preferences& preferences) const
{
    std::vector<Plane> res;
    for (const auto& plane: planes) {
        auto dot = std::abs(plane.getNormal() * preferences.Gravity);
        if (dot < preferences.GravityDCos || dot > 1 - preferences.GravityDCos) res.emplace_back(plane);
    }
    return res;
}

std::vector<int>
PlanesDetector::FindPlanes(const std::vector<Vector3<double>>& points, const Preferences& preferences) const
{
    auto cloud = PointCloud();
    cloud.loadVector(points);
    auto octree = Octree(cloud, 30);

    auto planes = octree.detectPlanes(preferences.DepthThreshold, preferences.Epsilon,
                                      preferences.NumStartPoints, preferences.NumPoints,
                                      preferences.Steps, preferences.CountRatio, preferences.DCos);
    if (preferences.UseGravity) planes = FilterPlanes(planes, preferences);

    std::vector<int> res(points.size(), 0);

    for (int j = 0; j < points.size(); j++) {
        for (int i = 0; i < planes.size(); i++) {
            if (planes[i].accept(points[j])) res[j] = i + 1;
        }
    }

    return res;
}