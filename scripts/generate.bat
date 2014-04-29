cd ../generate/
:: Cor
:: ---
cd Cor/
TextTransform.exe Cor.tt -o ../../source/Cor.cs
cd ../

:: Cor.Lib.Foundation
:: --------------------------
cd Cor.Lib.Foundation/
TextTransform.exe Cor.Lib.Foundation.tt -o ../../source/Cor.Lib.Foundation.cs
cd ../

:: Cor.Lib.Foundation.Tests
:: --------------------------------
cd Cor.Lib.Foundation.Tests/
TextTransform.exe Cor.Lib.Foundation.Tests.tt -o ../../source/Cor.Lib.Foundation.Tests.cs
cd ../

:: Cor.Lib.Khronos
:: -----------------------
cd Cor.Lib.Khronos/
TextTransform.exe Cor.Lib.Khronos.tt -o ../../source/Cor.Lib.Khronos.cs
cd ../

:: Cor.Platform.MonoMac
:: ----------------------------
cd Cor.Platform.MonoMac/
TextTransform.exe Cor.Platform.MonoMac.tt -o ../../source/Cor.Platform.MonoMac.cs
cd ../

:: Cor.Platform.Psm
:: ------------------------
cd Cor.Platform.Psm/
TextTransform.exe Cor.Platform.Psm.tt -o ../../source/Cor.Platform.Psm.cs
cd ../

:: Cor.Platform.Xdroid
:: ---------------------------
cd Cor.Platform.Xdroid/
TextTransform.exe Cor.Platform.Xdroid.tt -o ../../source/Cor.Platform.Xdroid.cs
cd ../

:: Cor.Platform.Xios
:: -------------------------
cd Cor.Platform.Xios/
TextTransform.exe Cor.Platform.Xios.tt -o ../../source/Cor.Platform.Xios.cs
cd ../

:: Cor.Platform.Xna4
:: -------------------------
cd Cor.Platform.Xna4/
TextTransform.exe Cor.Platform.Xna4.tt -o ../../source/Cor.Platform.Xna4.cs
cd ../

:: Cor.Platform.Stub
:: -----------------
cd Cor.Platform.Stub/
TextTransform.exe Cor.Platform.Stub.tt -o ../../source/Cor.Platform.Stub.cs
cd ../

cd ../