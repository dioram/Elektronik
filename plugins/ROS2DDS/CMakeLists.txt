cmake_minimum_required(VERSION 3.17)
project(ROS2DDS CXX)
include(CopyDlls.cmake)
include(GNUInstallDirs)

find_package(asio REQUIRED)
find_package(OpenSSL REQUIRED)
find_package(tinyxml2 REQUIRED)
find_package(foonathan_memory REQUIRED)
find_package(fastcdr REQUIRED)
find_package(fastrtps REQUIRED)
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})
set_source_files_properties(interface/Connector.i PROPERTIES CPLUSPLUS ON)
# SWIG command line option. Sets where to find native dll in runtime.
set_property(SOURCE interface/Connector.i PROPERTY COMPILE_OPTIONS -dllimport Plugins/Ros/libraries/ROS2DDS.dll)

if (MSVC)
    set(CMAKE_CXX_FLAGS -EHsc)
endif ()
set(DataTypes DataTypes/Ros.h DataTypes/Ros.cxx DataTypes/RosPubSubTypes.h DataTypes/RosPubSubTypes.cxx)

swig_add_library(ROS2DDS
        TYPE SHARED
        LANGUAGE csharp
        OUTPUT_DIR ${PROJECT_SOURCE_DIR}/../Ros/Ros2/Online/SWIG_generated
        OUTFILE_DIR ${PROJECT_SOURCE_DIR}
        SOURCES interface/Connector.h interface/Connector.cpp interface/Connector.i interface/Handlers.h
        interface/Messages.h interface/Logger.h ${DataTypes})
target_link_libraries(ROS2DDS PRIVATE fastrtps fastcdr)
set_target_properties(ROS2DDS PROPERTIES CMAKE_CXX_STANDARD 17)

set(CMAKE_INSTALL_PREFIX ${PROJECT_SOURCE_DIR}/../../build/Plugins/Ros/libraries CACHE PATH "installation prefix")
set(CMAKE_INSTALL_BINDIR ${PROJECT_SOURCE_DIR}/../../build/Plugins/Ros/libraries)

set(DEPLOY_DLLS)
file(GLOB DEPLOY_DLLS ${CMAKE_BINARY_DIR}/*.dll)
set(DEPLOY_SOS)
file(GLOB DEPLOY_SOS ${CMAKE_BINARY_DIR}/*.so)
set(DEPLOY_LIBS)
target_copy_libs(ROS2DDS foonathan_memory)
file(GLOB DEPLOY_LIBS ${CMAKE_BINARY_DIR}/*.lib)

install(FILES ${DEPLOY_DLLS} DESTINATION ${CMAKE_INSTALL_BINDIR})
install(FILES ${DEPLOY_LIBS} DESTINATION ${CMAKE_INSTALL_BINDIR})
