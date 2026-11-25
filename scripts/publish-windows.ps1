$ErrorActionPreference = "Stop"

if ($env:OS -ne "Windows_NT") {
    throw "AutoCue's self-contained Windows executable must be published on Windows."
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$outputPath = Join-Path $repositoryRoot "artifacts\windows-x64"

dotnet publish (Join-Path $repositoryRoot "AutoCue.csproj") `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -o $outputPath

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE."
}

$executablePath = Join-Path $outputPath "AutoCue.exe"
if (-not (Test-Path $executablePath)) {
    throw "Publish completed without producing $executablePath."
}

Write-Output "Published standalone executable: $executablePath"
