cd ./build/
mv -rf ./StandaloneLinux64/* ./
rm ./StandaloneLinux64
ls -la
cd ../
cd ./plugins

sudo dotnet publish Protobuf -o ../build/Plugins/Protobuf/libraries
cd ../build/Plugins/Protobuf
mkdir ./data
mv ./libraries/*.csv ./data
cd ../../../plugins
cp ./Protobuf/*.proto ../build/Plugins/Protobuf/Data

sudo dotnet publish Ros -o ../build/Plugins/Ros/libraries
cd ../build/Plugins/Ros/libraries
mkdir ./data
mv ./libraries/*.csv ./data
cd ../../../plugins
