# Pipeline Build Da Nen Tang DragonBoy TriHienKun (PC, Android, iOS)
$ErrorActionPreference = "Stop"

Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "  PIPELINE BUILD DA NEN TANG - DRAGONBOY TRIHIENKUN (.NET 8 NATIVE)   " -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan

# 1. Build PC Native (.NET 8 NativeAOT)
Write-Host "`n>>> [1/3] DANG BIEN DICH PC NATIVE (.NET 8 NATIVE AOT)..." -ForegroundColor Yellow
Push-Location "C:\ModNRO\DragonBoy_Net8_Native"
try {
    dotnet publish -c Release -r win-x64 -o "bin\Release\net8.0\win-x64\publish"
    if ($LASTEXITCODE -ne 0) { throw "Build PC Native that bai!" }
    $pcSrc = "C:\ModNRO\DragonBoy_Net8_Native\bin\Release\net8.0\win-x64\publish\DragonBoy_Net8_Native.exe"
    $pcDst = "C:\Users\PhamTriHien\Desktop\DragonBoy_Net8_Native.exe"
    if (Test-Path $pcSrc) {
        Copy-Item -Path $pcSrc -Destination $pcDst -Force
        Write-Host "  + Da dong bo PC Native ra Desktop: $pcDst" -ForegroundColor Green
    }
} finally {
    Pop-Location
}

# 2. Build Android APK
Write-Host "`n>>> [2/3] DANG BIEN DICH ANDROID APK (ARM64 / ARMv7 / x86_64)..." -ForegroundColor Yellow
python "C:\ModNRO\01_Android_Builds\build_android_native.py"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Bien dich Android APK that bai!"
    exit 1
}

# 3. Build iOS IPA
Write-Host "`n>>> [3/3] DANG DONG GOI IOS IPA (RETINA HD / CODERESOURCES SHA-1/256)..." -ForegroundColor Yellow
python "C:\ModNRO\02_iOS_Builds\build_ios_native.py"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Dong goi iOS IPA that bai!"
    exit 1
}

Write-Host "`n======================================================================" -ForegroundColor Green
Write-Host "HOAN TAT PIPELINE BUILD TOAN BO 3 NEN TANG THANH CONG 100%!" -ForegroundColor Green
Write-Host "  1. PC Native (.exe)  : C:\Users\PhamTriHien\Desktop\DragonBoy_Net8_Native.exe" -ForegroundColor White
Write-Host "  2. Android APK (.apk): C:\Users\PhamTriHien\Desktop\DragonBoy250_Mod_Android.apk" -ForegroundColor White
Write-Host "  3. iOS IPA (.ipa)    : C:\Users\PhamTriHien\Desktop\DragonBoy_Mod_iOS.ipa" -ForegroundColor White
Write-Host "======================================================================" -ForegroundColor Green
