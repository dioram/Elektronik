cd plugins/ROS2DDS
cmake -G "Ninja" -DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake -DCMAKE_CXX_COMPILER="C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/VC/Tools/Llvm/x64/bin/clang-cl.exe" -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --build .
cmake -G "Ninja" -DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake -DCMAKE_CXX_COMPILER="C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/VC/Tools/Llvm/x64/bin/clang-cl.exe" -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --install .

cd ../PlanesDetector
cmake -G "Ninja" -DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake -DCMAKE_CXX_COMPILER="C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/VC/Tools/Llvm/x64/bin/clang-cl.exe" -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --build .
cmake -G "Ninja" -DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake -DCMAKE_CXX_COMPILER="C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/VC/Tools/Llvm/x64/bin/clang-cl.exe" -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --install .