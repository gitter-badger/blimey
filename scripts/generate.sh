#!/bin/bash

startPath=`pwd`

# Generate Blimey.cs
cd ../generate/Blimey
mono-t4 Blimey.tt -o ../../source/Blimey.cs
cd $startPath

# now build to check it's good
#cd ../build_examples/Sungiant.Blimey
#xbuild "Sungiant.Blimey (xios).csproj" /p:Configuration=Debug /verbosity:minimal
#cd $startPath