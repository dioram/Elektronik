"D:\Unity\2020.3.18f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -serial %1 -username %2 -password %3 -logFile .\Logs\pre_build.log -executeMethod Elektronik.Editor.PlayerBuildScript.BuildAddressables -projectPath .\
"D:\Unity\2020.3.18f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -serial %1 -username %2 -password %3 -logFile .\Logs\build.log -projectPath .\ -buildWindows64Player .\build\Elektronik.exe 
git apply ./ProjectSettings/EnableVR.patch
"D:\Unity\2020.3.18f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -serial %1 -username %2 -password %3 -logFile .\Logs\build.log -projectPath .\ -buildWindows64Player .\build_vr\Elektronik.exe 
