#pragma once

#include <vector>

#include "Vector3.h"

class Preferences
{
public:
    Preferences(int depthThreshold, double epsilon, int numStartPoints, int numPoints, int steps, double countRatio,
                float dCos, const Vector3<double>& gravity, bool useGravity, float gravityDCos);

    int DepthThreshold;
    double Epsilon;
    int NumStartPoints;
    int NumPoints;
    int Steps;
    double CountRatio;
    float DCos;
    Vector3<double> Gravity;
    bool UseGravity;
    float GravityDCos;
};

class PlanesDetector
{
public:
    std::vector<int> FindPlanes(const std::vector<Vector3d>& points, const Preferences& preferences) const;
};

