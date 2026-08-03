@echo off
setlocal

set "PROJECT_DIR=%~dp0"
set "REPO_ROOT=%PROJECT_DIR%..\..\.."
set "PYTHON=D:\Python\Python312\python.exe"
set "PUBLISH_SOURCE=%REPO_ROOT%\Publish\win-x64"
set "NATIVE_SOURCE=%REPO_ROOT%\packages\PaddleOCRRuntime_x64.4.5.3\runtimes\win-x64\native"
set "PYINSTALLER_EXE=%PROJECT_DIR%dist\PaddleOCRDesktop.exe"
set "DIST_DIR=%PROJECT_DIR%dist\PaddleOCRDesktop"

if not exist "%PYTHON%" (
    echo Python not found: %PYTHON%
    exit /b 1
)
if not exist "%NATIVE_SOURCE%\PaddleOCR.dll" (
    echo PaddleOCR native runtime not found: %NATIVE_SOURCE%
    exit /b 1
)
if not exist "%PUBLISH_SOURCE%\models" (
    echo Models not found: %PUBLISH_SOURCE%\models
    exit /b 1
)

pushd "%PROJECT_DIR%"
"%PYTHON%" -m PyInstaller --noconfirm --clean PaddleOCRDesktop.spec
if errorlevel 1 goto :failed

if exist "%DIST_DIR%" rmdir /S /Q "%DIST_DIR%"
if exist "%DIST_DIR%" (
    echo Unable to clean output directory: %DIST_DIR%
    goto :failed
)
if not exist "%PYINSTALLER_EXE%" (
    echo PyInstaller output not found: %PYINSTALLER_EXE%
    goto :failed
)
mkdir "%DIST_DIR%"
if errorlevel 1 goto :failed
move /Y "%PYINSTALLER_EXE%" "%DIST_DIR%\PaddleOCRDesktop.exe" >nul
if errorlevel 1 goto :failed

robocopy "%NATIVE_SOURCE%" "%DIST_DIR%\runtimes\win-x64\native" /E /NFL /NDL /NJH /NJS /NP
if errorlevel 8 goto :failed
robocopy "%PUBLISH_SOURCE%\models" "%DIST_DIR%\models" /E /NFL /NDL /NJH /NJS /NP
if errorlevel 8 goto :failed

if exist "%PUBLISH_SOURCE%\PaddleOCRModelsDownloader.exe" copy /Y "%PUBLISH_SOURCE%\PaddleOCRModelsDownloader.exe" "%DIST_DIR%\" >nul
if exist "%PUBLISH_SOURCE%\PaddleOCRLicenseCode.exe" copy /Y "%PUBLISH_SOURCE%\PaddleOCRLicenseCode.exe" "%DIST_DIR%\" >nul

echo.
echo Build succeeded:
echo %DIST_DIR%\PaddleOCRDesktop.exe
popd
exit /b 0

:failed
echo.
echo Build failed with exit code %errorlevel%.
popd
exit /b 1