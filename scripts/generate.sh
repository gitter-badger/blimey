#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Platform.Managed.MonoMac

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Cor.Platform.Managed.MonoMac.cs
mono-t4 Cor.Platform.Managed.MonoMac.tt -o ../../source/Cor.Platform.Managed.MonoMac.cs

cd $startPath