#!/bin/bash

startPath=`pwd`

cd ../build/Oats.Tests/

xbuild "Oats.Tests.csproj" /p:Configuration=Debug\ \(xs.mono40\)

cd $startPath
