cd Plugins
dotnet publish ContextMenuSetter -o ../build/Plugins/ContextMenuSetter
dotnet publish Updater -o ../build/Plugins/Updater

dotnet publish Protobuf -o ../build/Plugins/Protobuf/libraries
cd ../build/Plugins/Protobuf
mkdir ./data
move ./libraries/*.csv ./data
cd ../../../Plugins
cp ./Protobuf/*.proto ../build/Plugins/Protobuf/Data

dotnet publish Ros -o ../build/Plugins/Ros/libraries
cd ../build/Plugins/Ros/libraries
mkdir ./data
move ./libraries/*.csv ./data
cd ../../../Plugins