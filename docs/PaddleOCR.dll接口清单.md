# PaddleOCR C++ API 接口清单

本文件基于PaddleOCR.dll导出函数清单，按功能分组列出接口签名、参数说明与返回值说明。

> 注意：所有返回“指向字符串的指针（UTF-8）”均为库内部分配的缓冲区，调用者应使用 `FreeResultBuffer` 释放（除非接口文档另有说明）。

---

## 通用设置

- `void EnableLog(bool useLog)`
  - 描述：是否生成库内部日志，便于调试。
  - 参数：`useLog` — true 开启，false 关闭。

- `void EnableASCIIResult(bool useASCII)`
  - 描述：控制返回 JSON 中是否将非 ASCII 字符转为 ASCII 编码（如 `\uXXXX`）。
  - 参数：`useASCII` — true 返回 ASCII 编码，false 返回原始 UTF-8。

- `void EnableJsonResult(bool enable)`
  - 描述：设置库返回结果的格式是否为 JSON（默认 true）。
  - 参数：`enable` — true 返回 JSON，false 使用自定义文本格式。

- `char* GetError()`
  - 描述：获取上一次操作的错误信息。
  - 返回：指向错误字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

---

## 文字识别（OCR）API

- `bool Init(const char* det_infer, const char* cls_infer, const char* rec_infer, const OCRParameter parameter)`
  - 描述：初始化 OCR 引擎并加载模型（传入结构体参数）。
  - 参数：模型路径与参数结构体（参见 `AI_Parameter.h`）。
  - 返回：初始化成功返回 `true`，失败返回 `false`。

- `bool Initjson(const char* det_infer, const char* cls_infer, const char* rec_infer, const char* parameterjson)`
  - 描述：使用 JSON 字符串参数初始化 OCR 引擎。
  - 参数：模型路径与 JSON 参数字符串。
  - 返回：初始化成功返回 `true`，失败返回 `false`。

- `bool DynamicInit(SyncParameter parameter)`
  - 描述：使用同步参数动态初始化（适用于运行时根据配置调整参数并重新初始化）。
  - 参数：`SyncParameter` 结构体。
  - 返回：成功返回 `true`，失败返回 `false`。

- `const char* Detect(const char* imageFile)`
  - 描述：对输入图片文件执行 OCR 检测并返回结果字符串（JSON 或自定义格式）。
  - 参数：图片文件路径。
  - 返回：指向结果字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

- `const char* DetectMat(const cv::Mat& cvmat)`
  - 描述：对输入 OpenCV `cv::Mat` 执行 OCR 检测并返回结果字符串。
  - 参数：OpenCV `cv::Mat` 引用。
  - 返回：指向结果字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

- `const char* DetectByte(const unsigned char* imagebytedata, size_t size)`
  - 描述：对图片字节数组执行 OCR 检测。
  - 参数：图片字节数组指针与长度（字节）。
  - 返回：指向结果字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

- `const char* DetectBase64(const char* imagebase64)`
  - 描述：对 Base64 编码的图片字符串执行 OCR 检测。
  - 参数：Base64 字符串。
  - 返回：指向结果字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

- `int FreeEngine()`
  - 描述：释放并关闭 OCR 引擎，释放相关资源。
  - 返回：成功返回 `0`，失败返回非 `0`。

---

## 表格识别（Table）API

- `bool InitTable(const char* det_infer, const char* cls_infer, const char* rec_infer, const char* table_model_dir, TableParameter parameter)`
  - 描述：初始化表格识别引擎并加载模型（结构体参数）。
  - 参数：模型路径、表格模型目录与参数结构体。
  - 返回：初始化成功返回 `true`，失败返回 `false`。

- `bool InitTablejson(const char* det_infer, const char* cls_infer, const char* rec_infer, const char* table_model_dir, const char* parameterjson)`
  - 描述：使用 JSON 参数字符串初始化表格识别引擎。
  - 返回：初始化成功返回 `true`，失败返回 `false`。

- `const char* DetectTable(const char* imageFile)`
  - 描述：对图片文件执行表格识别并返回结果字符串（JSON 或自定义格式）。
  - 参数：图片文件路径。
  - 返回：指向结果字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

- `const char* DetectTableByte(const unsigned char* imagebytedata, size_t size)`
  - 描述：对图片字节数组执行表格识别。
  - 返回：指向结果字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

- `const char* DetectTableBase64(const char* imagebase64)`
  - 描述：对 Base64 编码的图片字符串执行表格识别。
  - 返回：指向结果字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

- `int FreeTableEngine()`
  - 描述：释放并关闭表格识别引擎。
  - 返回：成功返回 `0`，失败返回非 `0`。

---

## 文档图像矫正（UVDoc）API

- `bool InitUVDoc(const char* uvdoc_infer, UVDocParameter uvdocpara)`
  - 描述：初始化文本图像矫正模块（结构体参数）。
  - 返回：成功返回 `true`，失败返回 `false`。

- `bool InitUVDocjson(const char* uvdoc_infer, const char* parjson)`
  - 描述：使用 JSON 参数初始化 UVDoc 模块。

- `void UVDocImageFile(const char* filename, const char* outputfilepath)`
  - 描述：对输入文件执行图像矫正并保存到指定输出路径。

- `void UVDocMat(void* cvmat, const char* outputfilepath)`
  - 描述：接收 OpenCV `Mat` 指针进行矫正并输出（输出路径必须指定）。

- `void UVDocByte(const unsigned char* imagebyte, long long size, const char* outputfilepath)`
  - 描述：接收图片字节数组进行矫正并输出。

- `void UVDocBase64(const char* base64, const char* outputfilepath)`
  - 描述：接收 Base64 字符串进行矫正并输出。

- `int FreeUVDocEngine()`
  - 描述：释放 UVDoc 实例并回收资源。成功返回 `0`，失败返回 `-1`。

---

## 以图找图 API

- `const char* FindImage(const char* bigImagePath, const char* smallImagePath, double threshold = 0.8, bool toGray = true, bool useSlideMatch = false)`
  - 描述：在大图中查找小图并返回 JSON 结果。
  - 参数：
    - `bigImagePath`：大图路径。
    - `smallImagePath`：小图路径。
    - `threshold`：匹配阈值 [0,1]，默认 0.8（滑块匹配场景可适当降低到 ~0.2）。
    - `toGray`：是否转换为灰度图进行匹配（默认 true）。
    - `useSlideMatch`：是否使用滑块/边缘验证（默认 false）。
  - 返回：指向结果 JSON 字符串的指针（UTF-8），调用者需使用 `FreeResultBuffer` 释放；失败时可能返回 NULL。

---

## 释放缓冲区

- `void FreeResultBuffer(void* buffer)`
  - 描述：释放由 `Detect*` / `GetError` / `GetPerformanceReport` 等函数返回的缓冲区。
  - 参数：由库返回的缓冲区指针。

---

## OCRVLServiceController WebAPI 接口

> 基于视觉语言模型（VLM）的 OCR 扩展服务，路由前缀：`/OCRVLService`。
> 需要在 `appsettings.json` 中将 `OCRVLConfig.enabled` 设为 `true` 后方可使用。

### GET `/OCRVLService/Get`

- 描述：检查 OCR-VL 服务状态。
- 返回：服务状态信息（message、timestamp）。

---

### POST `/OCRVLService/GetOCRVL`

- 描述：通用 OCR 识别，传入提示词和图片 Base64 编码（**未启用版面分析时使用**）。
- Content-Type：`application/json`
- 请求体：

```json
{
  "Prompt": "请识别图片中的文字",
  "Base64String": "<图片Base64字符串>"
}
```

- 返回：

```json
{
  "code": 200,
  "data": { "content": "识别结果文本" }
}
```

---

### POST `/OCRVLService/GetOCRVLFile`

- 描述：通用 OCR 识别，上传图片文件和提示词（**未启用版面分析时使用**）。
- Content-Type：`multipart/form-data`
- 请求参数：
  - `file`：图片文件（必填）
  - `prompt`：提示词字符串（必填）
- 返回：

```json
{
  "code": 200,
  "data": { "content": "识别结果文本" }
}
```

---

### POST `/OCRVLService/GetDOCVL`

- 描述：版面分析识别，传入图片 Base64 编码（**启用版面分析时使用**）。
- Content-Type：`application/json`
- 请求体：

```json
{
  "Base64String": "<图片Base64字符串>"
}
```

- 返回：

```json
{
  "code": 200,
  "data": {
    "content": "原始返回内容",
    "markdown": "Markdown格式的版面分析结果",
    "jsonText": "JSON格式的版面分析结果"
  }
}
```

---

### POST `/OCRVLService/GetDOCVLFile`

- 描述：版面分析识别，上传图片文件（**启用版面分析时使用**）。
- Content-Type：`multipart/form-data`
- 请求参数：
  - `file`：图片文件（必填）
- 返回：

```json
{
  "code": 200,
  "data": {
    "content": "原始返回内容",
    "markdown": "Markdown格式的版面分析结果",
    "jsonText": "JSON格式的版面分析结果"
  }
}
```

---