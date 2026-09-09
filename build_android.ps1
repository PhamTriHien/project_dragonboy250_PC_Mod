$ErrorActionPreference = "Stop"

$apktool = "C:\ModNRO\ModNRO_Tools\apktool.jar"
$buildTools = "C:\Users\PhamTriHien\AppData\Local\Android\Sdk\build-tools\36.0.0"
$zipalign = Join-Path $buildTools "zipalign.exe"
$apksigner = Join-Path $buildTools "apksigner.bat"
$keystore = "C:\ModNRO\01_Android_Builds\debug.keystore"
if (-not (Test-Path $keystore)) { $keystore = "C:\ModNRO\debug.keystore" }
$sourceDir = "C:\ModNRO\ModNRO_Tools\Decompiled\APK_apktool"
$tempDir = "C:\ModNRO\01_Android_Builds\.temp"
if (-not (Test-Path $tempDir)) { New-Item -ItemType Directory -Path $tempDir -Force | Out-Null }

$outputUnsigned = Join-Path $tempDir "DragonBoy250_Unsigned.apk"
$outputAligned = Join-Path $tempDir "DragonBoy250_Aligned.apk"
$outputSigned = "C:\ModNRO\01_Android_Builds\DragonBoy250_Mod_Android.apk"
$desktopApk = "C:\Users\PhamTriHien\Desktop\DragonBoy250_Mod_Android.apk"

Write-Host "======================================================================" -ForegroundColor Cyan
Write-Host "  DONG GOI BUILD ANDROID APK - DRAGONBOY 250 MOD" -ForegroundColor Cyan
Write-Host "======================================================================" -ForegroundColor Cyan

if (-not (Test-Path $sourceDir)) {
    Write-Error "Source directory not found: $sourceDir"
    exit 1
}

Write-Host "`n[1/4] Bien dich APK voi Apktool..." -ForegroundColor Yellow
& java -jar $apktool b $sourceDir -o $outputUnsigned --use-aapt2
if ($LASTEXITCODE -ne 0) {
    Write-Host "Thu lai voi aapt thuong..." -ForegroundColor Yellow
    & java -jar $apktool b $sourceDir -o $outputUnsigned
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Apktool build that bai!"
        exit 1
    }
}

Write-Host "`n[2/4] Toi uu can chinh 4-byte (zipalign)..." -ForegroundColor Yellow
if (Test-Path $outputAligned) { Remove-Item $outputAligned -Force }
& $zipalign -p -f 4 $outputUnsigned $outputAligned
if ($LASTEXITCODE -ne 0) {
    Write-Error "Zipalign that bai!"
    exit 1
}

Write-Host "`n[3/4] Ky so APK (apksigner v2/v3 scheme)..." -ForegroundColor Yellow
if (Test-Path $outputSigned) { Remove-Item $outputSigned -Force }
& cmd.exe /c $apksigner sign --ks $keystore --ks-pass pass:android --key-pass pass:android --ks-key-alias androiddebugkey --out $outputSigned $outputAligned
if ($LASTEXITCODE -ne 0) {
    Write-Error "Apksigner that bai!"
    exit 1
}

Write-Host "`n[4/4] Xac thuc chu ky APK..." -ForegroundColor Yellow
& cmd.exe /c $apksigner verify --verbose $outputSigned

Write-Host "`nSao chep APK ra Desktop..." -ForegroundColor Green
Copy-Item $outputSigned $desktopApk -Force
if (Test-Path $desktopApk) {
    Write-Host "  + Tep APK da san sang tai: $desktopApk" -ForegroundColor Green
}
Write-Host "  + Ban luu tai pipeline: $outputSigned" -ForegroundColor Green

# Don dep tep tam
if (Test-Path $tempDir) { Remove-Item $tempDir -Recurse -Force }

Write-Host "`nBUILD ANDROID THANH CONG 100%!" -ForegroundColor Green
