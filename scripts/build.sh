#!/bin/bash

startPath=`pwd`

cd ../build/xs.mono40/Fudge.Tests/

xbuild "Fudge.Tests (xs.mono40).csproj" /p:Configuration=Debug

cd $startPath
