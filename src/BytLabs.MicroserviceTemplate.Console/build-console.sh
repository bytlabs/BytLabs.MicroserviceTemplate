#!/usr/bin/env bash
# Builds the Next.js console (static export) and copies it into the API's wwwroot/console so the
# .NET API serves it under /console. Run from anywhere.
set -euo pipefail
cd "$(dirname "$0")"

npm ci
npm run build   # produces ./out (basePath /console)

DEST="../BytLabs.MicroserviceTemplate.Api/wwwroot/console"
rm -rf "$DEST"
mkdir -p "$DEST"
cp -r out/* "$DEST/"
echo "Console copied to $DEST"
