cmake_minimum_required(VERSION 3.17)
project(MeshBuilder CXX)

set(CMAKE_CXX_STANDARD 17)
if (MSVC)
    set(CMAKE_CXX_FLAGS -EHsc)
endif ()

find_package(SWIG REQUIRED)
find_package(PCL REQUIRED)

include(${SWIG_USE_FILE})
set_source_files_properties(Builder.i PROPERTIES CPLUSPLUS ON)
set_property(SOURCE Builder.i PROPERTY COMPILE_OPTIONS -namespace Elektronik.DataSources.Containers.NativeMesh)

swig_add_library(MeshBuilder
        TYPE SHARED
        LANGUAGE csharp
        OUTPUT_DIR ${PROJECT_SOURCE_DIR}/../../Assets/Scripts/DataSources/Containers/NativeMesh
        OUTFILE_DIR ${PROJECT_SOURCE_DIR}
        SOURCES MeshBuilder.h MeshBuilder.cpp Builder.i)
if (WIN32)
    target_link_libraries(MeshBuilder ${PCL_LIBRARIES})
elseif(UNIX)
    target_link_libraries(MeshBuilder ${PCL_LIBRARIES} -lgomp)
endif ()

set(CMAKE_INSTALL_BINDIR ${PROJECT_SOURCE_DIR}/../../Assets/Plugins/Native)
install(DIRECTORY ${CMAKE_BINARY_DIR}/ DESTINATION ${CMAKE_INSTALL_BINDIR} FILES_MATCHING REGEX ".*\\.((lib)|(so)|(dll))")