#pragma once

#include "Point.h"
#include "RGB.h"
#include "UnionFind.h"
#include <vector>
#include <iostream>
#include <opencv2/core/core.hpp>

class Plane
{
    // Print the plane.
    friend std::ostream& operator<<(std::ostream& os, const Plane& p);

public:
    // Invalid plane.
    Plane();

    // Plane that best fit the points with least squares minimization.
    Plane(const std::vector<SharedPoint>& pts);

    // Distance between point and plane.
    double distance(SharedPoint p);

    double distance(Point p);

    // Square of distance between point and plane.
    double squareDistance(SharedPoint p);

    // Whether the point is close to the plane.
    bool accept(SharedPoint p);

    bool accept(Vector3d p);

    // Fit the plane to the points.
    void setPoints(const std::vector<SharedPoint>& pts);

    // Change the color of the plane.
    void setColor(RGB color, UnionFindPlanes& colors);

    // Reset all points to their initial color.
    void destroy(UnionFindPlanes& colors);

    // Ajoute un point au plan (sans recalculer l'equation)
    void addPoint(SharedPoint p, UnionFindPlanes& colors);

    // Compute equation and attributes of the plane (radius, thickness)
    void computeEquation();

    // Decides whether p is mergeable with this.
    bool mergeableWith(const Plane& p, double dCos) const;

    // Merge plane p into this.
    void merge(Plane& p, UnionFindPlanes& colors);

    Vector3d getNormal() const
    { return normal; }

    Vector3d getCenter() const
    { return center; }

    inline unsigned int getCount()
    { return count; }

private:
    // Initialize plane attributes.
    void init();

    // Add a point to the plane (without recomputing equation).
    void addPoint(const Point& p);

    // Best fit of the plane using least squares.
    void leastSquares();

    // Distance betwen u and v along the normal.
    double distanceAlong(Vector3d u, Vector3d v) const;

    // Angular difference between normals.
    double getCos(const Plane& p) const;

    // Plane equation : normal * X + d = 0
    Vector3d normal;
    double d;
    // Matrix to compute quickly the optimal equation.
    cv::Mat m;
    Vector3d sum;
    // Attributes for plane merging.
    Vector3d center;
    double radius;
    double thickness;

    // One point related to the plane
    SharedPoint point;
    // Number of points
    unsigned int count;
};

typedef std::shared_ptr<Plane> SharedPlane;
