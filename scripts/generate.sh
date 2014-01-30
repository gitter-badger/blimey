#!/bin/bash

# Cor
# ---
startPath=`pwd`
cd ../generate/Cor
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.tt -o ../../source/Cor.cs
cd $startPath

# Cor.Lib.Managed.Foundation
# --------------------------
startPath=`pwd`
cd ../generate/Cor.Lib.Managed.Foundation
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Lib.Managed.Foundation.tt -o ../../source/Cor.Lib.Managed.Foundation.cs
cd $startPath

# Cor.Lib.Managed.Foundation.Tests
# --------------------------------
startPath=`pwd`
cd ../generate/Cor.Lib.Managed.Foundation.Tests
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Lib.Managed.Foundation.Tests.tt -o ../../source/Cor.Lib.Managed.Foundation.Tests.cs
cd $startPath

# Cor.Lib.Managed.Khronos
# -----------------------
startPath=`pwd`
cd ../generate/Cor.Lib.Managed.Khronos
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Lib.Managed.Khronos.tt -o ../../source/Cor.Lib.Managed.Khronos.cs
cd $startPath

# Cor.Platform.Managed.MonoMac
# ----------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Managed.MonoMac
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Managed.MonoMac.tt -o ../../source/Cor.Platform.Managed.MonoMac.cs
cd $startPath

# Cor.Platform.Managed.Psm
# ------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Managed.Psm
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Managed.Psm.tt -o ../../source/Cor.Platform.Managed.Psm.cs
cd $startPath

# Cor.Platform.Managed.Xdroid
# ---------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Managed.Xdroid
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Managed.Xdroid.tt -o ../../source/Cor.Platform.Managed.Xdroid.cs
cd $startPath

# Cor.Platform.Managed.Xios
# -------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Managed.Xios
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Managed.Xios.tt -o ../../source/Cor.Platform.Managed.Xios.cs
cd $startPath

# Cor.Platform.Managed.Xna4
# -------------------------
startPath=`pwd`
cd ../generate/Cor.Platform.Managed.Xna4
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Managed.Xna4.tt -o ../../source/Cor.Platform.Managed.Xna4.cs
cd $startPath

# Cor.Platform.Stub
# -----------------
startPath=`pwd`
cd ../generate/Cor.Platform.Stub
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;
mono-t4 Cor.Platform.Stub.tt -o ../../source/Cor.Platform.Stub.cs
cd $startPath

