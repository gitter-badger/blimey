#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Lib.Managed.Khronos

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Khronos.cs
mono-t4 Cor.Lib.Managed.Khronos.tt -o ../../source/Cor.Lib.Managed.Khronos.cs

cd $startPath