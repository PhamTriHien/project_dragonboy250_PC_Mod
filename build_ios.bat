@echo off
chcp 65001 >nul
python "C:\ModNRO\02_iOS_Builds\build_ios_native.py"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [LOI] Qua trinh build iOS IPA that bai!
    pause
    exit /b %ERRORLEVEL%
)
pause
