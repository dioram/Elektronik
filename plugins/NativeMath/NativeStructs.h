#pragma once

#include <vector>
#include "PlaneDetection/Vector3.h"

struct NativeTransform
{
    Vector3<double> position;
    float r11, r12, r13, r21, r22, r23, r31, r32, r33;
};

struct NativeMesh
{
    std::vector<Vector3<double>> points;
    std::vector<int> triangles;
};
