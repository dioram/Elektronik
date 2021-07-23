#pragma once

#include <vector>

#include "NativeStructs.h"

class MeshBuilder
{
public:
    /// Reconstructs mesh from points and observations
    /// \param points
    /// \param view For each point array of observations seeing this point
    /// \param observations
    /// \return
    NativeMesh FromPointsAndObservations(const std::vector<Vector3<double>>& points,
                                         const std::vector<std::vector<int>>& views,
                                         const std::vector<NativeTransform>& observations);
};