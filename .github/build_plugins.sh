#!/bin/bash
set -e

cd ./build/
mv ./StandaloneLinux64/* ./
rm -rf ./StandaloneLinux64
cd ../
cd ./plugins

dotnet publish Protobuf -o ../build/Plugins/Protobuf/libraries
cd ../build/Plugins/Protobuf
mkdir ./data
mv ./libraries/*.csv ./data
cd ../../../plugins
cp ./Protobuf/*.proto ../build/Plugins/Protobuf/data

dotnet publish Ros -o ../build/Plugins/Ros/libraries
cd ../build/Plugins/Ros
mkdir ./data
mv ./libraries/*.csv ./data
cd ../../../plugins


cd ../build_vr/
mv ./StandaloneLinux64/* ./
rm -rf ./StandaloneLinux64
cd ../
cd ./plugins

dotnet publish Protobuf -o ../build_vr/Plugins/Protobuf/libraries
cd ../build_vr/Plugins/Protobuf
mkdir ./data
mv ./libraries/*.csv ./data
cd ../../../plugins
cp ./Protobuf/*.proto ../build_vr/Plugins/Protobuf/data

dotnet publish Ros -o ../build_vr/Plugins/Ros/libraries
cd ../build_vr/Plugins/Ros
mkdir ./data
mv ./libraries/*.csv ./data
cd ../../../plugins
