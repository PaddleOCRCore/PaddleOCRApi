# PaddleOCRVLSDK

PaddleOCRVLSDK 是一个面向 .NET 的轻量封装，用于调用 llamaocr-vl.dll 提供的视觉语言 OCR 能力。

## 功能

- 初始化通用 VL OCR 引擎
- 基于提示词执行图片问答与文字识别
- 支持文件路径、字节数组、Base64、OpenCV Mat 四种输入方式
- 初始化文档分析引擎
- 支持 Markdown、JSON 或混合输出

## 快速开始

```csharp
using PaddleOCRVLSDK;

var service = new OCRVLService();
service.Init("configs/Qwen3VL-2B.yaml");

var chatResult = service.Chat("请提取图片中的全部文字", "images/sample.png");
Console.WriteLine(chatResult.Content);

service.InitDoc("configs/PaddleOCRVL.yaml");
var docResult = service.DocChat("images/doc.png", PocrOutputFormat.Both);
Console.WriteLine(docResult.Markdown);
Console.WriteLine(docResult.JsonText);
```

## 运行要求

- Windows x64
- llamaocr-vl.dll 及其依赖项位于应用程序可加载路径中
- 对应模型配置文件可被 DLL 正常访问

## 说明

- DLL 导出函数声明参考 pocr_dll_api.h
- 字符串按 UTF-8 进行封送
- Chat 和 DocChat 返回的非托管内存会在 SDK 内部自动释放