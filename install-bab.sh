#!/bin/sh
python build.py

rm -r -f /usr/local/bin/bab_app
mkdir /usr/local/bin/bab_app

cp bab /usr/local/bin/bab
chmod +x /usr/local/bin/bab

cp bin/blimey.asset.builder.exe /usr/local/bin/bab_app/blimey.asset.builder.exe
cp bin/*.dll /usr/local/bin/bab_app/
cp bin/*.mdb /usr/local/bin/bab_app/

TIME="$(date +%s)"

# Rewrite the installation file.
echo "{" > $HOME/.bab.installation
echo "    \"InstallTime\":$TIME," >> $HOME/.bab.installation
echo "}" >> $HOME/.bab.installation

