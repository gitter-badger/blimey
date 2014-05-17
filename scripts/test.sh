#!/bin/bash

startPath=`pwd`

mono ../libs/NUnit-2.6.3/bin/nunit-console.exe ../build/Oats.Tests/bin/Debug/Oats.Tests\ \(xs.mono40\).dll

cd $startPath
