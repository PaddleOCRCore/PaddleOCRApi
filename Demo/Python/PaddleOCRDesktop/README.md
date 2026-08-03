# PaddleOCR Desktop

Windows x64 本地桌面程序，直接调用 `PaddleOCR.dll`，提供文本识别和版面识别。程序不调用 WebAPI，不包含 OCR-VL 和文档图像矫正功能。

## 运行源码

```powershell
D:\Python\Python312\python.exe -m paddleocr_desktop
```

## 构建 EXE

运行 `build_exe.bat`。输出目录：

```text
dist\PaddleOCRDesktop\PaddleOCRDesktop.exe
```

构建脚本会装配项目中的完整 Windows 原生运行时、现有模型和模型下载器。当前仓库发布资产包含文本识别模型；版面识别需要先通过“下载 OCR 模型”补齐所选版面模型及其结构化识别依赖，再勾选“启用版面识别”并初始化。