@echo off
chcp 65001 >nul
echo ========================================
echo 图像采集卡上位机软件 - 快速运行
echo ========================================
echo.

cd /d "%~dp0ImageCaptureApp"

echo 正在检查 .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [错误] 未找到 .NET SDK，请先安装 .NET 6.0 或更高版本
    echo 下载地址: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [1/3] 还原 NuGet 包...
dotnet restore
if errorlevel 1 (
    echo [错误] NuGet 包还原失败
    pause
    exit /b 1
)

echo [2/3] 编译项目...
dotnet build
if errorlevel 1 (
    echo [错误] 项目编译失败
    pause
    exit /b 1
)

echo [3/3] 运行项目...
echo.
echo ========================================
echo 应用程序即将启动...
echo ========================================
echo.

dotnet run

pause
