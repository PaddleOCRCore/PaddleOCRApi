@echo off
set CURRENT_DIR=%~dp0
echo StartingOCRCoreService.dll..
dotnet "%CURRENT_DIR%OCRCoreService.dll" --urls http://*:5000
pause