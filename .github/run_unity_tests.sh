#!/bin/bash
set -e
/home/srv1/Unity/Hub/Editor/2020.3.16f1/Editor/Unity -accept-apiupdate -batchmode -nographics -logFile ./Logs/tests.log -projectPath ./ -runTests -testResults ./tests.xml -runSynchronously