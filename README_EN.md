[<img src="https://img.shields.io/badge/Language-简体中文-red.svg">](README.md) [<img src="https://img.shields.io/badge/Language-English-blue.svg">](README_EN.md)
# PaddleOCRApi Offline OCR SDK - Support C#、C++、Java、Python、Go

<p align="center">
    <a href="./LICENSE"><img src="https://img.shields.io/badge/license-Apache%202-dfd.svg"></a>
    <a href="https://github.com/PaddleOCRCore/PaddleOCRApi/releases"><img src="https://img.shields.io/github/v/release/PaddleOCRCore/PaddleOCRApi?color=ffa"></a>
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
- [Community](#-community)
- [Changelog](#-changelog)

### WinFormDemo Preview

<img src="./PaddleOCRSDK/Images/ocrDemo.png" width="800px;" />

## 🚀 Introduction

A completely offline Chinese character recognition component based on Baidu's PaddleOCR deep encapsulation, providing a simple and easy-to-use API interface that supports C#/C++/Java/Python/Go and other development languages. Completely free to use and upgrade, supports multi-threading concurrency and automatic memory management. Built on C++ dynamic library wrapper of Baidu's PaddleOCR, supports the latest paddle_inference 3.3.0 inference engine.

**If you like this project, please give us a free Star ⭐**

Supports the latest PP-OCRv6 models, backward compatible with V5/V4 models and custom trained models.

> 💡 **Note**: The open-source version is suitable for learning and research. For commercial projects, paid versions are recommended for better performance and technical support. For paid version details, contact developer QQ: **2380243976**

Core C++ Dynamic Link Library PaddleOCR.dll Interface Documentation： [PaddleOCR.dll接口清单.md](docs/PaddleOCR.dll接口清单.md)

## ✨ Features

- ✅ **Multi-Language Support**: C#, C++, Java, Python, Go
- ✅ **High Performance**: CPU/GPU inference support
- ✅ **Easy Integration**: WebAPI service for online calling
- ✅ **Multi-Threading**: Concurrent processing with automatic memory management
- ✅ **Offline Operation**: No internet required, secure data processing
- ✅ **Rich Models**: Support for PP-OCRv5/v4 series models
- ✅ **Comprehensive Features**: Text detection, recognition, orientation classification, table recognition
- ✅ **Image Correction**: Document image geometric transformation, correcting distortion, tilt, and perspective deformation to improve recognition accuracy
- ✅ **Vision-Language Model**: Integrated PaddleOCR-VL vision-language model supporting general OCR with prompts and document layout analysis

## 📁 Project Structure

```
PaddleOCRWebApi/
├── PaddleOCRSDK/                  # Core OCR SDK project
│   ├── PaddleOCR/                  # OCR service implementation
│   │   ├── IOCRService.cs         # Interface definitions
│   │   ├── OCRService.cs         # OCR recognition service
│   │   └── OCRSDK.cs             # SDK core wrapper
│   ├── UVDoc/                    # Document image correction module
│   │   └── ...                   # Geometric transformation, perspective correction
│   ├── PaddleOCRVL/              # Vision-Language model module
│   │   ├── IOCRVLService.cs      # VL service interface
│   │   ├── OCRVLService.cs       # VL recognition service
│   │   └── OCRVLSDK.cs          # VL SDK wrapper
│   ├── Models/                   # Data models
│   └── PaddleOCRSDK.csproj      # SDK project file
│
├── OCRCoreService/               # WebAPI service project
│   ├── Controllers/              # API controllers
│   │   ├── OCRServiceController.cs      # OCR endpoints
│   │   ├── UVDocServiceController.cs    # Document correction endpoints
│   │   ├── OCRVLServiceController.cs    # Vision-Language model endpoints
│   │   └── HomeController.cs            # Home page
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
│   └── WinFormsApp/            # OCR recognition WinForms demo
│       ├── MainForm.cs         # Main form
│       └── Services/           # Service layer
│
├── docs/                        # Documentation
└── README.md                   # Project documentation
```

## 🚀 Quick Start

### 1. NuGet Package Installation (Recommended)

For paddle_inference 3.3+ version:

```xml
<PackageReference Include="PaddleOCRSDK" Version="4.5.1" />
<PackageReference Include="PaddleOCRRuntime_x64" Version="4.5.1" />
```

### 2. WebAPI Service Startup

```bash
# Run WebAPI service
cd OCRCoreService
dotnet run --urls http://*:5000

# Access scalar documentation
http://localhost:5000/scalar
```

For detailed WebAPI documentation, please refer to: [WebApi Documentation](./OCRCoreService/README.md)

## 🔧 Runtime Environment

### Basic Requirements

OCRCoreService (WebAPI) and WinForms project require VS2026 + .NET10.0

### Inference Library Version

1. **Default paddle_inference 3.3.0 CPU version**, other versions can be downloaded manually or compiled

2. **Core file PaddleOCR.dll** is a C++ dynamic library, supports CPU/GPU mode (GPU requires environment setup)

### .NET Platform Support

Supported frameworks: netstandard2.0; net45; net461; net47; net48; net6.0; net7.0; net8.0; net9.0; net10.0

## 📋 Parameter Description

| Parameter Name               | Default | Description                                                                                   |
| ---------------------------- | ------- | --------------------------------------------------------------------------------------------- |
| det_model_dir                | -       | Detection model inference model path                                                          |
| cls_model_dir                | -       | Direction classifier inference model path                                                     |
| rec_infer                    | -       | Text recognition model inference model path                                                   |
| table_model_dir              | -       | Table recognition model inference model path                                                  |
| **General Parameters**       | --      | --                                                                                            |
| det                          | true    | Whether to execute text detection                                                             |
| rec                          | true    | Whether to execute text recognition                                                           |
| cls                          | false   | Whether to execute text direction classification                                              |
| use_gpu                      | false   | Whether to use GPU                                                                            |
| gpu_id                       | 0       | GPU id, effective when using GPU                                                              |
| gpu_mem                      | 4000    | GPU memory usage                                                                              |
| use_tensorrt                 | false   | Whether to enable TensorRT when using GPU prediction                                          |
| cpu_mem                      | 4000    | CPU memory usage limit in MB. -1 means no limit                                               |
| cpu_threads                  | 30      | Number of threads for CPU prediction, larger value means faster prediction with sufficient cores |
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
| return_word_box              | false   | Whether to return per-character coordinates                                                   |
| ocr_instance_count           | false   | Number of OCR engine instances: default is 1, maximum is 10, suitable for high-concurrency scenarios.  |
| **Layout Structure Recognition Parameters (LayoutParameter)** | -- | **For PP-StructureV3** |
| use_gpu                      | false   | Whether to use GPU                                                                            |
| gpu_id                       | 0       | GPU id, effective when using GPU                                                              |
| gpu_mem                      | 4000     | GPU memory usage                                                                              |
| use_tensorrt                 | false   | Whether to enable TensorRT when using GPU prediction                                          |
| cpu_mem                      | 0       | CPU memory usage limit in MB. 0 means no limit                                                |
| cpu_threads                  | 30       | Number of threads for CPU prediction                                                          |
| enable_mkldnn                | true    | Whether to use mkldnn library                                                                 |
| visualize                    | false   | Whether to visualize results                                                                  |
| **Document Preprocessing**   | --      | --                                                                                            |
| use_doc_preprocessor         | false   | Whether to use document preprocessing                                                         |
| use_doc_orientation_classify | false   | Whether to use document orientation classification                                            |
| use_doc_unwarping            | false   | Whether to use document unwarping                                                             |
| **Layout Detection**         | --      | --                                                                                            |
| use_layout_detection         | true    | Whether to use layout detection                                                               |
| use_region_detection         | false   | Whether to use region detection                                                               |
| layout_threshold             | 0.5     | Layout detection threshold                                                                    |
| layout_nms                   | true    | Whether to use layout non-maximum suppression                                                 |
| layout_unclip_ratio_w        | 1.0     | Layout box horizontal expansion ratio                                                         |
| layout_unclip_ratio_h        | 1.0     | Layout box vertical expansion ratio                                                           |
| **OCR Parameters**           | --      | --                                                                                            |
| run_ocr_after_layout         | true    | Whether to execute OCR after layout detection                                                 |
| text_det_thresh              | 0.3     | Text detection threshold                                                                      |
| text_rec_score_thresh        | 0.5     | Text recognition score threshold                                                              |
| use_textline_orientation     | true    | Whether to use text line orientation                                                          |
| max_side_len                 | 960     | Input image longest side limit                                                                |
| **Conditional Recognition**  | --      | --                                                                                            |
| use_table_recognition        | true    | Whether to use table recognition                                                              |
| use_seal_recognition         | false   | Whether to use seal recognition                                                               |
| use_formula_recognition      | true    | Whether to use formula recognition                                                            |
| use_chart_recognition        | false   | Whether to use chart-to-table recognition                                                     |
| seal_det_limit_side_len      | 736     | Seal detection image long side limit                                                          |
| seal_det_limit_type          | 0       | Seal detection limit type                                                                     |
| seal_det_thresh              | 0.2     | Seal detection threshold                                                                      |
| seal_det_box_thresh          | 0.6     | Seal detection box threshold                                                                  |
| seal_det_unclip_ratio        | 0.5     | Seal detection box expansion ratio                                                            |
| seal_rec_score_thresh        | 0.0     | Seal recognition score threshold                                                              |
| **Output Parameters**        | --      | --                                                                                            |
| format_block_content         | false   | Whether to format block content                                                               |
| output_markdown              | true    | Whether to output Markdown format                                                             |

For more complete examples, please check the `Demo/` directory for each language example code.

## 🤖 OCR-VL Vision-Language Model

PaddleOCR-VL is an OCR extension module based on a vision-language model (VLM), providing inference capabilities via `llamaocr-vl.dll`.

### API Overview

| Endpoint | Description |
| -------- | ----------- |
| `GetOCRVL` | General OCR recognition with prompt + image Base64, returns recognized text (use when layout analysis is not enabled) |
| `GetOCRVLFile` | General OCR recognition with prompt + image file upload, returns recognized text (use when layout analysis is not enabled) |
| `GetDOCVL` | Layout analysis recognition with image Base64, returns Markdown and JSON structured results (use when layout analysis is enabled) |
| `GetDOCVLFile` | Layout analysis recognition with image file upload, returns Markdown and JSON structured results (use when layout analysis is enabled) |

### Configuration

Configure `OCRVLConfig` in `appsettings.json`:

```json
"OCRVLConfig": {
  "enabled": true,
  "yaml_path": "configs/PaddleOCR-VL-1.5.yaml"
}
```

| Parameter | Default | Description |
| --------- | ------- | ----------- |
| `enabled` | `false` | Whether to enable the OCR-VL service |
| `yaml_path` | `configs/PaddleOCR-VL-1.5.yaml` | Path to the yaml model configuration file |

## 🖥️ GPU Configuration

### paddle_inference 3.x GPU Version

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
| CUDNN | [https://developer.nvidia.com/cudnn-archive](https://developer.nvidia.com/cudnn-archive) |
| TensorRT | [https://developer.nvidia.com/nvidia-tensorrt-download](https://developer.nvidia.com/nvidia-tensorrt-download) |
| PP-OCRv4/v5 Models | [https://paddlepaddle.github.io/PaddleX/latest/pipeline_usage/tutorials/ocr_pipelines/OCR.html#11](https://paddlepaddle.github.io/PaddleX/latest/pipeline_usage/tutorials/ocr_pipelines/OCR.html#11) |
| Other Models | [https://gitee.com/paddlepaddle/PaddleOCR/blob/main/docs/version3.x/model_list.md) |

## 🔗 WebAPI Interface

For detailed WebAPI documentation, please refer to: [WebApi Documentation](./OCRCoreService/README.md)

**Scalar Documentation**: `http://localhost:5000/scalar`

## 💬 Community

Welcome to join QQ group **475159576** for discussion, or add QQ for custom projects: **2380243976**

If you like this project, please give us a free **Star ⭐**

<img src="./PaddleOCRSDK/Images/qq.png" width="150px;" />

## ☕ Donation

If this project helps you, please scan the QR code below to buy us a coffee.

<img src="./PaddleOCRSDK/Images/donate.jpg" width="150px;" />

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
│    paddle_inference 3.3.0 / 2.6.2       │
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

For detailed update history, please see: [Changelog](./docs/CHANGELOG_EN.md)

## 🔍 FAQ

For frequently asked questions, please see: [FAQ](./docs/FAQ_EN.md)

## 🙏 Acknowledgments

This project is based on the following open source projects:
- [PaddleOCR](https://github.com/PaddlePaddle/PaddleOCR) - Baidu PaddleOCR toolkit
- [Paddle](https://github.com/PaddlePaddle/Paddle) - PaddlePaddle inference engine

## ⭐️ Star

[![Star History Chart](https://api.star-history.com/svg?repos=PaddleOCRCore/PaddleOCRApi&type=Date)](https://star-history.com/#PaddleOCRCore/PaddleOCRApi&Date)

## 📄 License

This project is released under [Apache License Version 2.0](./LICENSE). Welcome to use and contribute.
