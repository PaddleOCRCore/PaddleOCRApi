# UVDoc文本图像矫正服务集成说明

## 概述

UVDoc文本图像矫正服务已成功集成到PaddleOCR WebApi项目中，提供文本图像矫正功能。

## 配置说明

### appsettings.json 配置

在 `appsettings.json` 中添加了 `UVDocConfig` 配置节：

```json
"UVDocConfig": {
    "enabled": false,           // 是否启用UVDoc服务，默认false
    "uvdoc_infer": "UVDoc_infer", // UVDoc模型路径（相对于models目录）
    "enable_mkldnn": true,      // CPU模式下是否使用MKLDNN加速
    "cpu_threads": 10,          // CPU线程数
    "use_gpu": false,           // 是否使用GPU
    "gpu_id": 0,                // GPU设备ID
    "gpu_mem": 2000,            // GPU内存（MB）
    "use_tensorrt": false       // 是否使用TensorRT加速
}
```

### 启用服务

1. 将 `enabled` 设置为 `true`
2. 确保模型文件存在于 `models/UVDoc_infer/` 目录
3. 重启应用程序

## API 接口说明

### 1. 服务状态检查

**接口:** `GET /UVDocService/Get`

**响应:**
```json
{
    "status": "OK",
    "data": {
        "message": "UVDoc文本图像矫正服务已启动",
        "initialized": true,
        "timestamp": "2026-01-05T10:00:00"
    }
}
```

### 2. Base64图像矫正

**接口:** `POST /UVDocService/UVDocBase64`

**请求参数:**
```json
{
    "base64String": "iVBORw0KGgo..."
}
```

**响应:**
```json
{
    "status": "OK",
    "data": {
        "base64Image": "iVBORw0KGgo...",
        "timestamp": "2026-01-05T10:00:00"
    }
}
```

**C# 调用示例:**
```csharp
using var client = new HttpClient();
var requestData = new { base64String = Convert.ToBase64String(imageBytes) };
var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
var response = await client.PostAsync("http://localhost:5000/UVDocService/UVDocBase64", content);
var result = await response.Content.ReadAsStringAsync();
```

### 3. 文件上传矫正（返回图片文件）

**接口:** `POST /UVDocService/UVDocFile`

**请求:** multipart/form-data
- `file`: 图片文件

**响应:** 图片文件流（image/jpeg）

**C# 调用示例:**
```csharp
using var client = new HttpClient();
using var form = new MultipartFormDataContent();
using var fileStream = File.OpenRead("document.jpg");
using var streamContent = new StreamContent(fileStream);
streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
form.Add(streamContent, "file", "document.jpg");

var response = await client.PostAsync("http://localhost:5000/UVDocService/UVDocFile", form);
var imageBytes = await response.Content.ReadAsByteArrayAsync();
await File.WriteAllBytesAsync("corrected.jpg", imageBytes);
```

### 4. 文件上传矫正（返回Base64）

**接口:** `POST /UVDocService/UVDocFileToBase64`

**请求:** multipart/form-data
- `file`: 图片文件

**响应:**
```json
{
    "status": "OK",
    "data": {
        "base64Image": "iVBORw0KGgo...",
        "originalFileName": "document.jpg",
        "timestamp": "2026-01-05T10:00:00"
    }
}
```

### 5. 字节数组矫正

**接口:** `POST /UVDocService/UVDocBytes`

**请求:** multipart/form-data
- `file`: 图片文件

**响应:** 图片文件流（image/jpeg）

## 部署说明

### 1. 模型文件部署

确保以下模型文件存在：
```
OCRCoreService/
├── models/
│   └── UVDoc_infer/
│       ├── inference.pdiparams
│       ├── inference.pdmodel
│       └── inference.json
```

### 2. DLL文件部署

确保以下DLL文件在应用程序目录：
- `PaddleDocVision.dll` (或 Linux 下的 `libPaddleDocVision.so`)
- `paddle_inference.dll`
- 相关依赖 DLL（MKL库、vcomp140.dll等）

### 3. 配置文件

修改 `appsettings.json`：
```json
"UVDocConfig": {
    "enabled": true,  // 启用服务
    "uvdoc_infer": "UVDoc_infer",
    "use_gpu": false,  // 根据硬件配置
    "cpu_threads": 10  // 根据CPU核心数调整
}
```

## 错误处理

### 常见错误

1. **服务未启用**
   - 错误: Controller 无法解析 IUVDocService
   - 解决: 检查 `appsettings.json` 中 `enabled` 是否为 `true`

2. **模型初始化失败**
   - 错误: 引擎未初始化
   - 解决: 
     - 检查模型文件路径是否正确
     - 检查日志中的详细错误信息
     - 确认 DLL 文件已正确部署

3. **DLL找不到**
   - 错误: DllNotFoundException
   - 解决: 
     - 确认 `PaddleDocVision.dll` 在应用程序目录
     - 确认所有依赖 DLL 都已部署
     - Windows: 检查 Visual C++ 运行库是否已安装

## 性能优化建议

1. **CPU模式**
   - 设置 `enable_mkldnn: true` 启用Intel MKL加速
   - 根据CPU核心数调整 `cpu_threads`
   - 建议值: 逻辑核心数的 50%-75%

2. **GPU模式**
   - 设置 `use_gpu: true`
   - 调整 `gpu_mem` 根据显存大小
   - 启用 `use_tensorrt` 可进一步提升性能（需GPU支持）

3. **并发处理**
   - 服务使用单例模式，内部线程安全
   - 可同时处理多个请求
   - 根据硬件配置调整并发数

## 日志查看

查看 `Logs/` 目录下的日志文件：
- 服务启动日志: "UVDoc文本图像矫正服务已启用"
- 引擎初始化日志: "UVDoc引擎初始化成功 [GPU: false, CPU线程: 10]"
- 请求处理日志: "开始处理Base64文本图像矫正请求"

## 测试建议

1. 使用 Swagger UI 进行接口测试: `http://localhost:5000/swagger`
2. 先调用 `/UVDocService/Get` 确认服务状态
3. 使用小图片测试各个接口
4. 监控性能指标（响应时间、内存使用）

## 技术支持

- 项目地址: https://github.com/PaddleOCRCore/PaddleOCRApi
- 问题反馈: 通过 GitHub Issues
