#!/bin/bash

echo "Building and exporting project..."

# Extract version from project.godot
VERSION=$(grep 'config/version=' project.godot | sed 's/config\/version="\(.*\)"/\1/')

echo ==== dist/algonquin-$VERSION.dmg ====
/Applications/Godot_mono.app/Contents/MacOS/Godot --headless --export-debug "macOS" dist/algonquin-$VERSION.app

echo ==== dist/algonquin-server-$VERSION.dmg ====
/Applications/Godot_mono.app/Contents/MacOS/Godot --headless --export-debug "macOS-server" dist/algonquin-server-$VERSION.app