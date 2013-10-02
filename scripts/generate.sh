#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Xdroid

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.Xdroid.cs
mono-t4 Cor.Xdroid.tt -o ../../source/Cor.Xdroid.cs

cd $startPath