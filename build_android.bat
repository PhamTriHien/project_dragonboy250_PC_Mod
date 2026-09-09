@echo off
chcp 65001 >nul
powershell -ExecutionPolicy Bypass -File "%~dp0build_android.ps1"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [LỖI] Quá trình build thất bại!
    pause
    exit /b %ERRORLEVEL%
)
pause
