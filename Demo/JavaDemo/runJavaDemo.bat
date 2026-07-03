@echo off
CHCP 65001
echo Starting Java Demo...

set JNA_JAR=jna-5.13.0.jar

if not exist %JNA_JAR% (
    echo [Warning] %JNA_JAR% was not found in the current directory.
    echo Download it from:
    echo https://repo1.maven.org/maven2/net/java/dev/jna/jna/5.13.0/jna-5.13.0.jar
    pause
    exit /b
)

REM OCRJavaDemo.java resolves native libraries from runtimes\win-x64\native
REM or runtimes\linux-x64\native, then falls back to current/executable/lib.
javac -cp %JNA_JAR% OCRJavaDemo.java
if errorlevel 1 (
    pause
    exit /b
)

java -cp ".;%JNA_JAR%" OCRJavaDemo

pause
