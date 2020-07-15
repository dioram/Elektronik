cmake_minimum_required(VERSION 3.15)
project(cxx_example)

list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/CMake")

include(third_party/install.cmake)

add_subdirectory(Tests)