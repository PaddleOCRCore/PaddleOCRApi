# 🔍 FAQ (Frequently Asked Questions)

> 💡 **Tip**: This document compiles the most common questions and solutions for using PaddleOCRWebApi. If you don't find your answer, please join QQ group **475159576** for discussion.

---

## 📋 Table of Contents

- [Version Selection](#version-selection)
- [Environment Configuration](#environment-configuration)
- [Models](#models)
- [Performance Optimization](#performance-optimization)
- [Feature Usage](#feature-usage)
- [Development Integration](#development-integration)
- [GPU License](#gpu-license)
- [Troubleshooting](#troubleshooting)

---

## Version Selection

<details>
<summary><b>Q: How to choose between CPU and GPU version?</b></summary>

**A:** 

| Version | Use Case | Advantages | Requirements |
|---------|----------|------------|--------------|
| **CPU Version** | Small batch recognition, low concurrency | Simple deployment, no extra configuration | No special requirements |
| **GPU Version** | Large batch recognition, high concurrency | 3-10x faster | CUDA 12.9+ (v3.x) or CUDA 12.9 (v2.6.2) |

**Recommendations:**
- Daily processing < 1000 images → CPU version
- Daily processing > 1000 images → GPU version
- High real-time requirements → GPU version + TensorRT acceleration
</details>

---

## Environment Configuration

<details>
<summary><b>Q: How to configure GPU runtime environment on Windows?</b></summary>

**A:** 

### GPU Configuration Steps:

1. **Install CUDA 12.9+**
   - Download: [CUDA Toolkit Archive](https://developer.nvidia.com/cuda-toolkit-archive)
   - Verify after installation: `nvcc --version`

2. **Install cuDNN 9.x**
   - Download: [cuDNN Archive](https://developer.nvidia.com/cudnn-archive)
   - Copy DLLs from `bin` directory to application runtime directory

3. **Copy Paddle Inference Dependencies**

4. **Configure appsettings.json**
   ```json
   {
     "use_gpu": true,
     "gpu_id": 0,
     "gpu_mem": 4000,
     "use_tensorrt": false
   }
   ```

### Common Issues:
- ❌ **Cannot find cudnn64_9.dll** → Verify cuDNN version matches CUDA version
- ❌ **GPU initialization failed** → Check if graphics driver supports CUDA 12.9+
- ❌ **Insufficient VRAM** → Reduce `gpu_mem` parameter value
</details>

<details>
<summary><b>Q: How to use on Linux?</b></summary>

**A:** 

The current open-source version only supports **Windows x64** platform. For Linux support:

1. **Option 1: Deploy WebAPI Service (Recommended)**   

2. **Option 2: Custom Linux Version**
   - Need to compile PaddleOCR.so dynamic library for Linux platform
   - Contact developer QQ: **2380243976** for custom services

</details>

<details>
<summary><b>Q: How to reference SDK in .NET projects?</b></summary>

**A:** 

### Option 1: NuGet Package Installation (Recommended)

```xml
<!-- paddle_inference 3.4.0 version -->
<PackageReference Include="PaddleOCRSDK" Version="4.5.1 />
<PackageReference Include="PaddleOCRRuntime_x64" Version="4.5.1" />
```

### Option 2: Manual Reference

1. Download corresponding version from GitHub Release
2. Add `PaddleOCRSDK.dll` to project references
3. Ensure all dependency files are in the runtime directory

### Supported .NET Versions:
✅ netstandard2.0, net45-net48, net6.0-net10.0
</details>

---

## Models

<details>
<summary><b>Q: Which OCR models are supported?</b></summary>

**A:** 

### Standard OCR Models:
- ✅ **PP-OCRv6_medium** (Recommended, highest accuracy)
- ✅ **PP-OCRv6_small** (Recommended, fastest speed)
- ✅ **PP-OCRv5_server** (Recommended, highest accuracy)
- ✅ **PP-OCRv5_mobile** (Recommended, fastest speed)
- ✅ **PP-OCRv4_server/mobile** (Backward compatible)
- ✅ **Custom trained models**

### PP-Structure Layout Analysis Models (v4.3.0+):
- 📊 **PP-DocLayoutV2/PP-DocLayoutV3**: 20-class document element detection
- 📋 **SLANet_plus**: Table structure recognition
- 🔢 **UniMERNet**: Formula recognition
- 🔴 **Seal recognition model**
- 📈 **Chart-to-table model**

### OCR-VL Vision-Language Models (v4.3.0+):
- 🤖 **PaddleOCR-VL-1.6-GGUF**
- 🤖 **PaddleOCR-VL-1.5-GGUF**
- 🤖 **DeepSeek-OCR-GGUF**
- 🤖 **Qwen2-VL-OCR-GGUF**
- 🤖 **FireRed-OCR-GGUF**

**Model Downloads:**
- Official models: [PaddleOCR Official Website](https://paddlepaddle.github.io/PaddleX/latest/pipeline_usage/tutorials/ocr_pipelines/OCR.html#11)
- GGUF models: [ModelScope](https://www.modelscope.cn/models?name=OCR%20GGUF)
</details>

<details>
<summary><b>Q: How to choose the right model?</b></summary>

**A:** 

| Scenario | Recommended Model | Reason |
|----------|------------------|---------|
| General document recognition | PP-OCRv5_server | High accuracy, suitable for most scenarios |
| Mobile/Embedded devices | PP-OCRv5_mobile | Small size, fast speed |
| Table documents | PP-Structure + SLANet_plus | Specially optimized for table recognition |
| Documents with formulas | PP-Structure + UniMERNet | Supports mathematical formula recognition |
| Complex layouts | OCR-VL + PaddleOCR-VL-1.5 | Semantic understanding, intelligent recognition |
| Low-spec devices | PP-OCRv4_mobile | Low resource consumption |

**How to Switch Models:**
Modify model paths in `appsettings.json`:
```json
{
  "det_model_dir": "models/PP-OCRv5_server_det",
  "rec_infer": "models/PP-OCRv5_server_rec",
  "cls_model_dir": "models/PP-LCNet_x1_0_textline_ori"
}
```
</details>

<details>
<summary><b>Q: How to use custom trained models?</b></summary>

**A:** 

1. **Prepare Model Files**
   - Ensure exported in inference format (contains `.json` and `.pdiparams`)
   - Or use PaddleOCR's export tool

2. **Place Model Files**
   ```
   your_project/
   └── models/
       ├── custom_det/      # Detection model
       ├── custom_rec/      # Recognition model
       └── custom_cls/      # Classification model (optional)
   ```

3. **Configure Model Paths**
   ```json
   {
     "det_model_dir": "models/custom_det",
     "rec_infer": "models/custom_rec",
     "cls_model_dir": "models/custom_cls"
   }
   ```

4. **Reinitialize OCR Engine**
   ```csharp
   var ocr = new OCRService();
   ocr.Init(det_model_dir, rec_infer, cls_model_dir);
   ```
</details>

---

## Performance Optimization

<details>
<summary><b>Q: How to improve recognition accuracy?</b></summary>

**A:** 

### 1. Model-Level Optimization
```json
{
  // Use server version models (higher accuracy)
  "det_model_dir": "models/PP-OCRv5_server_det",
  "rec_infer": "models/PP-OCRv5_server_rec",
  
  // Enable orientation classifier
  "use_angle_cls": true,
  "cls_thresh": 0.9
}
```

### 2. Detection Parameter Tuning
```json
{
  // Lower thresholds detect more text (but may increase false positives)
  "det_db_thresh": 0.2,        // Default 0.3
  "det_db_box_thresh": 0.4,    // Default 0.5
  
  // Adjust text box tightness
  "det_db_unclip_ratio": 1.8   // Default 1.6, larger = looser boxes
}
```

### 3. Image Preprocessing
- ✅ **Resolution optimization**: Ensure text height ≥ 30px
- ✅ **Denoising**: Use `ImageBeauty` utility class
- ✅ **Contrast enhancement**: Improve text-background contrast
- ✅ **Binarization**: Try binarization for scanned documents

### 4. Recognition Parameter Optimization
```json
{
  // Increase recognition confidence threshold
  "rec_img_h": 48,             // Keep default
  "rec_img_w": 320,            // Can increase for long text
  
  // Enable single character coordinate output (for post-processing)
  "return_word_box": true
}
```

### 5. Special Scenario Handling
- **Tilted documents** → Enable `UVDoc` image correction
- **Complex layouts** → Use `PP-Structure` layout analysis
- **Handwriting** → Use specially trained handwriting models
- **Low-quality images** → Perform super-resolution processing first
</details>

<details>
<summary><b>Q: How to improve recognition speed?</b></summary>

**A:** 

### CPU Mode Optimization:

1. **Enable MKLDNN Acceleration**
   ```json
   {
     "enable_mkldnn": true,    // Key! 30-50% speedup
     "cpu_threads": 30         // Adjust based on CPU cores
   }
   ```

2. **Adjust Batch Size**
   ```json
   {
     "rec_batch_num": 6,       // Appropriately increase batch size
     "cls_batch_num": 1
   }
   ```

3. **Limit Input Image Size**
   ```json
   {
     "max_side_len": 960       // Oversized images significantly reduce speed
   }
   ```

### GPU Mode Optimization:

1. **Enable TensorRT**
   ```json
   {
     "use_gpu": true,
     "use_tensorrt": true,     // 2-5x speedup
     "gpu_mem": 4000
   }
   ```

2. **Use Mobile Version Models**
   - PP-OCRv5_mobile is about 40% faster than server version

### High Concurrency Optimization:

1. **Multi-Instance Configuration**
   ```json
   {
     "ocr_instance_count": 5   // Max 10, adjust based on memory
   }
   ```

2. **Asynchronous Calls**
   ```csharp
   // WebAPI already supports async processing
   var result = await ocr.DetectAsync(imageBytes);
   ```
</details>

<details>
<summary><b>Q: What to do if memory usage is too high?</b></summary>

**A:** 

### Root Cause Analysis:
1. Too many OCR instances created
2. Image processing not released in time
3. MKLDNN cache accumulation

### Solutions:

1. **Limit Instance Count**
   ```json
   {
     "ocr_instance_count": 3   // Adjust based on actual concurrency
   }
   ```

2. **Limit CPU Memory**
   ```json
   {
     "cpu_mem": 2000           // Unit: MB, -1 means unlimited
   }
   ```

3. **Disable MKLDNN (Trade Speed for Memory)**
   ```json
   {
     "enable_mkldnn": false    // Can reduce memory usage by 30-50%
   }
   ```
</details>

---

## Feature Usage

<details>
<summary><b>Q: What image formats are supported?</b></summary>

**A:** 

### Supported Formats:
✅ JPG/JPEG  
✅ PNG  
✅ BMP  
✅ PDF  

### Best Practices:
- 📸 **Recommended format**: JPG (good compression) or PNG (lossless)
- 📏 **Recommended resolution**: Width 1000-3000px
- 📐 **Recommended DPI**: 300dpi or higher (scanned documents)
- 💾 **File size**: Single image < 10MB

### Unsupported Formats:
❌ GIF (animated) → Extract first frame and convert  
❌ SVG (vector) → Convert to bitmap  
❌ PSD → Export as PNG/JPG  
</details>

<details>
<summary><b>Q: How to perform document image correction (UVDoc)?</b></summary>

**A:** 

**UVDoc** is used to correct distortion, tilt, and perspective deformation in photographed documents.

### Usage Methods:

#### Method 1: WebAPI Call

#### Method 2: C# SDK Call

#### Method 3: Python Call

### Application Scenarios:
- 📱 Document photos taken with mobile phones
- 📄 Tilted scanned paper documents
- 📐 Perspective-deformed book page photos
- 🔄 Curved receipts/invoices

### Effect Comparison:
Before correction → Text tilted, perspective distorted  
After correction → Text horizontal, layout regular  
**Recognition accuracy improvement: 20-40%**
</details>

<details>
<summary><b>Q: How to use PP-Structure layout analysis?</b></summary>

**A:** 

**PP-Structure** is an advanced feature added in v4.3.0, supporting 20-class document element recognition.

### Features:
- 📊 Layout detection (titles, paragraphs, images, tables, etc.)
- 📋 Table structure recognition (outputs HTML/Markdown)
- 🔢 Formula recognition (LaTeX format)
- 🔴 Seal recognition
- 📈 Chart-to-table conversion

### Usage Methods:

#### C# SDK Call

#### WebAPI Call

### Notes:
⚠️ Need to download related models separately (large size)  
⚠️ Recommended only when structured output is needed  
</details>

<details>
<summary><b>Q: How to use OCR-VL vision-language model?</b></summary>

**A:** 

**OCR-VL** is an advanced recognition module based on vision-language large models (v4.3.0+).

### Features:
- 🎯 **Prompt-guided recognition**: Specify content to extract
- 📄 **Intelligent layout understanding**: Understand document structure and semantics
- 🔍 **Targeted information extraction**: Extract only interested information
- 📊 **Structured output**: Directly output JSON/Markdown

### Supported Models:
- PaddleOCR-VL-1.5-GGUF
- DeepSeek-OCR-GGUF
- Qwen2-VL-OCR-GGUF
- FireRed-OCR-GGUF

### Configuration Method:

1. **Download GGUF Model**
   ```bash
   # Download from ModelScope
   # Example: PaddleOCR-VL-1.5-Q4_K_M.gguf
   ```

2. **Configure appsettings.json**
   ```json
   {
     "OCRVLConfig": {
       "enabled": true,
       "yaml_path": "configs/PaddleOCR-VL-1.5.yaml"
     }
   }
   ```

3. **Edit YAML Configuration File**
   ```yaml
   # configs/PaddleOCR-VL-1.5.yaml
   model_path: "models/PaddleOCR-VL-1.5-Q4_K_M.gguf"
   chat_template: "paddleocr-vl"
   ```

### Typical Application Scenarios:

1. **Invoice Information Extraction**
   ```
   Prompt: "Please extract invoice code, invoice number, issue date, total amount with tax"
   ```

2. **ID Card Recognition**
   ```
   Prompt: "Please extract name, gender, ethnicity, date of birth, address, ID number"
   ```

3. **Table to Excel Conversion**
   ```
   Prompt: "Please convert the table in the image to CSV format"
   ```

### Performance Notes:
- ⏱️ Single recognition: 1-60 seconds (depends on model size and hardware)
- 💾 Memory usage: 4-8GB (Q4 quantized version)
- 🎯 Accuracy: Better than traditional OCR, especially suitable for complex scenarios
</details>

<details>
<summary><b>Q: How to implement image matching (find image in image)?</b></summary>

**A:** 

**Image matching** is used to locate small images within large images, commonly used for slider CAPTCHA recognition.

### Use Cases:
- 🧩 Slider CAPTCHA gap positioning
- 🔍 Find specific icons in large images
- 🎯 Template matching

### Notes:
- ✅ Small image should be cropped from large image for pixel-level consistency
- ✅ Recommended small image size: 20-100px
- ⚠️ Rotation/scaling matching not supported
</details>

<details>
<summary><b>Q: How to recognize tables?</b></summary>

**A:** 

### Use PP-Structure (Recommended, v4.3.0+)

### Output Example (HTML):
```html
<table>
  <tr>
    <td>Name</td>
    <td>Age</td>
    <td>City</td>
  </tr>
  <tr>
    <td>Zhang San</td>
    <td>25</td>
    <td>Beijing</td>
  </tr>
</table>
```

### Improve Table Recognition Accuracy:
1. ✅ Use **SLANet_plus** model (latest)
2. ✅ Ensure table lines are clear
3. ✅ Avoid table tilt (correct with UVDoc first)
4. ✅ Adjust `table_max_len` parameter (default 488)
</details>

---

## Development Integration

<details>
<summary><b>Q: How to deploy WebAPI service?</b></summary>

**A:** 

### Quick Deployment:

1. **Clone Project**
   ```bash
   git clone https://github.com/PaddleOCRCore/PaddleOCRApi.git
   cd PaddleOCRApi/OCRCoreService
   ```

2. **Configure Model Paths**
   Edit `appsettings.json`, set correct model paths:
   ```json
   {
     "det_model_dir": "D:/Models/PP-OCRv5_det",
     "rec_infer": "D:/Models/PP-OCRv5_rec",
     "cls_model_dir": "D:/Models/cls"
   }
   ```

3. **Start Service**
   ```bash
   # Development mode
   dotnet run --urls http://*:5000
   
   # Production mode (background running)
   dotnet OCRCoreService.dll --urls http://*:5000
   ```

4. **Access Scalar Documentation**
   ```
   http://localhost:5000/scalar
   ```

### Production Environment Deployment:

#### Option 1: IIS Deployment

#### Option 2: Windows Service
```powershell
# Create Windows service using NSSM
nssm install PaddleOCRApi "C:\path\to\dotnet.exe" "C:\path\to\OCRCoreService.dll --urls http://*:5000"
nssm start PaddleOCRApi
```

### Scheduled Task Configuration:
For detailed configuration, refer to: [Scheduled Task Configuration Guide](../OCRCoreService/计划任务配置说明.md)

### Security Recommendations:
- 🔒 Enable HTTPS
- 🔒 Configure API key authentication
- 🔒 Limit request frequency
- 🔒 Configure firewall rules
</details>

<details>
<summary><b>Q: How to integrate in WinForms projects?</b></summary>

**A:** 

### Quick Integration Steps:

**Install NuGet Packages**
   ```xml
   <PackageReference Include="PaddleOCRSDK" Version="4.5.0" />
   <PackageReference Include="PaddleOCRRuntime_x64" Version="4.5.0" />
   ```

### Complete Example:
Refer to `Demo/WinFormsApp` project, includes:
- ✅ Image loading and preview
- ✅ Real-time recognition
- ✅ Detection result visualization
- ✅ Parameter configuration interface
- ✅ UVDoc image correction
- ✅ OCR-VL recognition
</details>

<details>
<summary><b>Q: How to call directly in C++ projects?</b></summary>

### Complete Example:
Refer to `Demo/CPP/PaddleOCRCpp.cpp`
</details>

---

## GPU License

<details>
<summary><b>Q: Is the project open source? Are both CPU and GPU modes free?</b></summary>

**A:** 

### CPU Mode (Open Source and Free)
- License: Apache License 2.0
- Usage period: Unlimited
- Commercial use: ✅ Allowed
- Scope: CPU inference scenarios such as text detection, text recognition, and layout recognition

### GPU Mode (GPU License Required)
- GPU inference requires a valid GPU license file.
- Default license file path: `models/paddleocr.lic`
- When GPU is enabled, the program automatically tries to activate the default license file before initialization.
- CPU mode does not require a GPU license file.
- One license file can include permissions for multiple products, such as `PaddleOCR` and `PaddleOCR-VL`.

### What does the license status check include?
- Whether the license is activated
- Whether GPU is allowed
- Licensed product list
- Licensed platforms
- Device binding status and whether the machine code matches
- License start time and expiration time

**Contact Information:**
- QQ: **2380243976**
- QQ Group: **475159576**
</details>

<details>
<summary><b>Q: How do I request and use a GPU license?</b></summary>

**A:** 

### Get the GPU License Request Code

The recommended way is to use the WinForms Demo:

1. Open `Demo/WinFormsApp`.
2. Click the top menu `License` → `Generate License Request Code`.
3. The log window will print the GPU license request code for the current machine.
4. Send the request code to the developer to generate a license file.

You can also call it in code:

```csharp
string requestCode = ocrService.GetLicenseRequestCode();
```

### Place the License File

Place the generated license file under the application runtime directory:

```text
models/paddleocr.lic
```

If you need a custom path, call this before initialization:

```csharp
ocrService.ActivateLicense("your-license-file-path");
```

### Check GPU License Status

The recommended way is to use the WinForms Demo:

1. Click the top menu `License` → `Check GPU License`.
2. The log window will show the license status for PaddleOCR and PaddleOCR-VL.

You can also call it in code:

```csharp
LicenseStatus? status = ocrService.GetLicenseStatusInfo();
```

### Common Failure Reasons

- `models/paddleocr.lic` is missing
- The license file has expired
- The license file does not include GPU permission
- The licensed product list does not include the module being used, for example only `PaddleOCR` is licensed but `PaddleOCR-VL` is running
- The machine code bound in the license does not match the current device
- System time is incorrect and causes validity-period checks to fail
</details>

<details>
<summary><b>Q: How do I get the Linux version?</b></summary>

**A:** 

The current open-source version mainly targets **Windows x64**. For Linux dynamic libraries, cross-platform deployment, or custom support, contact the developer.

**Contact Information:**
- QQ: **2380243976**
- QQ Group: **475159576**
</details>

<details>
<summary><b>Q: Is commercial use allowed?</b></summary>

**A:** 

The CPU open-source part follows Apache License 2.0 and can be used in commercial projects:

- License: Apache License 2.0
- Usage period: Unlimited
- Commercial use: ✅ Allowed
- Source code: Fully public on GitHub

GPU licenses, Linux versions, and custom services are separate license/service scopes. Contact the developer to confirm the license terms and usage scope.
</details>

---

## Troubleshooting

<details>
<summary><b>Q: What to do if recognition result is empty or erroneous?</b></summary>

**A:** 

### Common Causes and Solutions:

1. **Incorrect Model Path**
   ```
   ❌ Error: Cannot find model file
   ✅ Solution: Check if path in appsettings.json is correct
   ```

2. **Unsupported Image Format**
   ```
   ❌ Error: Cannot decode image
   ✅ Solution: Convert to JPG/PNG format
   ```

3. **Poor Image Quality**
   ```
   ❌ Error: Text is blurry
   ✅ Solution: Increase resolution, at least 300dpi
   ```

4. **Insufficient Memory**
   ```
   ❌ Error: OutOfMemoryException
   ✅ Solution: Reduce ocr_instance_count, increase physical memory
   ```

5. **Missing DLL Dependencies**
   ```
   ❌ Error: Cannot find paddle_inference.dll
   ✅ Solution: Ensure all dependent DLLs are in runtime directory
   ```
</details>

<details>
<summary><b>Q: Encountering CUDA errors in GPU mode?</b></summary>

**A:** 

### Common CUDA Errors and Solutions:

1. **CUDA initialization error**
   ```
   Cause: CUDA version mismatch
   Solution: Verify CUDA version matches paddle_inference requirements
            v3.3.0 → CUDA 12.9+
   ```

2. **cuDNN not found**
   ```
   Cause: Missing cuDNN DLL
   Solution: Copy cudnn64_x.dll to runtime directory
   ```

3. **Out of GPU memory**
   ```
   Cause: Insufficient VRAM
   Solution: Reduce gpu_mem parameter, or close other GPU applications
   ```

4. **TensorRT error**
   ```
   Cause: TensorRT version incompatibility
   Solution: Use TensorRT version compatible with CUDA
   ```

### Verify GPU Environment:

```bash
# Check CUDA version
nvcc --version

# Check GPU status
nvidia-smi

# Check cuDNN
# Output cuDNN version info in program
```

### Fallback to CPU Mode:
```json
{
  "use_gpu": false,
  "enable_mkldnn": true
}
```
</details>

---

## 📞 Contact Us

For other questions, feel free to contact us through:

- 💬 **QQ Group**: 475159576
- 👤 **Developer QQ**: 2380243976
- 🐙 **GitHub**: [PaddleOCRCore/PaddleOCRApi](https://github.com/PaddleOCRCore/PaddleOCRApi)

**If this project helps you, please give it a free Star ⭐**

---

*Last updated: 2026-04-29*
