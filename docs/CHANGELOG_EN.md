# 📝 Changelog

v4.1.1 2026.2.3
- ✅ Optimized PaddleOCR.dll: added a multi-instance PaddleOCR recognition engine with a configurable number of instances, suitable for high-concurrency scenarios; optimized memory usage; fixed an issue where recognition results could occasionally be empty; added support for passing screenshots directly as images for recognition.
- ✅ Released the NuGet package PaddleocrSDK v4.1.1. .NET projects can reference this package directly, or copy and modify the open-source project as needed.
- ✅ Released the NuGet package PaddleOCRRuntime_x64 v4.1.1, which includes the Paddle 3.3.0 CPU inference library, PaddleOCR.dll, and all required dependencies.
- ✅ Added a ConsoleSharp console application demo with a simple example demonstrating how to call OCR.

v4.0.1 2026.1.22
- ✅ Upgraded the project to VS 2026 + .NET 10
- ✅ Optimized PaddleOCR.dll and added image-to-image search functionality
- ✅ Released the NuGet package PaddleocrSDK v4.0.1 — .NET projects can reference this package directly; it already includes PaddleOCRRuntime_x64
- ✅ Released the NuGet package PaddleOCRRuntime_x64 v4.0.1, which includes the Paddle 3.2.2 inference library, PaddleOCR.dll, and all required dependencies

## v4.0.0 2026.1.17
- ✅ Optimized PaddleOCR.dll by integrating the UVDOC text image rectification feature, adding per-character (single-word) bounding box output controlled by the return_word_box parameter, and removing the Keys parameter from Init.
- ✅ Released PaddleOCRRuntime_x64 v4.0.0, including the Paddle 3.2.2 inference library, PaddleOCR.dll, and all required dependencies. The table recognition model has been upgraded to PP-SLANet_plus_infer.
- ✅ PaddleocrSDK v4.0.0 is aligned with PaddleOCR.dll. The WinFormsApp now supports optional per-character bounding box generation and integrates the text image rectification feature. Sample code for C++, Python, Go, and Java has been updated.

## v3.3.0 2026.1.11
- ✅ Optimized PaddleOCR.dll: C++ pointers are now allocated using CoTaskMemAlloc and released in C# via Marshal.FreeCoTaskMem, fixing the exception in the DetectTableByte interface.
- ✅ Released PaddleOCRRuntime_x64 v3.3.0, including the paddle 3.2.2 inference library, PaddleOCR.dll, and all dependencies; added the UVDoc_infer model.
- ✅ PaddleocrSDK v3.3.0 is aligned with PaddleOCR.dll, adds UVDoc text image rectification, and integrates this functionality into the WebApi; the demo now includes PaddleVisionWinform.

## v3.2.2 `2025.12.11`
- ✅ Optimized PaddleOCR.dll, supports paddle_inference 3.2.2 inference library
- ✅ Released PaddleOCRRuntime_x64 v3.2.2, includes paddle 3.2.2 inference library, PaddleOCR.dll and dependencies
- ⚠️ NuGet PaddleOCRSDK stops updating, core files integrated into PaddleOCRRuntime_x64, .NET projects refer to PaddleOCRSDK source code

## v3.1.0 `2025.9.15`
- ✅ Optimized PaddleOCR.dll, supports paddle_inference 3.2.0 inference library
- ✅ Added support for text line orientation classification model PP-LCNet_x1_0_textline_ori
- ✅ V4/V5 models use yml format
- ✅ Table recognition initialization adds orientation classification model parameter, can use table recognition independently
- ✅ Released PaddleOCRRuntime_x64 v3.1.1
- ✅ Released PaddleOCRSDK v3.1.0, aligned with PaddleOCR.dll

## v2.1.1 `2025.8.1`
- ✅ Released PaddleOCRSDK 2.1.1, added DetectMat interface

## v2.1.0 `2025.7.31`
- ✅ Modified PaddleOCR.dll interface, pointer type changed to char* (UTF8 encoding)
- ✅ Added DetectMat interface supporting direct Mat input
- ✅ EnableANSIResult renamed to EnableASCIIResult
- ✅ Released PaddleOCRSDK 2.1.0

## v2.0.0 `2025.6.4`
- ✅ Modified PaddleOCR.dll interface, added support for PP-OCRv5 model
- ✅ WinForm Demo added V5/V4 model selection dropdown

## v1.0.5 `2025.4.1`
- ✅ Optimized PaddleOCR.dll interface, Demo added table recognition feature

## v1.0.4 `2025.3.29`
- ✅ Optimized PaddleOCR.dll, added log output switch, OCR recognition speed improvement
- ✅ WebApi interface optimization, added OCR initialization and parameter settings

## v1.0.2 `2025.3.23`
- ✅ Optimized PaddleOCR.dll, added multi-thread queue support
- ✅ Added automatic memory recycling when limit reached
- ✅ WinFormDemo feature enhancement, added initialization options
- ✅ Added multi-image selection and concurrent testing simulation

## v1.0.1 `2025.3.5`
- ✅ Optimized PaddleOCR.dll, improved recognition speed, added smart pointers

## v1.0 `2025.1.22`
- 🎉 Initial release: PaddleOCRApi
