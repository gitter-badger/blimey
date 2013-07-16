#!/bin/bash

startPath=`pwd`

# Generate Cor.cs
cd ../generate/source
mono-t4 Cor.tt -o ../../source/Cor.cs
cd $startPath