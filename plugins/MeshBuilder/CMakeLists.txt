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
set_property(SOURCE Builder.i PROPERTY COMPILE_OPTIONS -namespace Elektronik.Mesh.Native)

swig_add_library(MeshBuilder
        TYPE SHARED
        LANGUAGE csharp
        OUTPUT_DIR ${PROJECT_SOURCE_DIR}/../../Assets/Scripts/Mesh/MeshBuildNative
        OUTFILE_DIR ${PROJECT_SOURCE_DIR}
        SOURCES MeshBuilder.h MeshBuilder.cpp Builder.i)
if (WIN32)
    target_link_libraries(MeshBuilder ${PCL_LIBRARIES})
elseif(UNIX)
    target_link_libraries(MeshBuilder ${PCL_LIBRARIES} -lgomp)
endif ()

set(CMAKE_INSTALL_PREFIX ${PROJECT_SOURCE_DIR}/../../Assets/Plugins/Native CACHE PATH "installation prefix")
set(CMAKE_INSTALL_BINDIR ${PROJECT_SOURCE_DIR}/../../Assets/Plugins/Native)
set(DEPLOY_DLLS)
file(GLOB DEPLOY_DLLS ${CMAKE_BINARY_DIR}/*.dll)
set(DEPLOY_SOS)
file(GLOB DEPLOY_SOS ${CMAKE_BINARY_DIR}/*.so)
set(DEPLOY_LIBS)
file(GLOB DEPLOY_LIBS ${CMAKE_BINARY_DIR}/*.lib)
install(FILES ${DEPLOY_DLLS} DESTINATION ${CMAKE_INSTALL_BINDIR})
install(FILES ${DEPLOY_SOS} DESTINATION ${CMAKE_INSTALL_BINDIR})
install(FILES ${DEPLOY_LIBS} DESTINATION ${CMAKE_INSTALL_BINDIR})