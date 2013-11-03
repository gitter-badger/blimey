#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Platform.Managed.Xios

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.Platform.Managed.Xios.cs
mono-t4 Cor.Platform.Managed.Xios.tt -o ../../source/Cor.Platform.Managed.Xios.cs

cd $startPath