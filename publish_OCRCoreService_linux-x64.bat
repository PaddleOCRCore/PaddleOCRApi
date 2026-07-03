@echo off
setlocal

chcp 65001 >nul

set "ROOT_DIR=%~dp0"
set "RID=linux-x64"
set "CONFIG=Release"
set "FRAMEWORK=net10.0"
set "SDK_PROJECT=%ROOT_DIR%PaddleOCRSDK\PaddleOCRSDK.csproj"
set "SERVICE_PROJECT=%ROOT_DIR%OCRCoreService\OCRCoreService.csproj"
set "PUBLISH_DIR=%ROOT_DIR%Publish\%RID%"

echo ========================================
echo Publish OCRCoreService for %RID%
echo Root: %ROOT_DIR%
echo Output: %PUBLISH_DIR%
echo ========================================
echo.
echo NOTE: This script does not add PaddleOCRRuntime_linux-x64.
echo       Linux native PaddleOCR .so files are not handled here yet.
echo.

where dotnet >nul 2>nul
if errorlevel 1 (
    echo ERROR: dotnet CLI was not found in PATH.
    if not defined CI pause
    exit /b 1
)

if not exist "%SDK_PROJECT%" (
    echo ERROR: SDK project not found: %SDK_PROJECT%
    if not defined CI pause
    exit /b 1
)

if not exist "%SERVICE_PROJECT%" (
    echo ERROR: OCRCoreService project not found: %SERVICE_PROJECT%
    if not defined CI pause
    exit /b 1
)

echo Restoring %RID% assets...
dotnet restore "%SERVICE_PROJECT%" -r %RID%
if errorlevel 1 goto :fail

echo.
echo Cleaning PaddleOCRSDK...
dotnet clean "%SDK_PROJECT%" -c %CONFIG% -f %FRAMEWORK%
if errorlevel 1 goto :fail

echo.
echo Cleaning OCRCoreService...
dotnet clean "%SERVICE_PROJECT%" -c %CONFIG% -f %FRAMEWORK% -r %RID%
if errorlevel 1 goto :fail

echo.
echo Removing old publish directory...
if exist "%PUBLISH_DIR%" rmdir /s /q "%PUBLISH_DIR%"
if errorlevel 1 goto :fail

echo.
echo Publishing OCRCoreService...
dotnet publish "%SERVICE_PROJECT%" -c %CONFIG% -f %FRAMEWORK% -r %RID% --self-contained false -p:PublishDir="%PUBLISH_DIR%\"
if errorlevel 1 goto :fail

echo.
echo SUCCESS: OCRCoreService %RID% published to:
echo %PUBLISH_DIR%
if not defined CI pause
exit /b 0

:fail
echo.
echo ERROR: Publish failed. ExitCode=%ERRORLEVEL%
if not defined CI pause
exit /b %ERRORLEVEL%
