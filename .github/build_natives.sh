#!/bin/bash
set -e
cd $1
cmake -DCMAKE_TOOLCHAIN_FILE=~/vcpkg/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --build .
cmake --install .
