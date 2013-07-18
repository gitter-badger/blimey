#!/bin/bash

startPath=`pwd`

# Generate Foundation.cs
cd ../generate/Foundation
mono-t4 Foundation.tt -o ../../source/Foundation.cs
cd $startPath

# Generate Foundation.Tests.cs
cd ../generate/Foundation.Tests
mono-t4 Foundation.Tests.tt -o ../../source/Foundation.Tests.cs
cd $startPath