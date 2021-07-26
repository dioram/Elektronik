%module Builder
%{
#include "MeshBuilder.h"
%}
%include "stdint.i"
%include "std_vector.i"
namespace std {
        %template(vectori) vector<int>;
        %template(vectori2d) vector<vector<int>>;
        %template(vectorv) vector<NativeVector>;
        %template(vectort) vector<NativeTransform>;
};
%include "MeshBuilder.h"
