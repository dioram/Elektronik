"C:\Program Files\Unity\Hub\Editor\2020.2.4f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -serial %1 -username %2 -password %3 -projectPath .\ -buildWindows64Player .\Build\Elektronik.exe
cd plugins
dotnet publish Protobuf -o ../Build/Plugins/Protobuf/libraries

cd ROS2DDS
cmake -G "Ninja" -DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake -DCMAKE_CXX_COMPILER="C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/VC/Tools/Llvm/x64/bin/clang-cl.exe" -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --build .
cmake --install .
cd ..
dotnet publish Ros -o ../Build/Plugins/Ros/libraries