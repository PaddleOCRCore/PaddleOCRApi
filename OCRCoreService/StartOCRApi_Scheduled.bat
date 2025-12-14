@echo off
REM ========================================
REM OCRCoreService 计划任务启动脚本
REM 适用于Windows计划任务自动启动
REM ========================================

chcp 65001 >nul
set CURRENT_DIR=%~dp0
cd /d "%CURRENT_DIR%"

REM 设置环境变量
set DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
set ASPNETCORE_ENVIRONMENT=Production
set COMPlus_EnableDiagnostics=0

REM 创建日志目录
if not exist "%CURRENT_DIR%logs" mkdir "%CURRENT_DIR%logs"

REM 记录启动时间
echo ================================================ >> "%CURRENT_DIR%logs\service_startup.log"
echo [%date% %time%] OCRCoreService Starting... >> "%CURRENT_DIR%logs\service_startup.log"
echo Working Directory: %CURRENT_DIR% >> "%CURRENT_DIR%logs\service_startup.log"
echo ================================================ >> "%CURRENT_DIR%logs\service_startup.log"

REM 启动服务（后台运行，输出重定向到日志）
start "" /B dotnet "%CURRENT_DIR%OCRCoreService.dll" --urls http://*:5000 >> "%CURRENT_DIR%logs\service_output.log" 2>&1

REM 等待服务启动
timeout /t 3 /nobreak >nul

REM 检查服务是否启动成功
tasklist | find "dotnet.exe" >nul
if %errorlevel% == 0 (
    echo [%date% %time%] Service started successfully >> "%CURRENT_DIR%logs\service_startup.log"
) else (
    echo [%date% %time%] ERROR: Service failed to start >> "%CURRENT_DIR%logs\service_startup.log"
)

exit
