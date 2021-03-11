"C:\Program Files\Unity\Hub\Editor\2020.2.4f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -serial %1 -username %2 -password %3 -projectPath .\ -buildWindows64Player .\Build\Elektronik.exe
cd plugins
dotnet publish Protobuf -o ../Build/Plugins/Protobuf/libraries
dotnet publish Rosbag2 -o ../Build/Plugins/Rosbag2/libraries