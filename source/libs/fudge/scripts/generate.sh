#!/bin/bash

startPath=`pwd`

cd ../

find . -name .DS_Store -exec rm -rf {} \;

cd generate/Fudge

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Fudge.cs
mono ../../libs/MonoDevelop.TextTemplating/TextTransform.exe Fudge.tt -o ../../source/Fudge.cs

cd $startPath

cd ../generate/Fudge.Tests

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Tests.cs
mono ../../libs/MonoDevelop.TextTemplating/TextTransform.exe Fudge.Tests.tt -o ../../source/Fudge.Tests.cs

cd $startPath
