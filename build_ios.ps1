# Pipeline Build iOS Mod Native TriHienKun (Doc Lap 100%)
$ErrorActionPreference = "Stop"

$scriptPath = "C:\ModNRO\02_iOS_Builds\build_ios_native.py"
if (-not (Test-Path $scriptPath)) {
    $scriptPath = Join-Path $PSScriptRoot "build_ios_native.py"
}

python $scriptPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Bien dich iOS Mod Native that bai!"
    exit 1
}
