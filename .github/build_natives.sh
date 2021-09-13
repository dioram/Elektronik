#!/bin/bash
set -e
echo $1
cmake -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release $1 .
cmake --build .
cmake -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release $1 .
cmake --install .
