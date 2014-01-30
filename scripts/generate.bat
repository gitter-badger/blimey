cd ../generate/
:: Cor
:: ---
cd Cor/
TextTransform.exe Cor.tt -o ../../source/Cor.cs
cd ../

:: Cor.Lib.Managed.Foundation
:: --------------------------
cd Cor.Lib.Managed.Foundation/
TextTransform.exe Cor.Lib.Managed.Foundation.tt -o ../../source/Cor.Lib.Managed.Foundation.cs
cd ../

:: Cor.Lib.Managed.Foundation.Tests
:: --------------------------------
cd Cor.Lib.Managed.Foundation.Tests/
TextTransform.exe Cor.Lib.Managed.Foundation.Tests.tt -o ../../source/Cor.Lib.Managed.Foundation.Tests.cs
cd ../

:: Cor.Lib.Managed.Khronos
:: -----------------------
cd Cor.Lib.Managed.Khronos/
TextTransform.exe Cor.Lib.Managed.Khronos.tt -o ../../source/Cor.Lib.Managed.Khronos.cs
cd ../

:: Cor.Platform.Managed.MonoMac
:: ----------------------------
cd Cor.Platform.Managed.MonoMac/
TextTransform.exe Cor.Platform.Managed.MonoMac.tt -o ../../source/Cor.Platform.Managed.MonoMac.cs
cd ../

:: Cor.Platform.Managed.Psm
:: ------------------------
cd Cor.Platform.Managed.Psm/
TextTransform.exe Cor.Platform.Managed.Psm.tt -o ../../source/Cor.Platform.Managed.Psm.cs
cd ../

:: Cor.Platform.Managed.Xdroid
:: ---------------------------
cd Cor.Platform.Managed.Xdroid/
TextTransform.exe Cor.Platform.Managed.Xdroid.tt -o ../../source/Cor.Platform.Managed.Xdroid.cs
cd ../

:: Cor.Platform.Managed.Xios
:: -------------------------
cd Cor.Platform.Managed.Xios/
TextTransform.exe Cor.Platform.Managed.Xios.tt -o ../../source/Cor.Platform.Managed.Xios.cs
cd ../

:: Cor.Platform.Managed.Xna4
:: -------------------------
cd Cor.Platform.Managed.Xna4/
TextTransform.exe Cor.Platform.Managed.Xna4.tt -o ../../source/Cor.Platform.Managed.Xna4.cs
cd ../

:: Cor.Platform.Stub
:: -----------------
cd Cor.Platform.Stub/
TextTransform.exe Cor.Platform.Stub.tt -o ../../source/Cor.Platform.Stub.cs
cd ../

cd ../