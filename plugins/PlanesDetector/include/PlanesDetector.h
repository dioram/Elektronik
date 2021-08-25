#pragma once

#include <vector>

#include "Vector3.h"

class Preferences
{
public:
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

