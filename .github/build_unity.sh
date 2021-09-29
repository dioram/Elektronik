#!/bin/bash
set -e
/home/srv1/Unity/Hub/Editor/2020.3.19f1/Editor/Unity -quit -accept-apiupdate -batchmode -nographics -logFile ./Logs/pre_build.log -executeMethod Elektronik.Editor.PlayerBuildScript.BuildAddressables -projectPath ./
/home/srv1/Unity/Hub/Editor/2020.3.19f1/Editor/Unity -quit -accept-apiupdate -nographics -batchmode -logFile ./Logs/build.log -projectPath ./ -buildLinux64Player ./build/Elektronik
git apply ./ProjectSettings/EnableVR.patch
/home/srv1/Unity/Hub/Editor/2020.3.19f1/Editor/Unity -quit -accept-apiupdate -nographics -batchmode -logFile ./Logs/build.log -projectPath ./ -buildLinux64Player ./build_vr/Elektronik
