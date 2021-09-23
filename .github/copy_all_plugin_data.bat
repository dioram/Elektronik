./.github/copy_plugin_data.sh Ros
./.github/copy_plugin_data.sh Protobuf
copy .\\plugins\\Protobuf\\*.proto .\\build\\plugins\\Protobuf\\data
./.github/copy_plugin_data.sh KMeans
./.github/copy_plugin_data.sh PlanesDetection
xcopy .\\build\\plugins\\ .\\build_vr\\plugins\\