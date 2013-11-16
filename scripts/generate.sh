#!/bin/bash

startPath=`pwd`

cd ../generate/Cor

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.cs
mono-t4 Cor.tt -o ../../source/Cor.cs

cd $startPath

cd ../generate/Cor.Tests

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.Tests.cs
mono-t4 Cor.Tests.tt -o ../../source/Cor.Tests.cs

cd $startPath