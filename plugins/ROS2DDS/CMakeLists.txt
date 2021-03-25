cmake_minimum_required(VERSION 3.17)
project(ROS2DDS CXX)

find_package(fastcdr REQUIRED)
find_package(fastrtps REQUIRED)
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})
set_source_files_properties(interface/Connector.i PROPERTIES CPLUSPLUS ON)
set_property(SOURCE interface/Connector.i PROPERTY COMPILE_OPTIONS -dllimport Plugins/Ros/libraries/ROS2DDS.dll)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_FLAGS -EHsc)
set(DataTypes DataTypes/Ros.h DataTypes/Ros.cxx DataTypes/RosPubSubTypes.h DataTypes/RosPubSubTypes.cxx)

swig_add_library(ROS2DDS
        TYPE SHARED
        LANGUAGE csharp
        OUTPUT_DIR ${PROJECT_SOURCE_DIR}/../Ros/Ros2/Online/SWIG_generated
        OUTFILE_DIR ${PROJECT_SOURCE_DIR}
        SOURCES interface/Connector.h interface/Connector.cpp interface/Connector.i interface/Handlers.h
        interface/Messages.h interface/Logger.h ${DataTypes})
target_link_libraries(ROS2DDS PRIVATE fastrtps fastcdr)

set(INSTALL_DIR ${PROJECT_SOURCE_DIR}/../../build/Plugins/Ros/libraries)

install(TARGETS ROS2DDS RUNTIME DESTINATION ${INSTALL_DIR})
install(DIRECTORY "${_VCPKG_INSTALLED_DIR}/${VCPKG_TARGET_TRIPLET}/bin/" DESTINATION ${INSTALL_DIR} FILES_MATCHING PATTERN "*.dll")