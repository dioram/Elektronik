#!/bin/bash
./.github/copy_plugin_data.sh Ros
./.github/copy_plugin_data.sh Protobuf
cp ./plugins/Protobuf/*.proto ./build/Plugins/Protobuf/data
./.github/copy_plugin_data.sh KMeans
./.github/copy_plugin_data.sh PlanesDetection
mkdir -p ./build_vr/Plugins
cp -r ./build/Plugins/* ./build_vr/Plugins
echo Copied