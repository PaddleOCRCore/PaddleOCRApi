@echo off
REM ========================================
REM 停止 OCRCoreService 服务
REM ========================================

chcp 65001 >nul
set CURRENT_DIR=%~dp0

echo 正在停止 OCRCoreService...

REM 查找并停止运行中的 OCRCoreService 进程
for /f "tokens=2" %%i in ('tasklist ^| findstr "OCRCoreService.dll"') do (
    echo 找到进程 PID: %%i
    taskkill /PID %%i /F
)

REM 记录停止时间
if exist "%CURRENT_DIR%logs\service_startup.log" (
    echo [%date% %time%] Service stopped >> "%CURRENT_DIR%logs\service_startup.log"
)

echo OCRCoreService 已停止
timeout /t 2 /nobreak >nul
