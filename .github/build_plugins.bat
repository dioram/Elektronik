cd plugins
dotnet publish ContextMenuSetter -o ../build/Plugins/ContextMenuSetter
dotnet publish Updater -o ../build/Plugins/Updater
dotnet publish Clustering.KMeans -o ../build/Plugins/KMeans
dotnet publish Clustering.PlanesDetection -o ../build/Plugins/PlanesDetection

dotnet publish Protobuf -o ../build/Plugins/Protobuf/libraries
cd ../build/Plugins/Protobuf/libraries
mkdir ../data
move *.csv ../data
move *.png ../data
for %%I in (../../../Elektronik_Data/Managed/*.*) do del %%~nxI
cd ../../../../plugins
copy ./Protobuf/*.proto ../build/Plugins/Protobuf/Data

dotnet publish Ros -o ../Build/Plugins/Ros/libraries
cd ../build/Plugins/Ros/libraries
mkdir ../data
move *.csv ../data
move *.png ../data
for %%I in (../../../Elektronik_Data/Managed/*.*) do del %%~nxI
cd ../../../../plugins

cd ../
mkdir ./build_vr/Plugins
copy ./build/Plugins/* ../build_vr/Plugins/