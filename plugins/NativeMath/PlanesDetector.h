#pragma once

#include <vector>

#include "NativeStructs.h"
#include "PlaneDetection/Plane.h"

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
    std::vector<int> FindPlanes(const std::vector<Vector3<double>>& points, const Preferences& preferences) const;

private:
    std::vector<Plane> FilterPlanes(const std::vector<Plane>& planes, const Preferences& preferences) const;
};

