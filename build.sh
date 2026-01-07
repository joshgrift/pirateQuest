#!/bin/bash

echo "Building and exporting project..."

# Extract version from project.godot
VERSION=$(grep 'config/version=' project.godot | sed 's/config\/version="\(.*\)"/\1/')

echo ==== dist/piratesquest-$VERSION.dmg ====
/Applications/Godot_mono.app/Contents/MacOS/Godot --headless --export-debug "macOS" dist/piratesquest-$VERSION.app
zip -r "dist/piratesquest-$VERSION.zip" "dist/piratesquest-$VERSION.app"

echo ==== dist/piratesquest-server-$VERSION.dmg ====
/Applications/Godot_mono.app/Contents/MacOS/Godot --headless --export-debug "macOS-server" dist/piratesquest-server-$VERSION.app
t-debug "macOS-server" dist/piratesquest-server-$VERSION.app
zip -r "dist/piratesquest-server-$VERSION.zip" "dist/piratesquest-server-$VERSION.app"