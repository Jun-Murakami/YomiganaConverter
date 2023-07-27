#!/bin/bash

dotnet restore -r osx-x64
dotnet publish -c Release --self-contained -p:PublishSingleFile=true --runtime osx-x64
mkdir -p YomiganaConverter.app/Contents/MacOS
cp -r bin/Release/net6.0/osx-x64/publish/* YomiganaConverter.app/Contents/MacOS
cp Info.plist YomiganaConverter.app/Contents/
mkdir YomiganaConverter.app/Contents/Resources
cp Assets/yomigana.icns YomiganaConverter.app/Contents/Resources