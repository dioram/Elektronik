%module PlanesDetector
%{
#include "include/Plane.h"
#include "include/PointCloud.h"
#include "include/Octree.h"
%};
%include "stdint.i"
%include "std_string.i"
%include "std_vector.i"
%include "std_shared_ptr.i"
%include "PointCloud.h"
%include "Octree.h"
%include "Plane.h"
%include "Vec3.h"
%template(Vec3d) Vec3<double>;
namespace std {
        %template(PlanesList) vector<Plane>;
        %template(PointsList) vector<Vec3d>;
};