#ifndef Octree_H
#define Octree_H

#include <vector>
#include <random>
#include "PointCloud.h"

// Octree structure.
class Octree {
public:
    // Build a tree from a point cloud.
    Octree(PointCloud& cloud, unsigned int maxdepth);

    std::vector<Plane> detectPlanes(int depthThreshold, double epsilon, int numStartPoints, int numPoints, int steps, double countRatio, double dCos) const
    {
        auto planes = std::vector<SharedPlane>();
        std::default_random_engine random;
        random.seed(std::chrono::high_resolution_clock::now().time_since_epoch().count());
        detectPlanes(depthThreshold, epsilon, numStartPoints, numPoints, steps, countRatio, random, planes, mCloud.colors(), dCos);
        auto res = std::vector<Plane>();
        res.reserve(planes.size());
        for(auto p : planes)
        {
            res.emplace_back(*p);
        }
        return res;
    }

    // Detect planes in the point cloud.
    void detectPlanes(int depthThreshold, double epsilon, int numStartPoints, int numPoints, int steps, double countRatio, std::default_random_engine& generator, std::vector<SharedPlane>& planes, UnionFindPlanes& colors, double dCos) const;

private:
    // Node of the tree structure.
    class Node {
    public:
        // Create a node.
        Node(const Vec3d& origin, const Vec3d& halfDimension);

        // Insert a point with max recursion depth. Return false if max depth reached, true otherwise.
        bool insert(SharedPoint p, unsigned int maxdepth);
        // Detect planes in this subtree.
        void detectPlanes(int depthThreshold, double epsilon, int numStartPoints, int numPoints, int steps, double countRatio, std::default_random_engine& generator, std::vector<SharedPlane>& planes, UnionFindPlanes& colors, double dCos, std::vector<SharedPoint>& pts) const;

        // Remove planes that have too few points, according to countRatio.
        static void removeSmallPlanes(std::vector<SharedPlane>& planes, double countRatio, UnionFind<SharedPoint, std::pair<RGB, bool>>& colors);

    private:
        // Collect all the points in this subtree.
        void getPoints(std::vector<SharedPoint>& pts) const;
        // Whether this is a leaf.
        bool isLeafNode() const;
        // Determine which octant of the tree contains p.
        int findOctant(SharedPoint p) const;

        // Physical position/size.
        Vec3d center;
        Vec3d halfSize;

        /*
         *  Children follow a predictable pattern to make accesses simple.
         *  Here, - means less than 'center' in that dimension, + means greater than.
         *  child:	0 1 2 3 4 5 6 7
         *  x:      - - - - + + + +
         *  y:      - - + + - - + +
         *  z:      - + - + - + - +
        */
        std::shared_ptr<Node> children[8];
        // Point for leaves only.
        SharedPoint point;
        // Number of points inside this Node.
        unsigned int count;
    };

    // Root of the tree.
    Node mRoot;

    PointCloud& mCloud;
};

#endif
