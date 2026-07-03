CHCP 65001
cd /d "%~dp0"
go build .\OCRGoDemo.go
if errorlevel 1 (
    pause
    exit /b
)
.\OCRGoDemo.exe
@pause
