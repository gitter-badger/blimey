#!/bin/bash

# Cor
# ---
startPath=`pwd`
cd ../generate/Cor
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.tt -o ../../source/Cor.cs
cd $startPath

# Cor.Lib.Foundation
# --------------------------
startPath=`pwd`
cd ../generate/Cor.Lib.Foundation
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Lib.Foundation.tt -o ../../source/Cor.Lib.Foundation.cs
cd $startPath

# Cor.Lib.Foundation.Tests
# --------------------------------
startPath=`pwd`
cd ../generate/Cor.Lib.Foundation.Tests
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Lib.Foundation.Tests.tt -o ../../source/Cor.Lib.Foundation.Tests.cs
cd $startPath

# Cor.Lib.Khronos
# -----------------------
startPath=`pwd`
cd ../generate/Cor.Lib.Khronos
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Lib.Khronos.tt -o ../../source/Cor.Lib.Khronos.cs
cd $startPath

# Cor.Platform.MonoMac
# ----------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.MonoMac
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.MonoMac.tt -o ../../source/Cor.Platform.MonoMac.cs
cd $startPath

# Cor.Platform.Psm
# ------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Psm
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Psm.tt -o ../../source/Cor.Platform.Psm.cs
cd $startPath

# Cor.Platform.Xdroid
# ---------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Xdroid
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Xdroid.tt -o ../../source/Cor.Platform.Xdroid.cs
cd $startPath

# Cor.Platform.Xios
# -------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Xios
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Xios.tt -o ../../source/Cor.Platform.Xios.cs
cd $startPath

# Cor.Platform.Xna4
# -------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Xna4
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Xna4.tt -o ../../source/Cor.Platform.Xna4.cs
cd $startPath

# Cor.Platform.Stub
# -----------------
startPath=`pwd`
cd ../generate/Cor.Platform.Stub
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Stub.tt -o ../../source/Cor.Platform.Stub.cs
cd $startPath

