@echo off
chcp 65001 >nul
setlocal

set SCRIPT=%~dp0build-release.ps1

echo 正在构建便携发布包（C# 自包含 + 内置 Python）...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT%"
if errorlevel 1 (
    echo.
    echo 发布失败。
    pause
    exit /b 1
)

echo.
echo 完成。将 PublishOutput 整个文件夹复制到新电脑即可运行。
pause
