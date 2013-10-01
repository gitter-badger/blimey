#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.MonoMac

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.MonoMac.cs
mono-t4 Cor.MonoMac.tt -o ../../source/Cor.MonoMac.cs

cd $startPath