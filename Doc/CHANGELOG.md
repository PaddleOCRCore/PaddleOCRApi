# 📝 更新日志

## v4.0.0 `2026.1.17`
- ✅ 优化PaddleOCR.dll，集成UVDOC文本图像矫正功能，增加单字坐标输出，由return_word_box参数控制,Init去掉Keys参数
- ✅ 发布PaddleOCRRuntime_x64 v4.0.0，包含paddle3.2.2推理库、PaddleOCR.dll及依赖文件，表格识别模型升级为PP-SLANet_plus_infer
- ✅ PaddleocrSDK v4.0.0对齐PaddleOCR.dll，WinFormsApp增加是否生成单字坐标功能，集成文本图像矫正功能。更新CPP、Python、GO、Java示例代码。
- 
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
