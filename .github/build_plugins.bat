cd plugins
dotnet publish ContextMenuSetter -o ../build_vr/Plugins/ContextMenuSetter
dotnet publish ContextMenuSetter -o ../build/Plugins/ContextMenuSetter
dotnet publish Updater -o ../build/Plugins/Updater
dotnet publish Updater -o ../build_vr/Plugins/Updater

dotnet publish Protobuf -o ../build/Plugins/Protobuf/libraries
cd ..\\build\\Plugins\\Protobuf\\libraries
mkdir ..\\data
move *.csv ..\\data
for %%I in (..\\..\\..\\Elektronik_Data\\Managed\\*.*) do del %%~nxI
cd ../../../../plugins
copy .\\Protobuf\\*.proto ..\\build\\Plugins\\Protobuf\\Data

dotnet publish Protobuf -o ../build_vr/Plugins/Protobuf/libraries
cd ..\\build_vr\\Plugins\\Protobuf\\libraries
mkdir ..\\data
move *.csv ..\\data
for %%I in (..\\..\\..\\Elektronik_Data\\Managed\\*.*) do del %%~nxI
cd ../../../../plugins
copy .\\Protobuf\\*.proto ..\\build_vr\\Plugins\\Protobuf\\Data

dotnet publish Ros -o ../Build/Plugins/Ros/libraries
cd ..\\Build\\Plugins\\Ros\\libraries
mkdir ..\\data
move *.csv ..\\data
for %%I in (..\\..\\..\\Elektronik_Data\\Managed\\*.*) do del %%~nxI
cd ../../../../plugins

dotnet publish Ros -o ../build_vr/Plugins/Ros/libraries
cd ..\\build_vr\\Plugins\\Ros\\libraries
mkdir ..\\data
move *.csv ..\\data
for %%I in (..\\..\\..\\Elektronik_Data\\Managed\\*.*) do del %%~nxI
cd ../../../../plugins