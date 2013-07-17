#!/bin/bash

startPath=`pwd`

# Generate Blimey.cs
cd ../generate/Blimey
mono-t4 Blimey.tt -o ../../source/Blimey.cs
cd $startPath