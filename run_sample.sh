#!/bin/bash
sh build.sh

cd sample/Sample
dotnet run 15 -f netcoreapp2.0
cd ..