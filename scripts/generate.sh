#!/bin/bash

startPath=`pwd`

# Generate Blimey.cs
cd ../generate/Blimey
mono-t4 Blimey.tt -o ../../source/Blimey.cs
cd $startPath

# Generate Tests.cs
cd ../generate/Blimey.Tests
mono-t4 Blimey.Tests.tt -o ../../source/Blimey.Tests.cs
cd $startPath