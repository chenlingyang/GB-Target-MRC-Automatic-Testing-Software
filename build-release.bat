@echo off
chcp 65001 >nul
cd /d "%~dp0ImageCaptureApp"
call publish.bat
