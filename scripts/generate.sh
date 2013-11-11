#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Platform.Stub

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.Platform.Stub.cs
mono-t4 Cor.Platform.Stub.tt -o ../../source/Cor.Platform.Stub.cs

cd $startPath