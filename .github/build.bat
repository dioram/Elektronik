"C:\Program Files\Unity\Hub\Editor\2020.2.4f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -serial %1 -username %2 -password %3 -logFile .\Logs\pre_build.log -executeMethod Elektronik.Editor.PlayerBuildScript.BuildAddressables -projectPath .\
"C:\Program Files\Unity\Hub\Editor\2020.2.4f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -serial %1 -username %2 -password %3 -logFile .\Logs\build.log -projectPath .\ -buildWindows64Player .\Build\Elektronik.exe 
"C:\Program Files\Unity\Hub\Editor\2020.2.4f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -serial %1 -username %2 -password %3 -logFile .\Logs\tests.log -projectPath .\ -runTests -testResults .\tests.xml

cd plugins
dotnet publish ContextMenuSetter -o ../Build/Plugins/ContextMenuSetter
dotnet publish Updater -o ../Build/Plugins/Updater

dotnet publish Protobuf -o ../Build/Plugins/Protobuf/libraries
cd ..\\Build\\Plugins\\Protobuf\\libraries
mkdir ..\\data
move *.csv ..\\data
for %%I in (..\\..\\..\\Elektronik_Data\\Managed\\*.*) do del %%~nxI
cd ../../../../plugins
copy .\\Protobuf\\*.proto ..\\Build\\Plugins\\Protobuf\\Data

cd ROS2DDS
cmake -G "Ninja" -DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake -DCMAKE_CXX_COMPILER="C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/VC/Tools/Llvm/x64/bin/clang-cl.exe" -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --build .
cmake -G "Ninja" -DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake -DCMAKE_CXX_COMPILER="C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/VC/Tools/Llvm/x64/bin/clang-cl.exe" -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
cmake --install .
cd ..
dotnet publish Ros -o ../Build/Plugins/Ros/libraries
cd ..\\Build\\Plugins\\Ros\\libraries
mkdir ..\\data
move *.csv ..\\data
for %%I in (..\\..\\..\\Elektronik_Data\\Managed\\*.*) do del %%~nxI
cd ../../../../plugins