#!/bin/bash
set -e

mv ./build/StandaloneLinux64/* ./build/
rm -rf ./build/StandaloneLinux64
mv ./build_vr/StandaloneLinux64/* ./build_vr/
rm -rf ./build_vr/StandaloneLinux64

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

# Copy plugins to VR build

cd ../
mkdir -p ./build_vr/Plugins
cp -r ./build/Plugins/* ./build_vr/Plugins