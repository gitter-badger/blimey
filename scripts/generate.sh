#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Platform.Xios

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.Platform.Xios.cs
mono-t4 Cor.Platform.Xios.tt -o ../../source/Cor.Platform.Xios.cs

cd $startPath