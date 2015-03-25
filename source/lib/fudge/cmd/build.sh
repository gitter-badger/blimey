#!/bin/bash

startPath=`pwd`

cd ../build/xamarin_studio.mono40/Fudge.Tests/

xbuild "Fudge.Tests (xamarin_studio.mono40).csproj" /p:Configuration=Debug

cd $startPath
