@echo off
echo Starting PaddleOCRWebApi..
dotnet "./OCRCoreService.dll" --urls http://*:5000
pause