#!/bin/bash

startPath=`pwd`

cd ../build/Oats.Tests/

xbuild "Oats.Tests.csproj" /p:Configuration=Debug && exit 1

cd $startPath
