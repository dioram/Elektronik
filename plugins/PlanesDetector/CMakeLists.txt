CMAKE_MINIMUM_REQUIRED(VERSION 3.17)

SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

project(PlanesDetector CXX)

FIND_PACKAGE(OpenCV REQUIRED)
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})
set_source_files_properties(include/Detector.i PROPERTIES CPLUSPLUS ON)
set_property(SOURCE include/Detector.i PROPERTY COMPILE_OPTIONS
        -namespace Elektronik.Clusterization.Algorithms.PlanesDetectionNative
        -dllimport Plugins/PlanesDetection/libraries/PlanesDetector.dll)

if (MSVC)
    set(CMAKE_CXX_FLAGS -EHsc)
endif ()

include_directories(include)
swig_add_library(PlanesDetector
        TYPE SHARED
        LANGUAGE csharp
        OUTPUT_DIR ${PROJECT_SOURCE_DIR}/../../Plugins/Clusterization.PlanesDetection/Native
        OUTFILE_DIR ${PROJECT_SOURCE_DIR}
        SOURCES include/Octree.h include/Plane.h include/Point.h include/PointCloud.h include/Ransac.h include/RGB.h
                include/UnionFind.h include/Vector3.h src/Octree.cpp src/Plane.cpp src/PointCloud.cpp src/Ransac.cpp
                include/Detector.i include/PlanesDetector.h src/PlanesDetector.cpp)

TARGET_LINK_LIBRARIES(PlanesDetector ${OpenCV_LIBS})

set(CMAKE_INSTALL_BINDIR ${PROJECT_SOURCE_DIR}/../../build/Plugins/PlanesDetection/libraries)
install(DIRECTORY ${CMAKE_BINARY_DIR}/ DESTINATION ${CMAKE_INSTALL_BINDIR} FILES_MATCHING REGEX ".*\\.((lib)|(so)|(dll))")