# PaddleOCR C++ API 接口清单

本文件基于PaddleOCR.dll导出函数清单,按功能分组列出接口签名、参数说明与返回值说明。

> 注意:所有返回"指向字符串的指针(UTF-8)"均为库内部分配的缓冲区,调用者应使用 `FreeResultBuffer` 释放(除非接口文档另有说明)。

---

## 通用设置

- `void EnableLog(bool useLog)`
  - 描述:是否生成库内部日志,便于调试。
  - 参数:`useLog` — true 开启,false 关闭。

- `void EnableASCIIResult(bool useASCII)`
  - 描述:控制返回 JSON 中是否将非 ASCII 字符转为 ASCII 编码(如 `\uXXXX`)。
  - 参数:`useASCII` — true 返回 ASCII 编码,false 返回原始 UTF-8。

- `void EnableJsonResult(bool enable)`
  - 描述:设置库返回结果的格式是否为 JSON(默认 true)。
  - 参数:`enable` — true 返回 JSON,false 使用自定义文本格式。

- `char* GetError()`
  - 描述:获取上一次操作的错误信息。
  - 返回:指向错误字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

---

## 文字识别(OCR)API

### 初始化接口

- `bool Init(const char* det_infer, const char* cls_infer, const char* rec_infer, const OCRParameter parameter)`
  - 描述:初始化 OCR 引擎并加载模型(传入结构体参数)。
  - 参数:
    - `det_infer`:文本检测模型路径
    - `cls_infer`:文本行方向分类模型路径
    - `rec_infer`:文本识别模型路径
    - `parameter`:OCRParameter 结构体(参见 `AI_Parameter.h`)
  - 返回:初始化成功返回 `true`,失败返回 `false`。

- `bool Initjson(const char* det_infer, const char* cls_infer, const char* rec_infer, const char* parameterjson)`
  - 描述:使用 JSON 字符串参数初始化 OCR 引擎。
  - 参数:
    - `det_infer`:文本检测模型路径
    - `cls_infer`:文本行方向分类模型路径
    - `rec_infer`:文本识别模型路径
    - `parameterjson`:JSON 格式的参数字符串
  - 返回:初始化成功返回 `true`,失败返回 `false`。

- `bool DynamicInit(SyncParameter parameter)`
  - 描述:动态修改 OCR 识别参数(运行时调整配置)。
  - 参数:`SyncParameter` 结构体,包含需要更新的参数。
  - 返回:成功返回 `true`,失败返回 `false`。

### 识别接口

- `const char* Detect(const char* imageFile)`
  - 描述:对输入图片文件执行 OCR 检测并返回结果字符串(JSON 或自定义格式)。
  - 参数:`imageFile` — 图片文件路径。
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* DetectMat(const cv::Mat& cvmat)`
  - 描述:对输入 OpenCV `cv::Mat` 执行 OCR 检测并返回结果字符串。
  - 参数:`cvmat` — OpenCV `cv::Mat` 引用(通过指针传递)。
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* DetectByte(const unsigned char* imagebytedata, size_t size)`
  - 描述:对图片字节数组执行 OCR 检测。
  - 参数:
    - `imagebytedata`:图片字节数组指针
    - `size`:字节数组长度
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* DetectBase64(const char* imagebase64)`
  - 描述:对 Base64 编码的图片字符串执行 OCR 检测。
  - 参数:`imagebase64` — Base64 编码的图片字符串。
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* DetectScreenShot(const unsigned char* data, int size)`
  - 描述:对内存截图数据执行 OCR 检测(适用于屏幕截图等场景)。
  - 参数:
    - `data`:截图字节数据指针
    - `size`:截图字节长度
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

### 资源释放

- `int FreeEngine()`
  - 描述:释放并关闭 OCR 引擎,释放相关资源。
  - 返回:成功返回 `0`,失败返回非 `0`。

---

## 版面结构识别(PP-StructureV3)API

> 支持20类文档元素识别:版面检测、表格识别、公式识别、印章识别、图表转表等

### 初始化接口

- `bool InitStructure(const char* det_infer, const char* cls_infer, const char* rec_infer, const char* layout_model_dir, const char* table_model_dir, const char* formula_model_dir, const char* seal_model_dir, const char* doc_cls_infer, const char* doc_unwarp_model, const char* region_model_dir, const LayoutParameter tablepara)`
  - 描述:初始化结构化文档识别引擎(扩展版本),支持多种文档元素的综合识别。
  - 参数:
    - `det_infer`:文本检测模型路径
    - `cls_infer`:文本行方向分类模型路径(可选,NULL表示不使用)
    - `rec_infer`:文本识别模型路径
    - `layout_model_dir`:版面分析模型目录路径
    - `table_model_dir`:表格识别模型目录路径
    - `formula_model_dir`:公式识别模型路径(可选,NULL表示不使用)
    - `seal_model_dir`:印章识别模型路径(可选,NULL表示不使用)
    - `doc_cls_infer`:文档方向分类模型路径(可选,NULL表示不使用)
    - `doc_unwarp_model`:文档图像矫正模型路径(可选,NULL表示不使用)
    - `region_model_dir`: 区域检测模型目录路径，可选
    - `tablepara`:LayoutParameter 结构体参数
  - 返回:初始化成功返回 `true`,失败返回 `false`。

- `bool InitStructurejson(const char* det_infer, const char* cls_infer, const char* rec_infer, const char* layout_model_dir, const char* table_model_dir, const char* formula_model_dir, const char* seal_model_dir, const char* doc_cls_infer, const char* doc_unwarp_model, const char* region_model_dir, const char* parjson)`
  - 描述:使用 JSON 参数字符串初始化结构化文档识别引擎。
  - 参数:同上,最后一个参数为 JSON 格式的参数字符串。
  - 返回:初始化成功返回 `true`,失败返回 `false`。

### 识别接口

- `const char* DetectLayout(const char* imageFile)`
  - 描述:执行文档版面分析(扩展版本),包含:文档预处理→版面检测→全局OCR→条件识别(表格/公式/印章/图表)→结果融合→版面排序。
  - 参数:`imageFile` — 输入图片文件路径。
  - 返回:完整分析结果 JSON 字符串(需使用 `FreeResultBuffer` 释放);失败时可能返回 NULL。

- `const char* DetectLayoutMat(const cv::Mat& cvmat)`
  - 描述:执行文档版面分析 - OpenCV Mat 输入。
  - 参数:`cvmat` — OpenCV Mat 引用(通过指针传递)。
  - 返回:完整分析结果 JSON 字符串(需使用 `FreeResultBuffer` 释放);失败时可能返回 NULL。

- `const char* DetectLayoutByte(const unsigned char* imagebytedata, size_t size)`
  - 描述:执行文档版面分析 - 字节数组输入。
  - 参数:
    - `imagebytedata`:图片字节数组指针
    - `size`:字节数组长度(字节数)
  - 返回:完整分析结果 JSON 字符串(需使用 `FreeResultBuffer` 释放);失败时可能返回 NULL。

- `const char* DetectLayoutBase64(const char* imagebase64)`
  - 描述:执行文档版面分析 - Base64 编码输入。
  - 参数:`imagebase64` — Base64 编码的图片字符串。
  - 返回:完整分析结果 JSON 字符串(需使用 `FreeResultBuffer` 释放);失败时可能返回 NULL。

### 资源释放

- `int FreeStructureEngine()`
  - 描述:释放文档版面分析引擎及所有相关资源。
  - 返回:成功返回 `0`,失败返回非 `0`。

---

## 文档图像矫正(UVDoc)API

### 初始化接口

- `bool InitUVDoc(const char* uvdoc_infer, UVDocParameter uvdocpara)`
  - 描述:初始化文本图像矫正模块(传入参数结构体)。
  - 参数:
    - `uvdoc_infer`:UVDoc 模型路径
    - `uvdocpara`:UVDocParameter 参数结构体
  - 返回:成功返回 `true`,失败返回 `false`。

- `bool InitUVDocjson(const char* uvdoc_infer, const char* parjson)`
  - 描述:使用 JSON 参数初始化 UVDoc 模块。
  - 参数:
    - `uvdoc_infer`:UVDoc 模型路径
    - `parjson`:JSON 格式的参数字符串
  - 返回:成功返回 `true`,失败返回 `false`。

### 矫正接口

> 注意:UVDoc 所有矫正接口均不返回结果字符串,而是直接保存到指定的输出文件路径。

- `void UVDocImageFile(const char* filename, const char* outputfilepath)`
  - 描述:对输入文件执行图像矫正并保存到指定输出路径。
  - 参数:
    - `filename`:输入文件路径
    - `outputfilepath`:输出文件路径(必须指定)

- `void UVDocMat(void* cvmat, const char* outputfilepath)`
  - 描述:接收 OpenCV `Mat` 指针进行矫正并输出到指定路径。
  - 参数:
    - `cvmat`:OpenCV Mat 指针
    - `outputfilepath`:输出文件路径(必须指定)

- `void UVDocByte(const unsigned char* imagebyte, long long size, const char* outputfilepath)`
  - 描述:接收图片字节数组进行矫正并输出到指定路径。
  - 参数:
    - `imagebyte`:图片字节数组指针
    - `size`:字节数组大小
    - `outputfilepath`:输出文件路径(必须指定)

- `void UVDocBase64(const char* base64, const char* outputfilepath)`
  - 描述:接收 Base64 字符串进行矫正并输出到指定路径。
  - 参数:
    - `base64`:Base64 编码的图片字符串
    - `outputfilepath`:输出文件路径(必须指定)

### 资源释放

- `int FreeUVDocEngine()`
  - 描述:释放 UVDoc 实例并回收资源。
  - 返回:成功返回 `0`,失败返回 `-1`。

---

## 以图找图 API

- `const char* FindImage(const char* bigImagePath, const char* smallImagePath, double threshold = 0.8, bool toGray = true, bool useSlideMatch = false)`
  - 描述:在大图中查找小图并返回 JSON 结果。
  - 参数:
    - `bigImagePath`:大图路径
    - `smallImagePath`:小图路径
    - `threshold`:匹配阈值 [0,1],默认 0.8(滑块匹配场景可适当降低到 ~0.2)
    - `toGray`:是否转换为灰度图进行匹配(默认 true)
    - `useSlideMatch`:是否使用滑块/边缘验证(默认 false,滑块找图时请将 threshold 改为 0.2 左右)
  - 返回:指向结果 JSON 字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

---

## 视觉语言模型(VL)API

> 基于 llamaocr-vl.dll 动态链接库,提供视觉语言模型的 OCR 和文档分析能力。
> 需要在 `appsettings.json` 中将 `OCRVLConfig.enabled` 设为 `true` 后方可使用。

### 通用 OCR 识别(Chat 模式)

#### 初始化接口

- `int Init(const char* configPath)`
  - 描述:初始化 VL 模型引擎。
  - 参数:`configPath` — YAML 配置文件路径(如 `PaddleOCR-VL-1.5.yaml` 或 `Qwen3VL-4B.yaml`)。
  - 返回:成功返回 `0`,失败返回非 `0`。

#### Chat 识别接口

- `const char* Chat(const char* prompt, const char* imagePath)`
  - 描述:使用提示词和图片文件路径进行通用 OCR 识别。
  - 参数:
    - `prompt`:提示词字符串(如 "请识别图片中的文字")
    - `imagePath`:图片文件路径
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* ChatData(const char* prompt, const unsigned char* imageData, size_t imageSize)`
  - 描述:使用提示词和图片字节数据进行通用 OCR 识别。
  - 参数:
    - `prompt`:提示词字符串
    - `imageData`:图片字节数据指针
    - `imageSize`:图片字节数据大小
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* ChatBase64(const char* prompt, const char* base64Image)`
  - 描述:使用提示词和 Base64 编码的图片进行通用 OCR 识别。
  - 参数:
    - `prompt`:提示词字符串
    - `base64Image`:Base64 编码的图片字符串
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* ChatMat(const char* prompt, void* cvMat)`
  - 描述:使用提示词和 OpenCV Mat 进行通用 OCR 识别。
  - 参数:
    - `prompt`:提示词字符串
    - `cvMat`:OpenCV Mat 指针
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

### 文档分析(Doc 模式)

#### 初始化接口

- `int InitDoc(const char* configPath)`
  - 描述:初始化文档分析引擎(启用版面分析模式)。
  - 参数:`configPath` — YAML 配置文件路径。
  - 返回:成功返回 `0`,失败返回非 `0`。

#### Doc 分析接口

- `const char* DocChat(const char* imagePath)`
  - 描述:对图片文件执行版面分析识别(返回 Markdown/JSON 格式结果)。
  - 参数:`imagePath` — 图片文件路径。
  - 返回:指向结果字符串的指针(UTF-8),包含 content、markdown、jsonText 等字段,调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* DocChatData(const unsigned char* imageData, size_t imageSize)`
  - 描述:对图片字节数据执行版面分析识别。
  - 参数:
    - `imageData`:图片字节数据指针
    - `imageSize`:图片字节数据大小
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* DocChatBase64(const char* base64Image)`
  - 描述:对 Base64 编码的图片执行版面分析识别。
  - 参数:`base64Image` — Base64 编码的图片字符串。
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

- `const char* DocChatMat(void* cvMat)`
  - 描述:对 OpenCV Mat 执行版面分析识别。
  - 参数:`cvMat` — OpenCV Mat 指针。
  - 返回:指向结果字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

### 资源释放

- `void FreeEngine()`
  - 描述:释放 VL 模型引擎及资源(Chat 模式)。

- `void FreeDocAnalyser()`
  - 描述:释放文档分析引擎及资源(Doc 模式)。

- `void FreeResultBuffer(void* ptr)`
  - 描述:释放由 VL 接口返回的结果缓冲区。
  - 参数:`ptr` — 由库返回的缓冲区指针。

- `char* GetError()`
  - 描述:获取 VL 操作的错误信息。
  - 返回:指向错误字符串的指针(UTF-8),调用者需使用 `FreeResultBuffer` 释放;失败时可能返回 NULL。

---

## 释放缓冲区

- `void FreeResultBuffer(void* buffer)`
  - 描述:释放由 `Detect*` / `GetError` / `DetectLayout*` / VL 接口等函数返回的缓冲区。
  - 参数:`buffer` — 由库返回的缓冲区指针。
  - 注意:**必须调用此函数释放所有返回字符串指针,否则会造成内存泄漏**。

---

## WebAPI 接口参考

> 以下接口通过 ASP.NET Core WebAPI 暴露,路由前缀根据控制器而定。

### OCRServiceController (标准 OCR)

路由前缀:`/OCRService`

- `GET /OCRService/Get` - 检查服务状态
- `POST /OCRService/Detect` - 标准 OCR 识别(Base64)
- `POST /OCRService/DetectFile` - 标准 OCR 识别(文件上传)

### UVDocServiceController (图像矫正)

路由前缀:`/UVDocService`

- `GET /UVDocService/Get` - 检查服务状态
- `POST /UVDocService/Correct` - 图像矫正(Base64)
- `POST /UVDocService/CorrectFile` - 图像矫正(文件上传)

### OCRVLServiceController (视觉语言模型)

路由前缀:`/OCRVLService`

- `GET /OCRVLService/Get` - 检查服务状态

#### Chat 模式(通用 OCR)

- `POST /OCRVLService/GetOCRVL`
  - 描述:通用 OCR 识别,传入提示词和图片 Base64(**未启用版面分析时使用**)
  - Content-Type:`application/json`
  - 请求体:
    ```json
    {
      "Prompt": "请识别图片中的文字",
      "Base64String": "<图片Base64字符串>"
    }
    ```
  - 返回:
    ```json
    {
      "code": 200,
      "data": { "content": "识别结果文本" }
    }
    ```

- `POST /OCRVLService/GetOCRVLFile`
  - 描述:通用 OCR 识别,上传图片文件和提示词(**未启用版面分析时使用**)
  - Content-Type:`multipart/form-data`
  - 请求参数:
    - `file`:图片文件(必填)
    - `prompt`:提示词字符串(必填)
  - 返回:同上

#### Doc 模式(版面分析)

- `POST /OCRVLService/GetDOCVL`
  - 描述:版面分析识别,传入图片 Base64(**启用版面分析时使用**)
  - Content-Type:`application/json`
  - 请求体:
    ```json
    {
      "Base64String": "<图片Base64字符串>"
    }
    ```
  - 返回:
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

- `POST /OCRVLService/GetDOCVLFile`
  - 描述:版面分析识别,上传图片文件(**启用版面分析时使用**)
  - Content-Type:`multipart/form-data`
  - 请求参数:
    - `file`:图片文件(必填)
  - 返回:同上

---

## 注意事项

1. **线程安全**:所有接口线程安全,多线程环境下C++内部队列执行或使用多个引擎实例。
2. **内存管理**:所有返回 `const char*` 或 `char*` 的接口,调用者**必须**使用 `FreeResultBuffer` 释放返回的指针。
3. **模型路径**:所有模型路径必须为绝对路径或相对于工作目录的有效路径。
4. **错误处理**:调用任何接口后,建议通过 `GetError()` 检查是否有错误发生。
5. **资源释放**:程序退出前务必调用对应的 `Free*` 函数释放引擎资源。
6. **UVDoc 特殊性**:UVDoc 的所有矫正接口均不返回结果字符串,而是直接将矫正后的图片保存到指定的输出文件路径。
7. **VL 模型依赖**:VL 模型依赖于 `llamaocr-vl.dll`,与标准 OCR 的 `PaddleOCR.dll` 不同,需注意区分。

---

## 版本信息

- **SDK 版本**:4.4.0 (paddle_inference 3.3+)
- **支持平台**:Windows x64
- **更新日期**:2026-05-12
