#include "../include/PointCloud.h"
#include "../include/Octree.h"

float randF()
{
    return static_cast <float> (std::rand()) / static_cast <float> (RAND_MAX);
}

int main()
{
    auto pc = new PointCloud();
    auto points = std::vector<Point>();
    points.reserve(3000);
    for (int i = 0; i < 3000; i++)
    {
        points.emplace_back(randF(), randF(), randF());
    }

    pc->loadVector(points);

    auto octree = new Octree(*pc, 30);
    auto res = octree->detectPlanes(100, 0.05, 10, 30, 10, 0.005, 3.14/180 * 15);

    std::cout << res.size() << std::endl;

    delete pc;
    delete octree;
}