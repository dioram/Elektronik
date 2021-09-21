#!/bin/bash

cd ./build/Plugins/$1
mkdir ./data
mv ./libraries/*.csv ./data
mv ./libraries/*.png ./data
echo "Copied!"