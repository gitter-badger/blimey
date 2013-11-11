#!/bin/bash

startPath=`pwd`

cd ../generate/Cor.Lib.Managed.Foundation

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Foundation.cs
mono-t4 Cor.Lib.Managed.Foundation.tt -o ../../source/Cor.Lib.Managed.Foundation.cs

cd $startPath

cd ../generate/Cor.Lib.Managed.Foundation.Tests

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Foundation.Tests.cs
mono-t4 Cor.Lib.Managed.Foundation.Tests.tt -o ../../source/Cor.Lib.Managed.Foundation.Tests.cs

cd $startPath