# 📝 更新日志

## v4.5.2 `2026.6.17`
- ✅ **PaddleOCR.dll**: 优化多实例多线程机制，以图找图接口支持Byte及Mat，支持PP-OCRv6模型。
- ✅ **WinFormsApp**: 增加支持PP-OCRv6模型及PaddleOCR-VL1.6模型。
- ✅ **发布NuGet包**: PaddleOCRRuntime_x64发布v4.5.2，默认包含PP-OCRv6模型，其它模型点击PaddleOCRModelsDownloader.exe下载。
- ✅ **发布NuGet包**: PaddleOCRSDK发布v4.5.2,适配PP-OCRv6及以图找图接口。

## v4.5.0 `2026.5.26`
- ✅ **PaddleOCR.dll**: 优化版面检测及识别，优化模型加载适配，增加GPU授权接口
- ✅ **llamaocr-vl.dll**: 优化版面检测及识别，优化模型加载适配，增加GPU授权接口
- ✅ **WinFormsApp**: 增加菜单栏，将部分功能移至菜单，PDF识别改为版面识别接口
- ✅ **发布NuGet包**: PaddleOCRRuntime_x64 v4.5.1，包含 paddle 3.4.0 CPU 推理库、PaddleOCR.dll、llamaocr-vl.dll 及全部依赖。

## v4.4.0 `2026.5.11`
- ✅ **PaddleOCR.dll**: 优化版面分析；优化GPU依赖检测，避免初始化弹窗。
- ✅ **PaddleOCR.dll**: 公式识别新增支持 PP-FormulaNet 系列模型；新增文档图像版面子模块检测；修复版面子模块识别错误和版面分析结果排序问题。
- ✅ **PaddleOCR.dll**: 删除 `use_chart_recognition` 及 `chart_model_dir` 参数，此功能依赖PaddleX，暂不封装。
- ✅ **llamaocr-vl.dll**: 优化内存回收机制；优化试卷识别和版面分析精度，对齐PaddleX。
- ✅ **代码优化**: PaddleOCRSDK发布 v4.4.0,OCRSDK.cs、OCRVLSDK.cs对齐新接口
- ✅ **OCRCoreService**: 增加网页版在线OCR识别Demo,支持PaddleOCRv5、PP-StructureV3、PaddleOCR-VL1.5，支持MarkDown显示
- ✅ **发布NuGet包**: PaddleOCRRuntime_x64 v4.4.0，包含 paddle 3.4.0 CPU 推理库、PaddleOCR.dll、llamaocr-vl.dll 及全部依赖。

## v4.3.0 `2026.4.29`
- ✅ **重大更新**: PaddleOCR.dll新增PP-Structure版面结构识别模块，支持20类文档元素综合识别（版面检测、表格识别、公式识别、印章识别、图表转表等）
- ✅ **新增接口**: PaddleOCR.dll新增`InitStructure`/`InitStructurejson`初始化接口，支持12个模型路径参数（文本检测、方向分类、文本识别、版面分析、表格、公式、印章、图表、文档方向、文档矫正、区域检测）,删除原有表格识别接口
- ✅ **新增接口**: 新增`DetectLayout`/`DetectLayoutMat`/`DetectLayoutByte`/`DetectLayoutBase64`四个版面分析接口，支持File/Mat/Byte/Base64四种输入方式
- ✅ **新增接口**: OCR标准识别新增`DetectScreenShot`接口，专门用于内存截图场景的OCR识别
- ✅ **架构优化**: 优化VL视觉语言模型`llamaocr-vl.dll`，同时支持版面检测、表格识别、公式识别、印章识别等，提升模块化程度
- ✅ **文档更新**: 全面更新`PaddleOCR.dll接口清单.md`，补充所有接口的详细参数说明和使用注意事项
- ✅ **代码优化**: PaddleOCRSDK发布 v4.3.0,OCRSDK.cs、UVDocSDK.cs、OCRVLSDK.cs三个SDK文件对齐新接口
- ✅ **发布NuGet包**: PaddleOCRRuntime_x64 v4.3.1，包含 paddle 3.3.0 CPU 推理库、PaddleOCR.dll、llamaocr-vl.dll 及全部依赖。
- ⚠️ **重要提示**: PP-Structure需要自行下载相关模型（PP-DocLayoutV3_infer、SLANet_plus、公式/印章/图表模型等）

## v4.2.0 `2026.4.13`
- ✅ 新增 OCR-VL 视觉语言识别模块，基于 Llama 推理引擎（llamaocr-vl.dll），支持 PaddleOCR-VL-1.5-GGUF、DeepSeek-OCR-GGUF、Qwen2-VL-OCR-GGUF、FireRed-OCR-GGUF 等主流视觉语言 OCR 模型，更多 GGUF 格式模型可访问：https://www.modelscope.cn/models?name=OCR%20GGUF
- ✅ OCRCoreService 新增 `OCRVLServiceController`，提供四个 WebAPI 接口：`GetOCRVL`（通用识别 + Base64）、`GetOCRVLFile`（通用识别 + 文件）、`GetDOCVL`（版面分析 + Base64）、`GetDOCVLFile`（版面分析 + 文件）。
- ✅ `appsettings.json` 新增 `OCRVLConfig` 配置节，支持配置是否启用服务及 yaml 模型路径。
- ✅ WinFormsApp Demo 新增 OCR-VL 识别界面，支持指定提示词进行通用识别和版面分析识别。
- ✅ 发布 NuGet 包 PaddleOCRSDK v4.2.0，对齐最新接口。
- ✅ 发布 NuGet 包 PaddleOCRRuntime_x64 v4.2.0，包含 paddle 3.3.0 CPU 推理库、PaddleOCR.dll、llamaocr-vl.dll 及全部依赖。
- ✅ OCR-VL识别请自行下载PP-DocLayoutV3_infer及GGUF模型

## v4.1.1 `2026.2.3`
- ✅ 优化PaddleOCR.dll，增加PaddleOCR多实例识别引擎，数量自定义，适合高并发情况使用，优化内存，修复有时识别为空问题，增加截图传入图片识别。
- ✅ 发布Nuget引用包PaddleocrSDK v4.1.1，.net项目可直接引用此包，也可Copy开源项目自行修改。
- ✅ 发布Nuget引用包PaddleOCRRuntime_x64 v4.1.1，包含paddle3.3.0 CPU推理库、PaddleOCR.dll及依赖文件
- ✅ Demo增加ConsoleSharp控制台应用，简单调用OCR示例。

## v4.0.1 `2026.1.22`
- ✅ 升级项目为VS2026+.net10
- ✅ 优化PaddleOCR.dll，增加以图找图功能
- ✅ 发布Nuget引用包PaddleocrSDK v4.0.1，.net项目可直接引用此包，已包含PaddleOCRRuntime_x64
- ✅ 发布Nuget引用包PaddleOCRRuntime_x64 v4.0.1，包含paddle3.2.2推理库、PaddleOCR.dll及依赖文件

## v4.0.0 `2026.1.17`
- ✅ 优化PaddleOCR.dll，集成UVDOC文本图像矫正功能，增加单字坐标输出，由return_word_box参数控制,Init去掉Keys参数
- ✅ 发布PaddleOCRRuntime_x64 v4.0.0，包含paddle3.2.2推理库、PaddleOCR.dll及依赖文件，表格识别模型升级为PP-SLANet_plus_infer
- ✅ PaddleocrSDK v4.0.0对齐PaddleOCR.dll，WinFormsApp增加是否生成单字坐标功能，集成文本图像矫正功能。更新CPP、Python、GO、Java示例代码。

## v3.3.0 `2026.1.11`
- ✅ 优化PaddleOCR.dll，C++指针采用CoTaskMemAlloc分配，C#使用Marshal.FreeCoTaskMem释放，修复DetectTableByte接口异常
- ✅ 发布PaddleOCRRuntime_x64 v3.3.0，包含paddle3.2.2推理库、PaddleOCR.dll及依赖文件，增加UVDoc_infer模型
- ✅ PaddleocrSDK v3.3.0对齐PaddleOCR.dll，增加UVDoc文本图像矫正功能，并集成到WebApi中，Demo增加PaddleVisionWinform。

## v3.2.2 `2025.12.11`
- ✅ 优化PaddleOCR.dll，支持paddle_inference3.2.2推理库
- ✅ 发布PaddleOCRRuntime_x64 v3.2.2，包含paddle3.2.2推理库、PaddleOCR.dll及依赖文件
- ⚠️ Nuget PaddleOCRSDK停止更新，核心文件已整合到PaddleOCRRuntime_x64中，.net项目请参考PaddleOCRSDK源码

## v3.1.0 `2025.9.15`
- ✅ 优化PaddleOCR.dll，支持paddle_inference3.2.0推理库
- ✅ 增加支持文本行方向分类模型PP-LCNet_x1_0_textline_ori
- ✅ v4/v5模型采用yml格式
- ✅ 表格识别初始化增加方向分类模型参数，可单独使用表格识别功能
- ✅ 发布PaddleOCRRuntime_x64 v3.1.1
- ✅ 发布PaddleOCRSDK v3.1.0，对齐PaddleOCR.dll

## v2.1.1 `2025.8.1`
- ✅ 发布PaddleOCRSDK2.1.1版本，增加DetectMat接口

## v2.1.0 `2025.7.31`
- ✅ 修改PaddleOCR.dll接口，指针类型改为char*(UTF8编码)
- ✅ 增加DetectMat接口支持直接传入Mat
- ✅ EnableANSIResult更名为EnableASCIIResult
- ✅ 发布PaddleOCRSDK2.1.0版本

## v2.0.0 `2025.6.4`
- ✅ 修改PaddleOCR.dll接口，增加支持PP-OCRv5模型
- ✅ WinForm Demo增加V5/V4模型选择下拉选项

## v1.0.5 `2025.4.1`
- ✅ 优化PaddleOCR.dll接口，Demo增加表格识别功能

## v1.0.4 `2025.3.29`
- ✅ 优化PaddleOCR.dll，增加日志输出开关，OCR识别提速
- ✅ WebApi接口优化，增加OCR初始化及参数设置

## v1.0.2 `2025.3.23`
- ✅ 优化PaddleOCR.dll，增加多线程队列支持
- ✅ 增加内存达到上限自动回收
- ✅ WinFormDemo功能强化，增加初始化选项
- ✅ 增加多图选择及模拟并发测试

## v1.0.1 `2025.3.5`
- ✅ 优化PaddleOCR.dll，提高识别速度，增加智能指针

## v1.0 `2025.1.22`
- 🎉 初版发行: PaddleOCRApi
