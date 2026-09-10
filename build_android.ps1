# Pipeline Build Android Mod Native TriHienKun (Doc Lap 100%)
$ErrorActionPreference = "Stop"

$scriptPath = "C:\ModNRO\01_Android_Builds\build_android_native.py"
if (-not (Test-Path $scriptPath)) {
    $scriptPath = Join-Path $PSScriptRoot "build_android_native.py"
}

python $scriptPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Bien dich Android Mod Native that bai!"
    exit 1
}
