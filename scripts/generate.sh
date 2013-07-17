#!/bin/bash

startPath=`pwd`

# Generate Blimey.cs
cd ../generate/source
mono-t4 Blimey.tt -o ../../source/Blimey.cs
cd $startPath