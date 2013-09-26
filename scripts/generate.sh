#!/bin/bash

startPath=`pwd`

cd ../generate/Foundation

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Foundation.cs
mono-t4 Foundation.tt -o ../../source/Foundation.cs

cd $startPath

cd ../generate/Foundation.Tests

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# Generate Foundation.Tests.cs
mono-t4 Foundation.Tests.tt -o ../../source/Foundation.Tests.cs

cd $startPath