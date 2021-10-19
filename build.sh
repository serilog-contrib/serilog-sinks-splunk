#!/bin/bash
set -e 
dotnet --info
dotnet restore

# Until # 65 is addressed build only core package.  When available move to netstandard for all packages
for path in src/**/Serilog.Sinks.Splunk.csproj; do
    dotnet build -f netstandard2.0 -c Release ${path}
    dotnet build -f netstandard2.1 -c Release ${path}
done

for path in test/*.Tests/*.csproj; do
    dotnet test -f net5.0  -c Release ${path}
done

dotnet build -f net5.0 -c Release sample/Sample/Sample.csproj