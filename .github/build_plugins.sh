#/bin/bash
set -e

cd ./build/
mv ./StandaloneLinux64/* ./
rm -rf ./StandaloneLinux64
ls -la
cd ../
cd ./plugins

dotnet publish Protobuf -o ../build/Plugins/Protobuf/libraries
cd ../build/Plugins/Protobuf
mkdir ./data
mv ./libraries/*.csv ./data
cd ../../../plugins
cp ./Protobuf/*.proto ../build/Plugins/Protobuf/data

dotnet publish Ros -o ../build/Plugins/Ros/libraries
cd ../build/Plugins/Ros/libraries
mkdir ./data
mv ./libraries/*.csv ./data
cd ../../../plugins
