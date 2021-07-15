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
    /// Reconstructs mesh from points and observations
    /// \param points
    /// \param view For each point array of observations seeing this point
    /// \param observations
    /// \return
    NativeMesh FromPointsAndObservations(const std::vector<NativeVector>& points,
                                         const std::vector<std::vector<int>>& views,
                                         const std::vector<NativeTransform>& observations);
};