# PaddleOCR Java Demo

这是一个参考 `GoDemo` 实现的 Java 调用示例。

## 准备工作

1.  **JNA 依赖**:
    本项目使用 [JNA (Java Native Access)](https://github.com/java-native-access/jna) 来调用 C++ 编写的 `PaddleOCR.dll`。
    您需要下载 `jna.jar` (推荐版本 5.13.0+)。
    - [下载 jna-5.13.0.jar](https://repo1.maven.org/maven2/net/java/dev/jna/jna/5.13.0/jna-5.13.0.jar)

2.  **动态库**:
    确保 `PaddleOCR.dll` 及其所有依赖项（如 `paddle_inference.dll`, `common.dll`, `mkldnn.dll` 等）在运行时的 **系统路径 (PATH)** 或 **程序的当前目录** 下。

3.  **模型与图片**:
    本示例默认模型路径在 `../../models` (相对于项目运行目录的 `models` 文件夹)，图片存放在 `../../images` 目录下。请根据实际情况调整 `OCRJavaDemo.java` 中的路径。

## 如何运行

1.  将 `jna-5.13.0.jar` 放入当前目录。
2.  确保 `PaddleOCR.dll` 在当前目录或 PATH 中。
3.  双击 `runJavaDemo.bat` 或在终端运行：
    ```bash
    javac -cp jna-5.13.0.jar OCRJavaDemo.java
    java -cp ".;jna-5.13.0.jar" OCRJavaDemo
    ```

## 导出函数说明

Java 接口通过 `StdCallLibrary` 实现，映射了以下核心函数：
- `Initjson`: 初始化 OCR 引擎。
- `Detect`: 对图片进行识别并返回文字结果。
- `EnableJsonResult`: 控制返回结果是纯文本还是 JSON 格式。
