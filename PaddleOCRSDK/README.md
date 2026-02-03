# PaddleOCR SDK

基于PaddleOCR深度封装的离线OCR识别SDK，提供简洁易用的API接口，支持C#调用。

<p align="center">
    <a href="../LICENSE"><img src="https://img.shields.io/badge/license-Apache%202-dfd.svg"></a>
    <a href="https://www.nuget.org/packages/PaddleOCRSDK/"><img src="https://img.shields.io/nuget/v/PaddleOCRSDK.svg"></a>
</p>

## 📦 简介

PaddleOCRSDK 是一个高性能的文字识别（OCR）库，基于百度飞浆PaddleOCR深度封装，完全离线运行，支持多线程并发，内存自动回收。

**主要功能：**
- ✅ 文字检测、识别、方向分类
- ✅ 表格结构识别
- ✅ 文本图像几何矫正与透视变换
- ✅ 支持PP-OCRv5/v4全系列模型
- ✅ CPU/GPU推理
- ✅ 支持自定义训练模型

## 🚀 快速开始

### 1. NuGet安装

```bash
Install-Package PaddleOCRRuntime_x64
Install-Package PaddleOCRSDK
```


## 🔧 运行环境

- **.NET**: .NET Core 6.0+
- **操作系统**: Windows x64
- **推理库**: PaddleOCR.dll

## 💻 多语言示例

该项目提供多种编程语言的调用示例：
- **C#**: NuGet包直接调用
- **C++**: 头文件 + DLL方式
- **Python**: 直接调用DLL
- **Java**: JNI调用
- **Go**: CGO调用

## 📄 许可证

Apache License 2.0 - 详见 [LICENSE](../LICENSE)

## 👥 交流反馈

如有问题或建议，欢迎提交Issue或Pull Request。

---

**更多信息**: https://github.com/PaddleOCRCore/PaddleOCRApi
