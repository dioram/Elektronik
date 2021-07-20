CMAKE_MINIMUM_REQUIRED(VERSION 3.17)

SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

project(PlanesDetector CXX)

FIND_PACKAGE(OpenCV REQUIRED)
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})
set_source_files_properties(include/Detector.i PROPERTIES CPLUSPLUS ON)
set_property(SOURCE include/Detector.i PROPERTY COMPILE_OPTIONS -namespace Elektronik.Clusterization.Algorithms.PlanesDetectionNative)

if (MSVC)
    set(CMAKE_CXX_FLAGS -EHsc)
endif ()

add_executable(PlanesDetectorTester include/Octree.h include/Plane.h include/Point.h include/PointCloud.h
        include/Ransac.h include/RGB.h include/UnionFind.h include/Vec3.h src/Octree.cpp src/Plane.cpp
        src/PointCloud.cpp src/Ransac.cpp src/main.cpp)
TARGET_LINK_LIBRARIES(PlanesDetectorTester ${OpenCV_LIBS})

swig_add_library(PlanesDetector
        TYPE SHARED
        LANGUAGE csharp
        OUTPUT_DIR ${PROJECT_SOURCE_DIR}/../../Assets/Scripts/Clusterization/Algorithms/PlanesDetectionNative
        OUTFILE_DIR ${PROJECT_SOURCE_DIR}
        SOURCES include/Octree.h include/Plane.h include/Point.h include/PointCloud.h include/Ransac.h include/RGB.h
                include/UnionFind.h include/Vec3.h src/Octree.cpp src/Plane.cpp src/PointCloud.cpp src/Ransac.cpp
                include/Detector.i)

TARGET_LINK_LIBRARIES(PlanesDetector ${OpenCV_LIBS})

set(CMAKE_INSTALL_BINDIR ${PROJECT_SOURCE_DIR}/../../Assets/Plugins/Native)

set(DEPLOY_DLLS)
file(GLOB DEPLOY_DLLS ${CMAKE_BINARY_DIR}/*.dll)
set(DEPLOY_LIBS)
file(GLOB DEPLOY_LIBS ${CMAKE_BINARY_DIR}/*.lib)
install(FILES ${DEPLOY_DLLS} DESTINATION ${CMAKE_INSTALL_BINDIR})
install(FILES ${DEPLOY_LIBS} DESTINATION ${CMAKE_INSTALL_BINDIR})