#!/bin/bash

startPath=`pwd`

cd ../generate/Blimey

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Blimey.cs
mono-t4 Blimey.tt -o ../../source/Blimey.cs

cd $startPath
