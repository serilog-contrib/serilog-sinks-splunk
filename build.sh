#!/bin/bash
dotnet restore
for path in src/*/project.json; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname} -c Release
done 