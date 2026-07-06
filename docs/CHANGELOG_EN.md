# 📝 Changelog
v4.5.3 2026.7.6
✅ ‌PaddleOCR.dll‌: Optimized the accuracy of layout recognition and table recognition, and adjusted the C++ interface for layout recognition initialization.
✅ ‌WinFormsApp‌: Added the option to enable or disable layout recognition, and optimized the model download program.
✅ ‌OCRCoreService‌: Optimized the style of the online Demo, and separated JavaScript and CSS files.
✅ ‌NuGet Package Release‌: PaddleOCRRuntime_x64 v4.5.3 is released, with C++ dependencies moved to runtimes\win-x64\native.
✅ ‌NuGet Package Release‌: PaddleOCRSDK v4.5.3 is released, which updates the C++ interface methods and adds the NativeRuntimeLoader for loading native dependencies.

v4.5.2 2026.6.17
- ✅ ‌PaddleOCR.dll‌: Optimized the multi-instance multi-threading mechanism. The image-to-image search interface now supports Byte and Mat formats, and is compatible with the PP-OCRv6 model.
- ✅ ‌WinFormsApp‌: Added support for the PP-OCRv6 model and the PaddleOCR-VL1.6 model.
- ✅ ‌NuGet Package Released‌: PaddleOCRRuntime_x64 v4.5.2 is released, which includes the PP-OCRv6 model by default. For other models, run PaddleOCRModelsDownloader.exe to download them.
- ✅ ‌NuGet Package Released‌: PaddleOCRSDK v4.5.2 is released, which is adapted to the PP-OCRv6 model and the image-to-image search interface.

v4.5.0 2026.5.26
- ✅ PaddleOCR.dll: Optimized layout detection and recognition, improved model loading compatibility, and added GPU licensing APIs.
- ✅ llamaocr-vl.dll: Optimized layout detection and recognition, improved model loading compatibility, and added GPU licensing APIs.
- ✅ WinFormsApp: Added a menu bar, moved some features into the menu, and changed PDF recognition to use the layout recognition API.
- ✅ NuGet Package Released: PaddleOCRRuntime_x64 v4.5.1, including Paddle 3.4.0 CPU inference libraries, PaddleOCR.dll, llamaocr-vl.dll, and all dependencies.

## v4.4.0 `2026.5.11`
- ✅ **PaddleOCR.dll**: Optimized layout analysis; improved GPU dependency detection to avoid initialization pop-ups.
- ✅ **PaddleOCR.dll**: Formula recognition now supports PP-FormulaNet series models; added document image layout sub-module detection; fixed layout sub-module recognition errors and layout analysis result sorting issues.
- ✅ **PaddleOCR.dll**: Removed `use_chart_recognition` and `chart_model_dir` parameters, as this feature depends on PaddleX and is not encapsulated for now.
- ✅ **llamaocr-vl.dll**: Optimized memory recycling mechanism; improved exam paper recognition and layout analysis accuracy, aligned with PaddleX.
- ✅ **Code Optimization**: Released PaddleOCRSDK v4.4.0, aligned OCRSDK.cs and OCRVLSDK.cs with new interfaces.
- ✅ **OCRCoreService**: Added web-based online OCR demo, supporting PaddleOCRv5, PP-StructureV3, and PaddleOCR-VL1.5, with Markdown display support.
- ✅ **NuGet Package Release**: Released PaddleOCRRuntime_x64 v4.4.0, including paddle 3.4.0 CPU inference library, PaddleOCR.dll, llamaocr-vl.dll, and all dependencies.

## v4.3.0 `2026.4.29`
- ✅ **Major Update**: Added PP-Structure document layout analysis module to PaddleOCR.dll, supporting comprehensive recognition of 20 document element types (layout detection, table recognition, formula recognition, seal recognition, chart-to-table conversion, etc.)
- ✅ **New Interfaces**: Added `InitStructure`/`InitStructurejson` initialization interfaces to PaddleOCR.dll, supporting 12 model path parameters (text detection, orientation classification, text recognition, layout analysis, table, formula, seal, chart, document orientation, document rectification, region detection); removed the original table recognition interface
- ✅ **New Interfaces**: Added four layout analysis interfaces: `DetectLayout`/`DetectLayoutMat`/`DetectLayoutByte`/`DetectLayoutBase64`, supporting File/Mat/Byte/Base64 input methods
- ✅ **New Interface**: Added `DetectScreenShot` interface to standard OCR recognition, specifically designed for memory screenshot scenarios
- ✅ **Architecture Optimization**: Optimized VL vision-language model `llamaocr-vl.dll`, now also supporting layout detection, table recognition, formula recognition, seal recognition, chart-to-table conversion, enhancing modularity
- ✅ **Documentation Update**: Comprehensively updated `PaddleOCR.dll接口清单.md`, adding detailed parameter descriptions and usage notes for all interfaces
- ✅ **Code Optimization**: Released PaddleOCRSDK v4.3.0; aligned OCRSDK.cs, UVDocSDK.cs, and OCRVLSDK.cs with new interfaces
- ✅ **NuGet Package Release**: Released PaddleOCRRuntime_x64 v4.3.1, including paddle 3.3.0 CPU inference library, PaddleOCR.dll, llamaocr-vl.dll, and all dependencies
- ⚠️ **Important Note**: PP-Structure requires downloading related models separately (PP-DocLayoutV3_infer, SLANet_plus, formula/seal/chart models, etc.)

## v4.2.0 `2026.4.13`
- ✅ Added OCR-VL vision-language recognition module powered by the Llama inference engine (llamaocr-vl.dll). Supports mainstream VLM-based OCR models including PaddleOCR-VL-1.5-GGUF, DeepSeek-OCR-GGUF, Qwen2-VL-OCR-GGUF, and FireRed-OCR-GGUF. More GGUF-format models available at: https://www.modelscope.cn/models?name=OCR%20GGUF
- ✅ Added `OCRVLServiceController` to OCRCoreService, providing four WebAPI endpoints: `GetOCRVL` (general OCR + Base64), `GetOCRVLFile` (general OCR + file upload), `GetDOCVL` (layout analysis + Base64), `GetDOCVLFile` (layout analysis + file upload).
- ✅ Added `OCRVLConfig` configuration section in `appsettings.json` for enabling the service and specifying the yaml model path.
- ✅ WinFormsApp Demo added OCR-VL recognition UI, supporting general recognition with custom prompts and document layout analysis.
- ✅ Released NuGet package PaddleOCRSDK v4.2.0, aligned with the latest interfaces.
- ✅ Released NuGet package PaddleOCRRuntime_x64 v4.2.0, including the paddle 3.3.0 CPU inference library, PaddleOCR.dll, llamaocr-vl.dll, and all dependencies.

v4.1.1 2026.2.3
- ✅ Optimized PaddleOCR.dll: added a multi-instance PaddleOCR recognition engine with a configurable number of instances, suitable for high-concurrency scenarios; optimized memory usage; fixed an issue where recognition results could occasionally be empty; added support for passing screenshots directly as images for recognition.
- ✅ Released the NuGet package PaddleocrSDK v4.1.1. .NET projects can reference this package directly, or copy and modify the open-source project as needed.
- ✅ Released the NuGet package PaddleOCRRuntime_x64 v4.1.1, which includes the Paddle 3.3.0 CPU inference library, PaddleOCR.dll, and all required dependencies.
- ✅ Added a ConsoleSharp console application demo with a simple example demonstrating how to call OCR.

v4.0.1 2026.1.22
- ✅ Upgraded the project to VS 2026 + .NET 10
- ✅ Optimized PaddleOCR.dll and added image-to-image search functionality
- ✅ Released the NuGet package PaddleocrSDK v4.0.1 — .NET projects can reference this package directly; it already includes PaddleOCRRuntime_x64
- ✅ Released the NuGet package PaddleOCRRuntime_x64 v4.0.1, which includes the Paddle 3.2.2 inference library, PaddleOCR.dll, and all required dependencies.

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
