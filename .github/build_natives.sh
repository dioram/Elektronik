cd plugins/ROS2DDS
cmake -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --build .
cmake -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --install .

cd ../MeshBuilder
cmake -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --build .
cmake -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --install .

cd ../PlanesDetector
cmake -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --build .
cmake -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --install .