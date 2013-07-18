#!/bin/bash

startPath=`pwd`

# Generate Cor.Xios.cs
cd ../generate/Cor.Xios
mono-t4 Cor.Xios.tt -o ../../source/Cor.Xios.cs
cd $startPath