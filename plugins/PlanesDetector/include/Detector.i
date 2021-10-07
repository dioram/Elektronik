%module PlanesDetection
%{
#include "include/PlanesDetector.h"
%};
%include "stdint.i"
%include "std_vector.i"
%include "Vector3.h"
%include "PlanesDetector.h"
%template(Vector3d) Vector3<double>;
namespace std {
        %template(ClustersList) vector<vector<int>>;
        %template(vectori) vector<int>;
        %template(PointsList) vector<Vector3d>;
};