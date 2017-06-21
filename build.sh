#!/bin/bash
set -e 
dotnet --info
dotnet restore

for path in src/**/*.csproj; do
    dotnet build -f netstandard1.1 -c Release ${path}
done

for path in test/*.Tests/*.csproj; do
    dotnet test -f netcoreapp1.0  -c Release ${path}
done

dotnet build -f netcoreapp1.0 -c Release sample/Sample/Sample.csproj