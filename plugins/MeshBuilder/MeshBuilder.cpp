#include "MeshBuilder.h"

#include <pcl/point_types.h>
#include <pcl/kdtree/kdtree_flann.h>
#include <pcl/features/normal_3d.h>
#include <pcl/surface/marching_cubes_hoppe.h>
#include <pcl/filters/statistical_outlier_removal.h>
#include <pcl/filters/radius_outlier_removal.h>
#include <pcl/surface/marching_cubes_rbf.h>
#include <pcl/surface/grid_projection.h>
#include <pcl/surface/poisson.h>

NativeVector GetColor(const std::vector<NativeVector>& colors, const pcl::Indices& indices)
{
    NativeVector color(0, 0, 0);
    for (int i = 0; i < 5; i++) {
        color.x += colors[indices[i]].x;
        color.y += colors[indices[i]].y;
        color.z += colors[indices[i]].z;
    }
    color.x /= 5;
    color.y /= 5;
    color.z /= 5;

    return color;
}

NativeMesh MeshBuilder::FromPoints(const std::vector<NativeVector>& points, const std::vector<NativeVector>& colors)
{
    // Input processing
    pcl::PointCloud<pcl::PointXYZ>::Ptr cloud(new pcl::PointCloud<pcl::PointXYZ>);
    for (const auto& vector: points) {
        cloud->emplace_back(pcl::PointXYZ(vector.x, vector.y, vector.z));
    }

    // Filtering
    pcl::PointCloud<pcl::PointXYZ>::Ptr cloud_filtered(new pcl::PointCloud<pcl::PointXYZ>);

    pcl::StatisticalOutlierRemoval<pcl::PointXYZ> sor;
    sor.setInputCloud(cloud);
    sor.setMeanK(50);
    sor.setStddevMulThresh(1.0);
    sor.filter(*cloud_filtered);

    if (cloud_filtered->size() == 0) return NativeMesh();

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
    reconstructor.setDepth(20);

    // Process output
    pcl::search::KdTree<pcl::PointXYZ>::Ptr tree3(new pcl::search::KdTree<pcl::PointXYZ>);
    tree3->setInputCloud(cloud);
    pcl::Indices k_indices;
    k_indices.resize(5);
    std::vector<float> k_sqr_distances;
    k_sqr_distances.resize(5);

    NativeMesh result;
    for (const auto& point: output_cloud) {
        result.points.emplace_back(NativeVector(point.x, point.y, point.z));
        tree3->nearestKSearch(pcl::PointXYZ(point.x, point.y, point.z), 5, k_indices, k_sqr_distances);
        result.normals.emplace_back(GetColor(colors, k_indices));
    }
    for (const auto& triangle: output_polygons) {
        result.triangles.emplace_back(triangle.vertices[0]);
        result.triangles.emplace_back(triangle.vertices[1]);
        result.triangles.emplace_back(triangle.vertices[2]);
    }
    return result;
}
