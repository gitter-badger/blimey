#!/bin/bash

startPath=`pwd`

mono ../libs/NUnit-2.6.3/bin/nunit-console.exe ../build/xs.mono40/Fudge.Tests/bin/Debug/Fudge.Tests.dll

cd $startPath
