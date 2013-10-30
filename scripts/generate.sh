#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Platform.MonoMac

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.Platform.MonoMac.cs
mono-t4 Cor.Platform.MonoMac.tt -o ../../source/Cor.Platform.MonoMac.cs

cd $startPath