#pragma once

#include <vector>

struct NativeVector
{
    float x, y, z;

    NativeVector() = default;

    NativeVector(float x, float y, float z)
    {
        this->x = x;
        this->y = y;
        this->z = z;
    }
};

struct NativeTransform
{
    NativeVector position;
    // Rotation matrix indices
    float r11, r12, r13, r21, r22, r23, r31, r32, r33;
};

struct NativeMesh
{
    std::vector<NativeVector> points;
    std::vector<NativeVector> normals;
    std::vector<int> triangles;
};

class MeshBuilder
{
public:

    NativeMesh FromPoints(const std::vector<NativeVector>& points, const std::vector<NativeVector>& colors);
};