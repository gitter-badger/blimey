#!/bin/sh
cd ../source/Blimey.AssetBuilder/
xbuild Blimey.AssetBuilder.csproj /p:Configuration=Debug /verbosity:quiet /clp:Summary /nologo

rm -r -f /usr/local/bin/bab_app
mkdir /usr/local/bin/bab_app

cp bab /usr/local/bin/bab
chmod +x /usr/local/bin/bab

cp bin/Debug/Blimey.AssetBuilder.exe /usr/local/bin/bab_app/Blimey.AssetBuilder.exe
cp bin/Debug/*.dll /usr/local/bin/bab_app/
cp bin/Debug/*.dylib /usr/local/bin/bab_app/
cp bin/Debug/*.config /usr/local/bin/bab_app/
cp bin/Debug/*.mdb /usr/local/bin/bab_app/

TIME="$(date +%s)"

# Rewrite the installation file.
echo "{" > $HOME/.bab.installation
echo "    \"InstallTime\":$TIME," >> $HOME/.bab.installation
echo "}" >> $HOME/.bab.installation

