cd ../generate/

:: Fudge
:: ------
cd Fudge/
TextTransform.exe Fudge.tt -o ../../source/Fudge.cs -r System.Core.dll
cd ../

:: Fudge.Tests
:: ------------
cd Fudge.Tests/
TextTransform.exe Fudge.Tests.tt -o ../../source/Fudge.Tests.cs -r System.Core.dll
cd ../

cd ../
