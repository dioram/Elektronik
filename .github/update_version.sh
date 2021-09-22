#!/bin/bash
export el=`cat ./ProjectSettings/ProjectSettings.asset | grep -oP '(?<=bundleVersion: )\d*.\d*.\d*(?=-rc\d*)?'`
perl -pe 's/(?<=bundleVersion: )\d*.\d*.\d*(?:-rc\d*)?/'$el'/' < ./ProjectSettings/ProjectSettings.asset > psl.txt
perl -pe 's/(?<=Version: )\d*.\d*.\d*(?:-rc\d*)?/'$el'/' < ./package/DEBIAN/control > control.txt
cp -f psl.txt ./ProjectSettings/ProjectSettings.asset
cp -f control.txt ./package/DEBIAN/control
