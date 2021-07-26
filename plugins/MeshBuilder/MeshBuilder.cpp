#include "MeshBuilder.h"

#include <OpenMVS/MVS.h>

using namespace MVS;

NativeMesh MeshBuilder::FromPointsAndObservations(const std::vector<NativeVector>& points,
                                                  const std::vector<std::vector<int>>& views,
                                                  const std::vector<NativeTransform>& observations)
{
    // Input processing
    Scene scene(4);
    scene.pointcloud.points.reserve(points.size());
    for (const auto& point: points) {
        scene.pointcloud.points.emplace_back(PointCloud::Point(point.x, point.y, point.z));
    }

    scene.pointcloud.pointViews.reserve(views.size());
    for (const auto& v: views) {
        scene.pointcloud.pointViews.emplace_back();
        for (const auto& v1: v) {
            scene.pointcloud.pointViews.back().emplace_back((uint32_t) v1);
        }
    }

    for (const auto& obs: observations) {
        Image image;
        Point3 c(obs.position.x, obs.position.y, obs.position.z);
        Matrix3x3 r(obs.r11, obs.r12, obs.r13, obs.r21, obs.r22, obs.r23, obs.r31, obs.r32, obs.r33);
        Camera camera(r, c);
        image.camera = camera;
        scene.images.emplace_back(image);
    }

    // Reconstructing
    scene.ReconstructMesh();

    // Output processing
    NativeMesh result;
    for (const auto& v: scene.mesh.vertices) {
        result.points.emplace_back(NativeVector(v.x, v.y, v.z));
    }
    for (const auto& v: scene.mesh.vertexNormals) {
        result.normals.emplace_back(NativeVector(v.x, v.y, v.z));
    }
    for (const auto& f: scene.mesh.faces) {
        result.triangles.emplace_back(f.x);
        result.triangles.emplace_back(f.y);
        result.triangles.emplace_back(f.z);
    }

    return result;
}
