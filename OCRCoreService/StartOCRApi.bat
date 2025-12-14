@echo off
REM ========================================
REM 运行 OCRCoreService 服务
REM ========================================

@echo off
chcp 65001 >nul
set CURRENT_DIR=%~dp0
cd /d "%CURRENT_DIR%"

REM 设置环境变量以支持非交互式会话
set DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
set ASPNETCORE_ENVIRONMENT=Production

REM 创建日志目录
if not exist "%CURRENT_DIR%logs" mkdir "%CURRENT_DIR%logs"

echo [%date% %time%] Starting OCRCoreService... >> "%CURRENT_DIR%logs\service_startup.log"

REM 启动服务，将输出重定向到日志文件
dotnet "%CURRENT_DIR%OCRCoreService.dll" --urls http://*:5000 >> "%CURRENT_DIR%logs\service_output.log" 2>&1