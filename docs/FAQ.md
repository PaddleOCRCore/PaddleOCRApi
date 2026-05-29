# 🔍 常见问题 (FAQ)

> 💡 **提示**：本文档整理了 PaddleOCRWebApi 使用过程中最常见的问题和解决方案。如未找到答案，请加入 QQ 群 **475159576** 交流讨论。

---

## 📋 目录

- [版本选择](#版本选择)
- [环境配置](#环境配置)
- [模型相关](#模型相关)
- [性能优化](#性能优化)
- [功能使用](#功能使用)
- [开发集成](#开发集成)
- [GPU授权](#gpu授权)
- [问题排查](#问题排查)

---

## 版本选择

<details>
<summary><b>Q: 如何选择CPU版本还是GPU版本？</b></summary>

**A:** 

| 版本 | 适用场景 | 优势 | 要求 |
|------|---------|------|------|
| **CPU版本** | 小批量识别、低并发场景 | 部署简单，无需额外配置 | 无特殊要求 |
| **GPU版本** | 大批量识别、高并发场景 | 速度快3-10倍 | CUDA 12.9+ (v3.x) 或 CUDA 12.9(v2.6.2) |

**建议：**
- 日均处理图片 < 1000张 → CPU版本
- 日均处理图片 > 1000张 → GPU版本
- 实时性要求高 → GPU版本 + TensorRT加速
</details>

---

## 环境配置

<details>
<summary><b>Q: Windows环境下如何配置GPU运行环境？</b></summary>

**A:** 

### GPU配置步骤：

1. **安装CUDA 12.9+**
   - 下载地址：[CUDA Toolkit Archive](https://developer.nvidia.com/cuda-toolkit-archive)
   - 安装后验证：`nvcc --version`

2. **安装cuDNN 9.x**
   - 下载地址：[cuDNN Archive](https://developer.nvidia.com/cudnn-archive)
   - 将 `bin` 目录下的DLL复制到程序运行目录

3. **复制Paddle Inference依赖库**

4. **配置appsettings.json**
   ```json
   {
     "use_gpu": true,
     "gpu_id": 0,
     "gpu_mem": 4000,
     "use_tensorrt": false
   }
   ```

### 常见问题：
- ❌ **找不到cudnn64_9.dll** → 确认cuDNN版本与CUDA版本匹配
- ❌ **GPU初始化失败** → 检查显卡驱动是否支持CUDA 12.9+
- ❌ **显存不足** → 降低 `gpu_mem` 参数值
</details>

<details>
<summary><b>Q: 如何在Linux上使用？</b></summary>

**A:** 

当前开源版本仅支持 **Windows x64** 平台。如需Linux支持：

1. **方案一：部署WebAPI服务（推荐）**   

2. **方案二：定制Linux版本**
   - 需要针对Linux平台编译PaddleOCR.so动态库
   - 联系开发者QQ：**2380243976** 咨询定制服务

</details>

<details>
<summary><b>Q: .NET项目如何引用SDK？</b></summary>

**A:** 

### 方式一：NuGet包安装（推荐）

```xml
<!-- paddle_inference 3.3.0 版本 -->
<PackageReference Include="PaddleOCRSDK" Version="4.3.0" />
<PackageReference Include="PaddleOCRRuntime_x64" Version="4.3.1" />

<!-- 或 paddle_inference 2.6.2 版本 -->
<PackageReference Include="PaddleOCRSDK" Version="1.0.5" />
<PackageReference Include="PaddleOCRRuntime_x64" Version="1.0.0" />
```

### 方式二：手动引用

1. 从GitHub Release下载对应版本
2. 将 `PaddleOCRSDK.dll` 添加到项目引用
3. 确保运行时目录下包含所有依赖文件

### 支持的.NET版本：
✅ netstandard2.0, net45-net48, net6.0-net10.0
</details>

---

## 模型相关

<details>
<summary><b>Q: 支持哪些OCR模型？</b></summary>

**A:** 

### 标准OCR模型：
- ✅ **PP-OCRv5_server**（推荐，精度最高）
- ✅ **PP-OCRv5_mobile**（推荐，速度最快）
- ✅ **PP-OCRv4_server/mobile**（向下兼容）
- ✅ **自定义训练模型**

### PP-Structure版面分析模型（v4.3.0+）：
- 📊 **PP-DocLayoutV2/PP-DocLayoutV3**：20类文档元素检测
- 📋 **SLANet_plus**：表格结构识别
- 🔢 **UniMERNet**：公式识别
- 🔴 **印章识别模型**
- 📈 **图表转表模型**

### OCR-VL视觉语言模型（v4.3.0+）：
- 🤖 **PaddleOCR-VL-1.5-GGUF**
- 🤖 **DeepSeek-OCR-GGUF**
- 🤖 **Qwen2-VL-OCR-GGUF**
- 🤖 **FireRed-OCR-GGUF**

**模型下载：**
- 官方模型：[PaddleOCR官网](https://www.paddleocr.ai/latest/version3.x/pipeline_usage/OCR.html)
- GGUF模型：[ModelScope](https://www.modelscope.cn/models?name=OCR%20GGUF)
</details>

<details>
<summary><b>Q: 如何选择合适的模型？</b></summary>

**A:** 

| 场景 | 推荐模型 | 理由 |
|------|---------|------|
| 通用文档识别 | PP-OCRv5_server | 精度高，适用大多数场景 |
| 移动端/嵌入式 | PP-OCRv5_mobile | 体积小，速度快 |
| 表格文档 | PP-Structure + SLANet_plus | 专门优化表格识别 |
| 含公式文档 | PP-Structure + UniMERNet | 支持数学公式识别 |
| 复杂版面 | OCR-VL + PaddleOCR-VL-1.5 | 理解语义，智能识别 |
| 低配置设备 | PP-OCRv4_mobile | 资源占用少 |

**切换模型方法：**
修改 `appsettings.json` 中的模型路径：
```json
{
  "det_model_dir": "models/PP-OCRv5_server_det",
  "rec_infer": "models/PP-OCRv5_server_rec",
  "cls_model_dir": "models/PP-LCNet_x1_0_textline_ori"
}
```
</details>

<details>
<summary><b>Q: 如何使用自定义训练的模型？</b></summary>

**A:** 

1. **准备模型文件**
   - 确保导出为 inference 格式（包含 `.json` 和 `.pdiparams`）
   - 或使用 PaddleOCR 提供的导出工具

2. **放置模型文件**
   ```
   your_project/
   └── models/
       ├── custom_det/      # 检测模型
       ├── custom_rec/      # 识别模型
       └── custom_cls/      # 分类模型（可选）
   ```

3. **配置模型路径**
   ```json
   {
     "det_model_dir": "models/custom_det",
     "rec_infer": "models/custom_rec",
     "cls_model_dir": "models/custom_cls"
   }
   ```

4. **重新初始化OCR引擎**
   ```csharp
   var ocr = new OCRService();
   ocr.Init(det_model_dir, rec_infer, cls_model_dir);
   ```
</details>

---

## 性能优化

<details>
<summary><b>Q: 如何提高识别准确率？</b></summary>

**A:** 

### 1. 模型层面优化
```json
{
  // 使用server版模型（精度更高）
  "det_model_dir": "models/PP-OCRv5_server_det",
  "rec_infer": "models/PP-OCRv5_server_rec",
  
  // 启用方向分类器
  "use_angle_cls": true,
  "cls_thresh": 0.9
}
```

### 2. 检测参数调优
```json
{
  // 降低阈值可检测到更多文字（但可能增加误检）
  "det_db_thresh": 0.2,        // 默认0.3
  "det_db_box_thresh": 0.4,    // 默认0.5
  
  // 调整文本框紧致程度
  "det_db_unclip_ratio": 1.8   // 默认1.6，越大框越松
}
```

### 3. 图像预处理
- ✅ **分辨率优化**：确保文字高度 ≥ 30px
- ✅ **去噪处理**：使用 `ImageBeauty` 工具类
- ✅ **对比度增强**：提高文字与背景对比度
- ✅ **二值化**：对于扫描件可尝试二值化处理

### 4. 识别参数优化
```json
{
  // 提高识别置信度阈值
  "rec_img_h": 48,             // 保持默认
  "rec_img_w": 320,            // 长文本可适当增大
  
  // 启用单字坐标输出（便于后处理）
  "return_word_box": true
}
```

### 5. 特殊场景处理
- **倾斜文档** → 启用 `UVDoc` 图像矫正
- **复杂版面** → 使用 `PP-Structure` 版面分析
- **手写体** → 使用专门训练的手写体模型
- **低质量图片** → 先进行超分辨率处理
</details>

<details>
<summary><b>Q: 如何提高识别速度？</b></summary>

**A:** 

### CPU模式优化：

1. **启用MKLDNN加速**
   ```json
   {
     "enable_mkldnn": true,    // 关键！提速30-50%
     "cpu_threads": 30         // 根据CPU核心数调整
   }
   ```

2. **调整批处理大小**
   ```json
   {
     "rec_batch_num": 6,       // 适当增大批次
     "cls_batch_num": 1
   }
   ```

3. **限制输入图像尺寸**
   ```json
   {
     "max_side_len": 960       // 过大的图片会显著降低速度
   }
   ```

### GPU模式优化：

1. **启用TensorRT**
   ```json
   {
     "use_gpu": true,
     "use_tensorrt": true,     // 提速2-5倍
     "gpu_mem": 4000
   }
   ```

2. **使用mobile版模型**
   - PP-OCRv5_mobile 比 server 版快约40%

### 高并发优化：

1. **多实例配置**
   ```json
   {
     "ocr_instance_count": 5   // 最大10，根据内存调整
   }
   ```

2. **异步调用**
   ```csharp
   // WebAPI已支持异步处理
   var result = await ocr.DetectAsync(imageBytes);
   ```
</details>

<details>
<summary><b>Q: 内存占用过高怎么办？</b></summary>

**A:** 

### 原因分析：
1. 创建了过多OCR实例
2. 图像处理未及时释放
3. MKLDNN缓存累积

### 解决方案：

1. **限制实例数量**
   ```json
   {
     "ocr_instance_count": 3   // 根据实际并发调整
   }
   ```

2. **限制CPU内存**
   ```json
   {
     "cpu_mem": 2000           // 单位MB，-1表示不限制
   }
   ```

3. **关闭MKLDNN（牺牲速度换内存）**
   ```json
   {
     "enable_mkldnn": false    // 可减少30-50%内存占用
   }
   ```
</details>

---

## 功能使用

<details>
<summary><b>Q: 支持哪些图片格式？</b></summary>

**A:** 

### 支持的格式：
✅ JPG/JPEG  
✅ PNG  
✅ BMP  
✅ PDF 

### 最佳实践：
- 📸 **推荐格式**：JPG（压缩比好）或 PNG（无损）
- 📏 **推荐分辨率**：宽度 1000-3000px
- 📐 **推荐DPI**：300dpi以上（扫描件）
- 💾 **文件大小**：单张 < 10MB

### 不支持的格式：
❌ GIF（动图）→ 提取首帧转换  
❌ SVG（矢量图）→ 转换为位图  
❌ PSD → 导出为PNG/JPG  
</details>

<details>
<summary><b>Q: 如何进行文本图像矫正（UVDoc）？</b></summary>

**A:** 

**UVDoc** 用于纠正拍摄文档时的扭曲、倾斜、透视变形等问题。

### 使用方法：

#### 方式一：WebAPI调用

#### 方式二：C# SDK调用

#### 方式三：Python调用

### 应用场景：
- 📱 手机拍摄的文档照片
- 📄 倾斜扫描的纸质文档
- 📐 透视变形的书页照片
- 🔄 弯曲的票据/收据

### 效果对比：
矫正前 → 文字倾斜、透视变形  
矫正后 → 文字水平、版面规整  
**识别准确率提升：20-40%**
</details>

<details>
<summary><b>Q: 如何使用PP-Structure版面分析？</b></summary>

**A:** 

**PP-Structure** 是v4.3.0新增的高级功能，支持20类文档元素识别。

### 功能特性：
- 📊 版面检测（标题、段落、图片、表格等）
- 📋 表格结构识别（输出HTML/Markdown）
- 🔢 公式识别（LaTeX格式）
- 🔴 印章识别
- 📈 图表转表格

### 使用方法：

#### C# SDK调用

#### WebAPI调用

### 注意事项：
⚠️ 需要自行下载相关模型（较大） 
⚠️ 建议仅在需要结构化输出时使用  
</details>

<details>
<summary><b>Q: 如何使用OCR-VL视觉语言模型？</b></summary>

**A:** 

**OCR-VL** 是基于视觉语言大模型的高级识别模块（v4.3.0+）。

### 功能特性：
- 🎯 **提示词引导识别**：指定要提取的内容
- 📄 **智能版面理解**：理解文档结构和语义
- 🔍 **定向信息抽取**：只提取感兴趣的信息
- 📊 **结构化输出**：直接输出JSON/Markdown

### 支持的模型：
- PaddleOCR-VL-1.5-GGUF
- DeepSeek-OCR-GGUF
- Qwen2-VL-OCR-GGUF
- FireRed-OCR-GGUF

### 配置方法：

1. **下载GGUF模型**
   ```bash
   # 从 ModelScope 下载
   # 例如：PaddleOCR-VL-1.5-Q4_K_M.gguf
   ```

2. **配置appsettings.json**
   ```json
   {
     "OCRVLConfig": {
       "enabled": true,
       "yaml_path": "configs/PaddleOCR-VL-1.5.yaml"
     }
   }
   ```

3. **编辑YAML配置文件**
   ```yaml
   # configs/PaddleOCR-VL-1.5.yaml
   model_path: "models/PaddleOCR-VL-1.5-Q4_K_M.gguf"
   chat_template: "paddleocr-vl"
   ```

### 典型应用场景：

1. **发票信息提取**
   ```
   Prompt: "请提取发票代码、发票号码、开票日期、价税合计"
   ```

2. **身份证识别**
   ```
   Prompt: "请提取姓名、性别、民族、出生日期、住址、身份证号"
   ```

3. **表格转Excel**
   ```
   Prompt: "请将图片中的表格转换为CSV格式"
   ```

### 性能说明：
- ⏱️ 单次识别：1-60秒（取决于模型大小和硬件）
- 💾 内存占用：4-8GB（Q4量化版本）
- 🎯 准确率：优于传统OCR，尤其适合复杂场景
</details>

<details>
<summary><b>Q: 如何实现以图找图功能？</b></summary>

**A:** 

**以图找图** 用于在大图中定位小图的位置坐标，常用于滑块验证码识别。

### 使用场景：
- 🧩 滑块验证码缺口定位
- 🔍 在大图中查找特定图标
- 🎯 模板匹配

### 注意事项：
- ✅ 小图应从大图中截取，保证像素级一致
- ✅ 建议小图尺寸 20-100px
- ⚠️ 不支持旋转/缩放匹配
</details>

<details>
<summary><b>Q: 如何识别表格？</b></summary>

**A:** 

### 使用PP-Structure（推荐，v4.3.0+）

### 输出示例（HTML）：
```html
<table>
  <tr>
    <td>姓名</td>
    <td>年龄</td>
    <td>城市</td>
  </tr>
  <tr>
    <td>张三</td>
    <td>25</td>
    <td>北京</td>
  </tr>
</table>
```

### 提高表格识别准确率：
1. ✅ 使用 **SLANet_plus** 模型（最新）
2. ✅ 确保表格线条清晰
3. ✅ 避免表格倾斜（先用UVDoc矫正）
4. ✅ 调整 `table_max_len` 参数（默认488）
</details>

---

## 开发集成

<details>
<summary><b>Q: WebAPI服务如何部署？</b></summary>

**A:** 

### 快速部署：

1. **克隆项目**
   ```bash
   git clone https://github.com/PaddleOCRCore/PaddleOCRApi.git
   cd PaddleOCRApi/OCRCoreService
   ```

2. **配置模型路径**
   编辑 `appsettings.json`，设置正确的模型路径：
   ```json
   {
     "det_model_dir": "D:/Models/PP-OCRv5_det",
     "rec_infer": "D:/Models/PP-OCRv5_rec",
     "cls_model_dir": "D:/Models/cls"
   }
   ```

3. **启动服务**
   ```bash
   # 开发模式
   dotnet run --urls http://*:5000
   
   # 生产模式（后台运行）
   dotnet OCRCoreService.dll --urls http://*:5000
   ```

4. **访问Salar文档**
   ```
   http://localhost:5000/scalar
   ```

### 生产环境部署：

#### 方式一：IIS部署

#### 方式二：Windows服务
```powershell
# 使用NSSM创建Windows服务
nssm install PaddleOCRApi "C:\path\to\dotnet.exe" "C:\path\to\OCRCoreService.dll --urls http://*:5000"
nssm start PaddleOCRApi
```

### 计划任务配置：
详细配置请参考：[计划任务配置说明.md](../OCRCoreService/计划任务配置说明.md)

### 安全建议：
- 🔒 启用HTTPS
- 🔒 配置API密钥认证
- 🔒 限制请求频率
- 🔒 配置防火墙规则
</details>

<details>
<summary><b>Q: 如何在WinForms项目中集成？</b></summary>

**A:** 

### 快速集成步骤：

**安装NuGet包**
   ```xml
   <PackageReference Include="PaddleOCRSDK" Version="4.3.0" />
   <PackageReference Include="PaddleOCRRuntime_x64" Version="4.3.1" />
   ```

### 完整示例：
参考 `Demo/WinFormsApp` 项目，包含：
- ✅ 图片加载与预览
- ✅ 实时识别
- ✅ 检测结果可视化
- ✅ 参数配置界面
- ✅ UVDoc图像矫正
- ✅ OCR-VL识别
</details>

<details>
<summary><b>Q: 如何在C++项目中直接调用？</b></summary>

### 完整示例：
参考 `Demo/CPP/PaddleOCRCpp.cpp`
</details>

---

## GPU授权

<details>
<summary><b>Q: 项目开源吗？CPU和GPU使用是否都免费？</b></summary>

**A:** 

### CPU模式（Windows版本开源免费）
- 许可证：Apache License 2.0
- 使用期限：无限制
- 商业用途：✅ 允许
- 适用范围：文本检测、文字识别、版面识别等 CPU 推理场景

### GPU模式（需要GPU授权）
- GPU推理需要有效的 GPU 授权文件。
- 授权文件默认路径：`models/paddleocr.lic`
- 启用GPU时，程序会在初始化前自动尝试激活默认授权文件。
- CPU模式不强制要求GPU授权文件。
- 同一授权文件可包含多个产品权限，例如 `PaddleOCR`、`PaddleOCR-VL`。

### 授权状态会检查哪些信息？
- 授权是否已激活
- 是否允许GPU
- 授权产品列表
- 授权平台
- 设备绑定状态与机器码是否匹配
- 授权开始时间与到期时间

**联系方式：**
- QQ：**2380243976**
- QQ群：**475159576**
</details>

<details>
<summary><b>Q: 如何申请和使用GPU授权？</b></summary>

**A:** 

### 获取GPU授权申请码

推荐使用 WinForms Demo：

1. 打开 `Demo/WinFormsApp`。
2. 点击顶部菜单 `授权` → `生成授权申请码`。
3. 日志窗口会输出当前机器的 GPU 授权申请码。
4. 将申请码发送给开发者生成授权文件。

也可以在代码中调用：

```csharp
string requestCode = ocrService.GetLicenseRequestCode();
```

### 放置授权文件

将生成的授权文件放到程序运行目录下：

```text
models/paddleocr.lic
```

如果需要自定义路径，可在调用初始化前自行调用：

```csharp
ocrService.ActivateLicense("your-license-file-path");
```

### 查看GPU授权状态

推荐使用 WinForms Demo：

1. 点击顶部菜单 `授权` → `查看GPU授权`。
2. 日志窗口会显示 PaddleOCR 和 PaddleOCR-VL 的授权状态。

也可以在代码中调用：

```csharp
LicenseStatus? status = ocrService.GetLicenseStatusInfo();
```

### 常见失败原因

- 未找到 `models/paddleocr.lic`
- 授权文件已过期
- 授权文件不包含GPU权限
- 授权产品不包含当前模块，例如只授权 `PaddleOCR`，但正在使用 `PaddleOCR-VL`
- 授权文件绑定的机器码与当前设备不匹配
- 系统时间异常导致授权有效期判断失败
</details>

<details>
<summary><b>Q: Linux版本如何获取？</b></summary>

**A:** 

当前开源版本主要面向 **Windows x64**。如需 Linux 动态库、跨平台部署或定制支持，请联系开发者咨询。

**联系方式：**
- QQ：**2380243976**
- QQ群：**475159576**
</details>

<details>
<summary><b>Q: 项目是否允许商业使用？</b></summary>

**A:** 

CPU开源部分遵循 Apache License 2.0，可用于商业项目：

- 许可证：Apache License 2.0
- 使用期限：无限制
- 商业用途：✅ 允许
- 源码获取：GitHub完全公开

GPU授权、Linux版本和定制服务属于独立授权/服务范围，请联系开发者确认授权方式和使用范围。
</details>

---

## 问题排查

<details>
<summary><b>Q: 识别结果为空或错误怎么办？</b></summary>

**A:** 

### 常见原因及解决：

1. **模型路径错误**
   ```
   ❌ 错误：找不到模型文件
   ✅ 解决：检查appsettings.json中的路径是否正确
   ```

2. **图片格式不支持**
   ```
   ❌ 错误：无法解码图片
   ✅ 解决：转换为JPG/PNG格式
   ```

3. **图片质量太差**
   ```
   ❌ 错误：文字模糊不清
   ✅ 解决：提高分辨率，至少300dpi
   ```

4. **内存不足**
   ```
   ❌ 错误：OutOfMemoryException
   ✅ 解决：降低ocr_instance_count，增加物理内存
   ```

5. **DLL依赖缺失**
   ```
   ❌ 错误：找不到paddle_inference.dll
   ✅ 解决：确保所有依赖DLL都在运行目录

</details>

<details>
<summary><b>Q: GPU模式下遇到CUDA错误？</b></summary>

**A:** 

### 常见CUDA错误及解决：

1. **CUDA initialization error**
   ```
   原因：CUDA版本不匹配
   解决：确认CUDA版本与paddle_inference要求一致
         v3.3.0 → CUDA 12.9+
   ```

2. **cuDNN not found**
   ```
   原因：缺少cuDNN DLL
   解决：复制cudnn64_x.dll到运行目录
   ```

3. **Out of GPU memory**
   ```
   原因：显存不足
   解决：降低gpu_mem参数，或关闭其他GPU应用
   ```

4. **TensorRT error**
   ```
   原因：TensorRT版本不兼容
   解决：使用与CUDA匹配的TensorRT版本
   ```

### 验证GPU环境：

```bash
# 检查CUDA版本
nvcc --version

# 检查GPU状态
nvidia-smi

# 检查cuDNN
# 在程序中输出cuDNN版本信息
```

### 回退到CPU模式：
```json
{
  "use_gpu": false,
  "enable_mkldnn": true
}
```
</details>

<details>
<summary><b>Q: WebAPI启动失败？</b></summary>

**A:** 

### 常见启动错误：

1. **端口被占用**
   ```
   错误：Failed to bind to address http://0.0.0.0:5000
   解决：更换端口或关闭占用程序
         dotnet run --urls http://*:5001
   ```

2. **模型加载失败**
   ```
   错误：Cannot load model from path
   解决：检查模型路径，确保文件完整
   ```

3. **.NET版本不匹配**
   ```
   错误：Required .NET 10.0 not found
   解决：安装.NET 10.0 Runtime
         https://dotnet.microsoft.com/download
   ```

4. **权限不足**
   ```
   错误：Access denied
   解决：以管理员身份运行
         或更改端口为 > 1024
   ```

### 诊断步骤：

1. **查看详细错误日志**
   ```bash
   dotnet run --urls http://*:5000 --verbosity detailed
   ```

2. **检查依赖文件**
   ```
   确保以下文件存在：
   - PaddleOCR.dll
   - paddle_inference.dll
   - common.dll
   - mkldnn.dll
   - libiomp5md.dll
   - mklml.dll
   ```

3. **测试最小配置**
   ```json
   {
     "use_gpu": false,
     "ocr_instance_count": 1
   }
   ```
</details>

<details>
<summary><b>Q: 遇到问题如何获取帮助？</b></summary>

**A:** 

### 自助排查流程：

1. **查阅文档**
   - 📖 [README.md](../README.md) - 项目说明
   - 📖 [WebAPI文档](../OCRCoreService/README.md) - 接口文档
   - 📖 [CHANGELOG.md](CHANGELOG.md) - 更新日志
   - 📖 [FAQ.md](FAQ.md) - 常见问题

2. **查看示例代码**
   - `Demo/WinFormsApp/` - C#完整示例
   - `Demo/Python/` - Python示例
   - `Demo/JavaDemo/` - Java示例
   - `Demo/GoDemo/` - Go示例
   - `Demo/CPP/` - C++示例

3. **搜索Issue**
   - GitHub Issues：[查看已有问题](https://github.com/PaddleOCRCore/PaddleOCRApi/issues)

### 寻求帮助渠道：

1. **QQ群交流（推荐）**
   - 群号：**475159576**
   - 活跃度高，响应快
   - 可分享经验和问题

2. **提交GitHub Issue**
   - 标题：【问题】简要描述
   - 内容：
     - 环境信息（系统、.NET版本、OCR版本）
     - 错误日志
     - 复现步骤
     - 相关代码片段

3. **联系开发者**
   - QQ：**2380243976**
   - 适合紧急问题或商业合作

### 提问技巧：

✅ **好的提问：**
```
环境：Windows 11, .NET 10.0, PaddleOCRSDK 4.2.0
问题：GPU模式下识别失败，报错"cudaErrorNoDevice"
已尝试：
1. 确认显卡驱动已更新
2. CUDA 12.9已安装
3. nvidia-smi显示正常
附件：错误日志截图、appsettings.json配置
```

❌ **不好的提问：**
```
不能用，报错了怎么办？
```
</details>

---

## 📞 联系我们

如有其他问题，欢迎通过以下方式联系：

- 💬 **QQ交流群**：475159576
- 👤 **开发者QQ**：2380243976
- 🐙 **GitHub**：[PaddleOCRCore/PaddleOCRApi](https://github.com/PaddleOCRCore/PaddleOCRApi)

**如果您觉得本项目对您有帮助，请给一个免费的 Star ⭐**

---

*最后更新时间：2026-04-29*
