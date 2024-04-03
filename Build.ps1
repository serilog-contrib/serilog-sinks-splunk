Write-Output "build: Build started"

Push-Location $PSScriptRoot

Write-Output "build: Tool versions follow"

dotnet --version
dotnet --list-sdks

if(Test-Path .\artifacts) {
	Write-Output "build: Cleaning ./artifacts"
	Remove-Item ./artifacts -Force -Recurse
}

& dotnet restore .\serilog-sinks-splunk.sln --no-cache

$branch = $NULL -ne $env:CI_TARGET_BRANCH ?  $env:CI_TARGET_BRANCH : (git symbolic-ref --short -q HEAD)
$revision = $NULL -ne $env:CI_BUILD_NUMBER ? "{0:00000}" -f [Convert]::ToInt32("0" + $env:CI_BUILD_NUMBER, 10) : "local"

# add a suffix if this is not a tag build
$suffix = $NULL -ne $env:CI_COMMIT_TAG ? "" : "$($branch.Substring(0, [Math]::Min(10,$branch.Length)) -replace '([^a-zA-Z0-9\-]*)', '')-$revision"
$prefix = $NULL -ne $env:CI_COMMIT_TAG ? $env:CI_COMMIT_TAG : $NULL

Write-Output $brach
Write-Output $revision
Write-Output $suffix

Write-Output "build: Package version suffix is $suffix"


foreach ($src in Get-ChildItem src/*) {
    Push-Location $src

	Write-Output "build: Packaging project in $src"

    if ($prefix) {
        # release build
        & dotnet pack -c Release -o ../../artifacts /p:VersionPrefix="$prefix"
    } elseif ($suffix) {
        # prerelease build
        & dotnet pack -c Release -o ../../artifacts /p:VersionSuffix="$suffix"
    } else {
        # local build
        & dotnet pack -c Release -o ../../artifacts
    }
    if($LASTEXITCODE -ne 0) { throw "Packaging failed" }

    Pop-Location
}

Write-Output "build: Checking complete solution builds"
& dotnet build .\serilog-sinks-splunk.sln -c Release
if($LASTEXITCODE -ne 0) { throw "Solution build failed" }


foreach ($test in Get-ChildItem test/*.Tests) {
    Push-Location $test

	Write-Output "build: Testing project in $test"

    & dotnet test -c Release
    if($LASTEXITCODE -ne 0) { throw "Testing failed" }

    Pop-Location
}

Pop-Location

if ($env:NUGET_API_KEY `
    -and ($NULL -ne $suffix -or $NULL -ne $prefix) `
    -and ($env:CI_TARGET_BRANCH -eq "dev" -or $env:CI_TARGET_BRANCH -eq "master")) {
    # GitHub Actions will only supply this to branch builds and not PRs. We publish
    # builds from any branch this action targets (i.e. master and dev).

    Write-Output "build: Publishing NuGet packages"

    foreach ($nupkg in Get-ChildItem artifacts/*.nupkg) {
        Write-Output "build: Publishing $nupkg"
        & dotnet nuget push -k $env:NUGET_API_KEY -s https://api.nuget.org/v3/index.json "$nupkg" --no-symbols
        if($LASTEXITCODE -ne 0) { throw "Publishing failed" }
    }
}