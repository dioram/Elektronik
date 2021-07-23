cmake_minimum_required(VERSION 3.19)
project(NativeMath CXX)

set(CMAKE_CXX_STANDARD 17)
if (MSVC)
    set(CMAKE_CXX_FLAGS -EHsc)
endif ()
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -march=native")

find_package(OpenMVS CONFIG REQUIRED)
find_package(SWIG REQUIRED)

include(${SWIG_USE_FILE})
set_source_files_properties(Math.i PROPERTIES CPLUSPLUS ON)
set_property(SOURCE Math.i PROPERTY COMPILE_OPTIONS -namespace Elektronik.NativeMath)

swig_add_library(NativeMath
        TYPE SHARED
        LANGUAGE csharp
        OUTPUT_DIR ${PROJECT_SOURCE_DIR}/../../Assets/Plugins/NativeMath/SwigGenerated
        OUTFILE_DIR ${PROJECT_SOURCE_DIR}
        SOURCES MeshBuilder.h MeshBuilder.cpp Math.i NativeStructs.h PlanesDetector.h PlanesDetector.cpp
        PlaneDetection/Octree.h PlaneDetection/Octree.cpp PlaneDetection/Plane.h PlaneDetection/Plane.cpp
        PlaneDetection/Point.h PlaneDetection/PointCloud.h PlaneDetection/PointCloud.cpp PlaneDetection/Ransac.h
        PlaneDetection/Ransac.cpp PlaneDetection/RGB.h PlaneDetection/UnionFind.h PlaneDetection/Vector3.h)
target_link_libraries(NativeMath ${OpenMVS_LIBRARIES})

set(CMAKE_INSTALL_BINDIR ${PROJECT_SOURCE_DIR}/../../Assets/Plugins/NativeMath)
set(DEPLOY_DLLS)
file(GLOB DEPLOY_DLLS ${CMAKE_BINARY_DIR}/*.dll)
# TODO: Copy *.so on linux
set(DEPLOY_LIBS)
file(GLOB DEPLOY_LIBS ${CMAKE_BINARY_DIR}/*.lib)
install(FILES ${DEPLOY_DLLS} DESTINATION ${CMAKE_INSTALL_BINDIR})
install(FILES ${DEPLOY_LIBS} DESTINATION ${CMAKE_INSTALL_BINDIR})