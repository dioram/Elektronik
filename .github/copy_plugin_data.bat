cd build\\Plugins\\%1
mkdir data\\
move libraries\\*.csv data\\
move libraries\\*.png data\\
cd libraries
for %%I in (../../../Elektronik_Data/Managed/*.*) do del %%~nxI
cd ../../../../