[<img src="https://img.shields.io/badge/Language-简体中文-red.svg">](README.md) [<img src="https://img.shields.io/badge/Language-English-blue.svg">](README_EN.md)
# PaddleOCRApi Offline OCR SDK - Support C#/C++/Java/Python/Go

<p align="center">
    <a href="https://discord.gg/z9xaRVjdbD"><img src="https://img.shields.io/badge/Chat-on%20discord-7289da.svg?sanitize=true" alt="Chat"></a>
    <a href="./LICENSE"><img src="https://img.shields.io/badge/license-Apache%202-dfd.svg"></a>
    <a href="https://github.com/PaddleOCRCore/PaddleOCRApi/releases"><img src="https://img.shields.io/github/v/release/PaddleOCRCore/PaddleOCRApi?color=ffa"></a>
    <a href=""><img src="https://img.shields.io/badge/os-linux%2C%20win%2C%20mac-pink.svg"></a>
    <a href="https://github.com/PaddleOCRCore/PaddleOCRApi/stargazers"><img src="https://img.shields.io/github/stars/PaddleOCRCore/PaddleOCRApi?color=ccf"></a>
</p>

## 📖 Table of Contents

- [Introduction](#-introduction)
- [Features](#-features)
- [Project Structure](#-project-structure)
- [Quick Start](#-quick-start)
- [Runtime Environment](#-runtime-environment)
- [Parameter Description](#-parameter-description)
- [GPU Configuration](#-gpu-configuration)
- [Multi-Language Examples](#-multi-language-examples)
- [Community](#-community)
- [Changelog](#-changelog)

## 🚀 Introduction

Free offline OCR SDK supporting CPU/GPU, free to use and upgrade. Supports C#, C++, Java, Python, and Go development with multi-threading, automatic memory management. Based on Baidu's PaddleOCR with C++ dynamic library wrapper, supports the latest paddle_inference 3.2.2 inference engine.

**If you like this project, please give us a free Star ⭐**

Supports the latest PP-OCRv5_mobile/PP-OCRv5_server models, backward compatible with V4/V3 models.

## ✨ Features

- ✅ **Multi-Language Support**: C#, C++, Java, Python, Go
- ✅ **High Performance**: CPU/GPU inference support, TensorRT acceleration
- ✅ **Easy Integration**: WebAPI service for online calling
- ✅ **Multi-Threading**: Concurrent processing with automatic memory management
- ✅ **Offline Operation**: No internet required, secure data processing
- ✅ **Rich Models**: Support for PP-OCRv5/v4/v3 series models
- ✅ **Comprehensive Features**: Text detection, recognition, orientation classification, table recognition

## 📁 Project Structure

```
PaddleOCRWebApi/
├── PaddleOCRSDK/                  # Core OCR SDK project
│   ├── Services/                  # OCR service implementation
│   │   ├── OCRService.cs         # OCR recognition service
│   │   └── OCRSDK.cs             # SDK core wrapper
│   ├── Interface/                # Interface definitions
│   ├── Models/                   # Data models
│   └── PaddleOCRSDK.csproj      # SDK project file
│
├── OCRCoreService/               # WebAPI service project
│   ├── Controllers/              # API controllers
│   │   ├── OCRServiceController.cs  # OCR endpoints
│   │   └── HomeController.cs        # Home page
│   ├── Services/                 # Business services
│   │   └── OCREngine.cs         # OCR engine
│   ├── Authorization/            # Authorization
│   ├── Extensions/               # Extension methods
│   ├── Utilities/                # Utility classes
│   ├── Views/                    # View files
│   ├── wwwroot/                  # Static resources
│   ├── appsettings.json         # Configuration file
│   └── README.md                # WebAPI documentation
│
├── Demo/                         # Multi-language examples
│   ├── CPP/                     # C++ calling example
│   │   ├── PaddleOCRCpp.cpp    # C++ example code
│   │   └── PaddleOCR.h         # C++ header file
│   ├── Python/                  # Python calling example
│   │   ├── OCRPythonDemo.py    # Python example
│   │   └── OCRTablePythonDemo.py # Table recognition example
│   ├── GoDemo/                  # Go calling example
│   │   └── OCRGoDemo.go        # Go example code
│   └── WinFormsApp/            # C# WinForms example
│       ├── MainForm.cs         # Main form
│       └── Services/           # Service layer
│
├── packages/                    # NuGet package dependencies
│   └── PaddleOCRRuntime_x64.3.2.2/  # Runtime library
│
├── Doc/                        # Documentation
└── README.md                   # Project documentation
```

## 🚀 Quick Start

### 1. NuGet Package Installation (Recommended)

For paddle_inference 3.2+ version:

```xml
<PackageReference Include="PaddleOCRRuntime_x64" Version="3.2.2" />
```

For paddle_inference 2.6.2 version:

```xml
<PackageReference Include="PaddleOCRSDK" Version="1.0.5" />
<PackageReference Include="PaddleOCRRuntime_x64" Version="1.0.0" />
```

### 2. C# Quick Example

```csharp
using PaddleOCRSDK;

// Initialize OCR engine
var ocrService = new OCRService();
ocrService.Initialize(
    detModelPath: "models/PP-OCRv5_mobile_det_infer",
    clsModelPath: "models/PP-LCNet_x1_0_textline_ori",
    recModelPath: "models/PP-OCRv5_mobile_rec_infer",
    keysPath: "models/ppocr_keys.txt"
);

// Recognize image
var result = ocrService.Detect("test.jpg");
Console.WriteLine(result);
```

### 3. WebAPI Service Startup

```bash
# Run WebAPI service
cd OCRCoreService
dotnet run --urls http://*:5000

# Access Swagger documentation
http://localhost:5000/swagger/index.html
```

For detailed WebAPI documentation, please refer to: [WebApi Documentation](./OCRCoreService/README.md)

## 🔧 Runtime Environment

### Basic Requirements

OCRCoreService (WebAPI) and WinForms project require VS2022 + .NET 8.0

### Inference Library Version

1. **Default paddle_inference 3.2.2 CPU version**, other versions can be downloaded manually or compiled

2. **paddle_inference 2.6.2 version** download Release V1.0.5
   - CPU version (included in PaddleOCRRuntime_x64):
   - https://paddle-inference-lib.bj.bcebos.com/2.6.2/cxx_c/Windows/CPU/x86-64_avx-mkl-vs2019/paddle_inference.zip

3. **Core file PaddleOCR.dll** is a C++ dynamic library, supports CPU/GPU mode (GPU requires environment setup)

### .NET Platform Support

Supported frameworks: netstandard2.0; net45; net461; net47; net48; net6.0; net7.0; net8.0; net9.0

### WinFormDemo Preview

<img src="./PaddleOCRSDK/PaddleOCR/ocrDemo.png" width="800px;" />


## 📋 Parameter Description

| Parameter Name               | Default | Description                                                                                   |
| ---------------------------- | ------- | --------------------------------------------------------------------------------------------- |
| det_model_dir                | -       | Detection model inference model path                                                          |
| cls_model_dir                | -       | Direction classifier inference model path                                                     |
| rec_infer                    | -       | Text recognition model inference model path                                                   |
| keys                         | -       | Text recognition dictionary file                                                              |
| table_model_dir              | -       | Table recognition model inference model path                                                  |
| table_char_dict_path         | -       | Table recognition dictionary file                                                             |
| **General Parameters**       | --      | --                                                                                            |
| det                          | true    | Whether to execute text detection                                                             |
| rec                          | true    | Whether to execute text recognition                                                           |
| cls                          | false   | Whether to execute text direction classification                                              |
| use_gpu                      | false   | Whether to use GPU                                                                            |
| gpu_id                       | 0       | GPU id, effective when using GPU                                                              |
| gpu_mem                      | 4000    | GPU memory usage                                                                              |
| use_tensorrt                 | false   | Whether to enable TensorRT when using GPU prediction                                          |
| cpu_mem                      | 4000    | CPU memory usage limit in MB. -1 means no limit                                               |
| cpu_math_library_num_threads | 10      | Number of threads for CPU prediction, larger value means faster prediction with sufficient cores |
| enable_mkldnn                | true    | Whether to use mkldnn library, disabling reduces memory usage but decreases speed             |
| **Detection Model**          | --      | --                                                                                            |
| max_side_len                 | 960     | When input image is larger than 960, scale proportionally to make longest side 960            |
| det_db_thresh                | 0.3     | Threshold for filtering DB prediction binarized image, 0-0.3 has minimal effect               |
| det_db_box_thresh            | 0.5     | DB post-processing box filtering threshold, reduce if detection misses boxes                  |
| det_db_unclip_ratio          | 1.6     | Text box tightness, smaller value means box closer to text                                    |
| use_dilation                 | false   | Whether to use dilation on output map                                                         |
| det_db_score_mode            | true    | true: use polygon to calculate bbox score, false: use rectangle. Rectangle is faster, polygon is more accurate for curved text |
| visualize                    | false   | Whether to visualize results, saves to output folder with same name as input image            |
| **Orientation Classifier**   | --      | --                                                                                            |
| use_angle_cls                | false   | Whether to use orientation classifier                                                         |
| cls_thresh                   | 0.9     | Orientation classifier score threshold                                                        |
| cls_batch_num                | 1       | Orientation classifier batch recognition quantity                                             |
| **Recognition Model**        | --      | --                                                                                            |
| rec_batch_num                | 6       | Text recognition model batch recognition quantity                                             |
| rec_img_h                    | 48      | Text recognition model input image height                                                     |
| rec_img_w                    | 320     | Text recognition model input image width                                                      |
| **Table Recognition Model**  | --      | --                                                                                            |
| table_max_len                | 488     | Table recognition model input image long side size, final network input size is (table_max_len, table_max_len) |
| merge_empty_cell             | true    | Whether to merge empty cells                                                                  |
| table_batch_num              | 1       | table_batch_num                                                                               |

## 🎯 Multi-Language Examples

### C# Example

```csharp
// See Demo/WinFormsApp/
var ocrService = new OCRService();
ocrService.Initialize(detModelPath, clsModelPath, recModelPath, keysPath);
var result = ocrService.Detect(imagePath);
```

### Python Example

```python
# See Demo/Python/OCRPythonDemo.py
import ctypes

ocr_dll = ctypes.CDLL("PaddleOCR.dll")
init_func = ocr_dll.Initjson
detect_func = ocr_dll.Detect

# Initialize
init_func(det_model_path, cls_model_path, rec_model_path, keys_path)
# Recognize
result = detect_func(image_path)
```

### Go Example

```go
// See Demo/GoDemo/OCRGoDemo.go
ocrDLL, _ := syscall.LoadDLL("PaddleOCR.dll")
initFunc, _ := ocrDLL.FindProc("Initjson")
detectFunc, _ := ocrDLL.FindProc("Detect")

// Initialize and call
initFunc.Call(detModelPath, clsModelPath, recModelPath, keysPath)
detectFunc.Call(imagePath)
```

### C++ Example

```cpp
// See Demo/CPP/PaddleOCRCpp.cpp
#include <PaddleOCR.h>

// Initialize
Initjson(detModelPath, clsModelPath, recModelPath, keysPath);
// Recognize
char* result = Detect(imagePath);
```

For more complete examples, please check the `Demo/` directory for each language example code.

## 🖥️ GPU Configuration

### paddle_inference 2.6.2 GPU Version

**Download Link**: [paddle_inference2.6.2](https://www.paddlepaddle.org.cn/inference/v2.6/guides/install/download_lib.html#windows)
- https://paddle-inference-lib.bj.bcebos.com/2.6.2/cxx_c/Windows/GPU/x86-64_cuda12.0_cudnn8.9.1_trt8.6.1.6_mkl_avx_vs2019/paddle_inference.zip

**Configuration Steps**:

1. Extract and copy the following DLL files to the program running folder:
   - `paddle\lib\` directory: `common.dll`, `paddle_inference.dll`
   - `third_party\install\mkldnn\lib\` directory: `mkldnn.dll`
   - `third_party\install\mklml\lib\` directory: `libiomp5md.dll`, `mklml.dll`

2. Install CUDA and CUDNN, copy the corresponding cublas64_12.dll、cublasLt64_12.dll、cudnn_cnn64_9.dll、cudnn_engines_precompiled64_9.dll、cudnn_engines_runtime_compiled64_9.dll、cudnn_graph64_9.dll、cudnn_heuristic64_9.dll、cudnn_ops64_9.dll、cudnn64_9.dll
   - Located at: `C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v12.x\bin`

### paddle_inference 3.x GPU Version

⚠️ **Note**: Official GPU version inference library is not available yet, needs self-compilation or contact author

**Configuration Steps**:

1. Extract and copy the following DLL files to the program running folder:
   - `paddle\lib\` directory: `common.dll`, `paddle_inference.dll`
   - `third_party\install\mkldnn\lib\` directory: `mkldnn.dll`
   - `third_party\install\mklml\lib\` directory: `libiomp5md.dll`, `mklml.dll`

2. Install CUDA and CUDNN, copy the corresponding cudnn64_x.dll
   - Located at: `C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v12.x\bin\cudnn64_x.dll`

### Related Download Links

| Resource | Link |
|----------|------|
| CUDA | [https://developer.nvidia.com/cuda-toolkit-archive](https://developer.nvidia.com/cuda-toolkit-archive) |
| CUDNN | [https://developer.nvidia.cn/rdp/cudnn-archive](https://developer.nvidia.cn/rdp/cudnn-archive) |
| TensorRT | [https://developer.nvidia.com/nvidia-tensorrt-download](https://developer.nvidia.com/nvidia-tensorrt-download) |
| PP-OCRv4/v5 Models | [https://www.paddleocr.ai/latest/version3.x/pipeline_usage/OCR.html](https://www.paddleocr.ai/latest/version3.x/pipeline_usage/OCR.html) |

## 🔗 WebAPI Interface

For detailed WebAPI documentation, please refer to: [WebApi Documentation](./OCRCoreService/README.md)

**Main Endpoints**:
- `POST /OCRService/GetOCRText` - Image OCR recognition (Base64 upload)
- `POST /OCRService/GetOCRFile` - Image OCR recognition (File upload)

**Swagger Documentation**: `http://localhost:5000/swagger/index.html`

## 💬 Community

Welcome to join QQ group **475159576** for discussion, or add QQ for custom projects: **2380243976**

If you like this project, please give us a free **Star ⭐**

<img src="./PaddleOCRSDK/PaddleOCR/qq.png" width="200px;" />

## ☕ Donation

If this project helps you, please scan the QR code below to buy us a coffee.

<img src="./PaddleOCRSDK/PaddleOCR/donate.jpg" width="200px;" />

## 🎯 Technical Architecture

### Core Components

```
┌─────────────────────────────────────────┐
│      Application Layer                  │
│  WinForms / WebAPI / Console / SDK      │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│    .NET Wrapper (PaddleOCRSDK)          │
│    OCRService / IOCRService / Models    │
└─────────────────┬───────────────────────┘
                  │ P/Invoke
┌─────────────────▼───────────────────────┐
│    C++ Library (PaddleOCR.dll)          │
│  Detection / Recognition / Cls / Table  │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│    Paddle Inference Engine              │
│    paddle_inference 3.2.2 / 2.6.2       │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      Hardware Acceleration              │
│     CPU (MKL) / GPU (CUDA+TensorRT)     │
└─────────────────────────────────────────┘
```

### Workflow

1. **Image Preprocessing** → Image normalization, size adjustment
2. **Text Detection** → DBNet detects text regions
3. **Orientation Classification** → Text direction correction (optional)
4. **Text Recognition** → CRNN recognizes text content
5. **Result Output** → JSON/text format return

## 📝 Changelog

### v3.2.2 `2025.12.11`
- ✅ Optimized PaddleOCR.dll, supports paddle_inference 3.2.2 inference library
- ✅ Released PaddleOCRRuntime_x64 v3.2.2, includes paddle 3.2.2 inference library, PaddleOCR.dll and dependencies
- ⚠️ NuGet PaddleOCRSDK stops updating, core files integrated into PaddleOCRRuntime_x64, .NET projects refer to PaddleOCRSDK source code

### v3.1.0 `2025.9.15`
- ✅ Optimized PaddleOCR.dll, supports paddle_inference 3.2.0 inference library
- ✅ Added support for text line orientation classification model PP-LCNet_x1_0_textline_ori
- ✅ V4/V5 models use yml format
- ✅ Table recognition initialization adds orientation classification model parameter, can use table recognition independently
- ✅ Released PaddleOCRRuntime_x64 v3.1.1
- ✅ Released PaddleOCRSDK v3.1.0, aligned with PaddleOCR.dll

### v2.1.1 `2025.8.1`
- ✅ Released PaddleOCRSDK 2.1.1, added DetectMat interface

### v2.1.0 `2025.7.31`
- ✅ Modified PaddleOCR.dll interface, pointer type changed to char* (UTF8 encoding)
- ✅ Added DetectMat interface supporting direct Mat input
- ✅ EnableANSIResult renamed to EnableASCIIResult
- ✅ Released PaddleOCRSDK 2.1.0

### v2.0.0 `2025.6.4`
- ✅ Modified PaddleOCR.dll interface, added support for PP-OCRv5 model
- ✅ WinForm Demo added V5/V4 model selection dropdown

### v1.0.5 `2025.4.1`
- ✅ Optimized PaddleOCR.dll interface, Demo added table recognition feature

### v1.0.4 `2025.3.29`
- ✅ Optimized PaddleOCR.dll, added log output switch, OCR recognition speed improvement
- ✅ WebApi interface optimization, added OCR initialization and parameter settings

### v1.0.2 `2025.3.23`
- ✅ Optimized PaddleOCR.dll, added multi-thread queue support
- ✅ Added automatic memory recycling when limit reached
- ✅ WinFormDemo feature enhancement, added initialization options
- ✅ Added multi-image selection and concurrent testing simulation

### v1.0.1 `2025.3.5`
- ✅ Optimized PaddleOCR.dll, improved recognition speed, added smart pointers

### v1.0 `2025.1.22`
- 🎉 Initial release: PaddleOCRApi

## 🔍 FAQ

<details>
<summary><b>Q: How to choose between CPU and GPU version?</b></summary>

**A:** 
- CPU version: Suitable for small batch recognition, simple deployment, no GPU environment required
- GPU version: Suitable for large batch recognition, faster speed, requires CUDA12.9 environment support
</details>

<details>
<summary><b>Q: How to improve recognition accuracy?</b></summary>

**A:** 
1. Choose the appropriate model (mobile/server)
2. Adjust `det_db_thresh`, `det_db_box_thresh` parameters
3. Enable orientation classifier `use_angle_cls=true`
4. Preprocess images (denoising, binarization, etc.)
</details>

<details>
<summary><b>Q: What image formats are supported?</b></summary>

**A:** Supports common image formats: jpg, jpeg, png, bmp, tiff, etc.
</details>

<details>
<summary><b>Q: How to use on Linux/Mac?</b></summary>

**A:** 
- Need to compile PaddleOCR.so/.dylib dynamic library for the corresponding platform
- Or deploy WebAPI service using Docker container
</details>

## 🙏 Acknowledgments

This project is based on the following open source projects:
- [PaddleOCR](https://github.com/PaddlePaddle/PaddleOCR) - Baidu PaddleOCR toolkit
- [Paddle Inference](https://www.paddlepaddle.org.cn/inference/master/guides/introduction/index_intro.html) - PaddlePaddle inference engine

## ⭐️ Star

[![Star History Chart](https://api.star-history.com/svg?repos=PaddleOCRCore/PaddleOCRApi&type=Date)](https://star-history.com/#PaddleOCRCore/PaddleOCRApi&Date)

## 📄 License

This project is released under [Apache License Version 2.0](./LICENSE). Welcome to use and contribute.
