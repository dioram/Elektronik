#!/bin/bash
mkdir -p ./package/opt/elektronik
cp -r ./build/* ./package/opt/elektronik
dpkg-deb --build package
mv package.deb elektronik.deb

rm -rf ./package/opt/elektronik/*
cp -r ./build_vr/* ./package/opt/elektronik
dpkg-deb --build package
mv package.deb elektronik_vr.deb