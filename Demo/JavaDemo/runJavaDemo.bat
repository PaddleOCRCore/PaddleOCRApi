@echo off
CHCP 65001
echo 正在启动 Java Demo...

:: 提示：运行此示例需要 JNA 依赖
:: 如果没有配置环境变量，请手动下载 jna.jar 并放入此目录
:: 下载地址: https://repo1.maven.org/maven2/net/java/dev/jna/jna/5.13.0/jna-5.13.0.jar

set JNA_JAR=jna-5.13.0.jar

if not exist %JNA_JAR% (
    echo [警告] 未在当前目录找到 %JNA_JAR%
    echo 请确认您已正确配置 JNA 依赖，或者将 jna.jar 放置在此目录下。
    pause
    exit /b
)

:: 编译
javac -cp %JNA_JAR% OCRJavaDemo.java

:: 运行 (假设 dll 和 models 在根目录或已正确放置)
java -cp ".;%JNA_JAR%" OCRJavaDemo

pause
