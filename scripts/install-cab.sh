#!/bin/sh
cd ../source/CorAssetBuilder/
xbuild CorAssetBuilder.csproj /p:Configuration=Debug /verbosity:quiet /clp:Summary /nologo

rm -r -f /usr/local/bin/cab_app
mkdir /usr/local/bin/cab_app

cp cab /usr/local/bin/cab
chmod +x /usr/local/bin/cab

cp bin/Debug/CorAssetBuilder.exe /usr/local/bin/cab_app/CorAssetBuilder.exe
cp bin/Debug/*.dll /usr/local/bin/cab_app/
cp bin/Debug/*.dylib /usr/local/bin/cab_app/
cp bin/Debug/*.config /usr/local/bin/cab_app/

TIME="$(date +%s)"

# Rewrite the installation file.
echo "{" > $HOME/.cab.installation
echo "    \"InstallTime\":$TIME," >> $HOME/.cab.installation
echo "}" >> $HOME/.cab.installation

