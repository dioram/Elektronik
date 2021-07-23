%module Math
%{
#include "NativeStructs.h"
#include "MeshBuilder.h"
#include "PlanesDetector.h"
#include "PlaneDetection/Vector3.h"
%}
%include "stdint.i"
%include "std_vector.i"
%include "std_string.i"
%include "std_shared_ptr.i"
%include "PlaneDetection/Vector3.h"
%include "NativeStructs.h"
%include "MeshBuilder.h"
%include "PlanesDetector.h"
%template(NativeVector) Vector3<double>;
namespace std {
        %template(vectori) vector<int>;
        %template(vectori2d) vector<vector<int>>;
        %template(vectorv) vector<Vector3<double>>;
        %template(vectort) vector<NativeTransform>;
};
