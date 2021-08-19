#pragma once

#include <vector>

struct NativePoint
{
    float x, y, z, r, g, b;
    NativePoint() = default;

    NativePoint(float x, float y, float z) : NativePoint(x, y, z, 0, 0, 0)
    { }

    NativePoint(float x, float y, float z, float r, float g, float b) : x(x), y(y), z(z), r(r), g(g), b(b)
    { }
};

struct NativeMesh
{
    std::vector<NativePoint> points;
    std::vector<int> triangles;
};

class MeshBuilder
{
public:
    NativeMesh FromPoints(const std::vector<NativePoint>& points);
};