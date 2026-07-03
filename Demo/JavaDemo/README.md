# PaddleOCR Java Demo

This demo calls the native PaddleOCR library through JNA.

## Native Library Layout

`OCRJavaDemo.java` resolves the native library automatically by platform:

- Windows x64: `runtimes/win-x64/native/PaddleOCR.dll`
- Linux x64: `runtimes/linux-x64/native/PaddleOCR.so`

It also falls back to the current directory, executable directory, and `lib`.

## Dependencies

Put `jna-5.13.0.jar` in this directory, or adjust the classpath in `runJavaDemo.bat`.

Download:
https://repo1.maven.org/maven2/net/java/dev/jna/jna/5.13.0/jna-5.13.0.jar

## Run

```bat
javac -cp jna-5.13.0.jar OCRJavaDemo.java
java -cp ".;jna-5.13.0.jar" OCRJavaDemo
```

The demo expects `models` and `images` under the process working directory.
