@echo off
REM ========================================
REM 运行 OCRCoreService 服务
REM ========================================

@echo off
chcp 65001 >nul
set CURRENT_DIR=%~dp0
cd /d "%CURRENT_DIR%"

REM 启动服务，将输出重定向到日志文件
dotnet "%CURRENT_DIR%OCRCoreService.dll" --urls http://*:5000