#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Xios

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.Xios.cs
mono-t4 Cor.Xios.tt -o ../../source/Cor.Xios.cs

cd $startPath