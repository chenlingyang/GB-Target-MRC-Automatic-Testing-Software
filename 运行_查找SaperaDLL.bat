@echo off
chcp 65001 >nul
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0??Sapera?DLL.ps1"
echo.
echo Press any key to close...
pause >nul
