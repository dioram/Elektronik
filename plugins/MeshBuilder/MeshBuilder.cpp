#include "MeshBuilder.h"

#include <pcl/point_types.h>
#include <pcl/kdtree/kdtree_flann.h>
#include <pcl/features/normal_3d.h>
#include <pcl/filters/statistical_outlier_removal.h>
#include <pcl/surface/poisson.h>
#include <set>

#define NEIGHBORS_FOR_COLOR 5

void GetColor(const std::vector<NativePoint>& points, const std::vector<int>& indices, NativePoint& point)
{
    float r = 0, g = 0, b = 0;
    for (int index: indices) {
        r += points[index].r;
        g += points[index].g;
        b += points[index].b;
    }
    point.r = r / 5;
    point.g = g / 5;
    point.b = b / 5;
}

template<typename T>
std::pair<std::vector<int>, std::vector<float>>
GetKNearest(const pcl::search::KdTree<T>& tree, int k, T point)
{
    pcl::Indices k_indices;
    k_indices.resize(k);
    std::vector<float> k_sqr_distances;
    k_sqr_distances.resize(k);
    tree.nearestKSearch(point, k, k_indices, k_sqr_distances);
    return std::make_pair(k_indices, k_sqr_distances);
}

NativeMesh MeshBuilder::FromPoints(const std::vector<NativePoint>& points)
{
    // Input processing
    pcl::PointCloud<pcl::PointXYZ>::Ptr cloud(new pcl::PointCloud<pcl::PointXYZ>);
    for (const auto& vector: points) {
        cloud->emplace_back(pcl::PointXYZ(vector.x, vector.y, vector.z));
    }
    pcl::search::KdTree<pcl::PointXYZ>::Ptr tree3(new pcl::search::KdTree<pcl::PointXYZ>);
    tree3->setInputCloud(cloud);

    // Filtering
    pcl::PointCloud<pcl::PointXYZ>::Ptr cloud_filtered(new pcl::PointCloud<pcl::PointXYZ>);

    pcl::StatisticalOutlierRemoval<pcl::PointXYZ> sor;
    sor.setInputCloud(cloud);
    sor.setMeanK(50);
    sor.setStddevMulThresh(1.0);
    sor.filter(*cloud_filtered);

    if (cloud_filtered->size() == 0) return {};

    // Normals estimation
    pcl::NormalEstimation<pcl::PointXYZ, pcl::Normal> n;
    pcl::PointCloud<pcl::Normal>::Ptr normals(new pcl::PointCloud<pcl::Normal>);
    pcl::search::KdTree<pcl::PointXYZ>::Ptr tree(new pcl::search::KdTree<pcl::PointXYZ>);
    tree->setInputCloud(cloud_filtered);
    n.setInputCloud(cloud_filtered);
    n.setSearchMethod(tree);
    n.setKSearch(20);
    n.compute(*normals);

    pcl::PointCloud<pcl::PointNormal>::Ptr cloud_with_normals(new pcl::PointCloud<pcl::PointNormal>);
    pcl::concatenateFields(*cloud_filtered, *normals, *cloud_with_normals);

    // Setting up reconstructor
    pcl::search::KdTree<pcl::PointNormal>::Ptr tree2(new pcl::search::KdTree<pcl::PointNormal>);
    tree2->setInputCloud(cloud_with_normals);

    pcl::Poisson<pcl::PointNormal> reconstructor;
    pcl::PointCloud<pcl::PointNormal> output_cloud;
    std::vector<pcl::Vertices> output_polygons;

    reconstructor.setInputCloud(cloud_with_normals);
    reconstructor.setSearchMethod(tree2);

    // Compute
    reconstructor.performReconstruction(output_cloud, output_polygons);

    // Process output
    std::set<int> excluded_points;

    NativeMesh result;
    for (int i = 0; i < output_cloud.size(); i++) {
        auto p = pcl::PointXYZ(output_cloud[i].x, output_cloud[i].y, output_cloud[i].z);
        auto distance = GetKNearest(*tree, 1, p).second.front();
        NativePoint point(p.x, p.y, p.z);
        if (distance > 1) {
            excluded_points.emplace(i);
        } else {
            GetColor(points, GetKNearest(*tree3, NEIGHBORS_FOR_COLOR, p).first, point);
        }
        result.points.emplace_back(point);
    }

    for (const auto& triangle: output_polygons) {
        auto a = triangle.vertices[0];
        auto b = triangle.vertices[1];
        auto c = triangle.vertices[2];
        if (excluded_points.count(a) > 0 || excluded_points.count(b) > 0 || excluded_points.count(c) > 0) continue;

        result.triangles.emplace_back(a);
        result.triangles.emplace_back(b);
        result.triangles.emplace_back(c);
    }
    return result;
}
