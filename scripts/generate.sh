#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.StubPlatform

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.StubPlatform.cs
mono-t4 Cor.StubPlatform.tt -o ../../source/Cor.StubPlatform.cs

cd $startPath