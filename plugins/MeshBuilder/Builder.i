%module Builder
%{
#include "MeshBuilder.h" 
%}
%typemap(cstype) const std::vector<NativePoint> & "Elektronik.Data.PackageObjects.SlamPoint[]"
%typemap(csin,
         pre="    var list = new vectorp($csinput.Length);\n"
             "    foreach (var p in $csinput) {\n"
             "      list.Add(new NativePoint(p.Position.x, p.Position.y, p.Position.z, p.Color.r, p.Color.g, p.Color.b));\n"
             "    }",
             pgcppname="$csinput") const std::vector<NativePoint> & "$csclassname.getCPtr(list)"
%include "stdint.i"
%include "std_vector.i"
namespace std{
        %template(vectori) vector<int>;
        %template(vectori2d) vector<vector<int>>;
        %template(vectorp) vector<NativePoint>;
};
%include "MeshBuilder.h"
