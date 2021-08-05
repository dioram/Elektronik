cd ./
cd plugins
dotnet publish ContextMenuSetter -o ../Build/Plugins/ContextMenuSetter
dotnet publish Updater -o ../Build/Plugins/Updater

dotnet publish Protobuf -o ../Build/Plugins/Protobuf/libraries
cd ..\\Build\\Plugins\\Protobuf\\libraries
mkdir ..\\data
mv *.csv ..\\data
for %%I in (..\\..\\..\\Elektronik_Data\\Managed\\*.*) do del %%~nxI
cd ../../../../plugins
copy .\\Protobuf\\*.proto ..\\Build\\Plugins\\Protobuf\\Data

dotnet publish Ros -o ../Build/Plugins/Ros/libraries
cd ..\\Build\\Plugins\\Ros\\libraries
mkdir ..\\data
mv *.csv ..\\data
for %%I in (..\\..\\..\\Elektronik_Data\\Managed\\*.*) do del %%~nxI
cd ../../../../plugins