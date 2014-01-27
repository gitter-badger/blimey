#!/bin/bash

cd cor/scripts/
sh generate.sh
cd ../../

cd cor.platform.stub/scripts/
sh generate.sh
cd ../../

cd cor.platform.managed.monomac/scripts/
sh generate.sh
cd ../../

cd cor.lib.managed.khronos/scripts/
sh generate.sh
cd ../../
