@echo off
chcp 65001 >nul
python "%~dp002_iOS_Builds\build_ios.py"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [LỖI] Quá trình build iOS IPA thất bại!
    pause
    exit /b %ERRORLEVEL%
)
pause
