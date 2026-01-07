# PaddleDocVision WinForms 演示程序

这是一个使用 .NET 8 WinForms 开发的文本图像矫正演示程序。

## 功能特性

- 上传图像文件（支持 JPG、PNG、BMP、TIFF 格式）
- 实时显示原始图像和矫正后的图像
- 保存矫正结果
- 友好的用户界面

## 使用方法

### 1. 编译项目

在 WinFormDemo 目录中运行：

```powershell
dotnet build -c Release
```

### 2. 部署 DLL 和模型

将以下文件复制到输出目录 `bin\Release\net8.0-windows\`:

- `PaddleDocVision.dll`
- `models\UVDoc_infer\` 目录（包含所有模型文件）

### 3. 运行程序

```powershell
dotnet run
```

或者直接运行编译后的可执行文件：

```powershell
.\bin\Release\net8.0-windows\PaddleVisionWinForm.exe
```

## 目录结构

```
WinFormDemo/
├── PaddleVisionWinForm.csproj   # 项目文件
├── Program.cs                       # 程序入口
├── MainForm.cs                      # 主窗体逻辑
├── MainForm.Designer.cs             # 主窗体设计器代码
├── UVDocWrapper.cs                  # DLL 包装类
└── README.md                        # 说明文档
```

## 系统要求

- Windows 10/11 或 Windows Server 2019+
- .NET 8.0 Runtime
- Visual C++ 2015-2022 Redistributable（用于 vcomp140.dll）

## 注意事项

1. 确保所有 DLL 文件都在程序目录中
2. 确保 models 目录包含完整的模型文件
3. 首次运行可能需要较长时间初始化模型
4. GPU 模式需要 CUDA 支持

## 故障排除

### 提示 "PaddleDocVision.dll 未找到"

将 DLL 文件复制到程序所在目录。

### 提示 "模型初始化失败"

检查 models/UVDoc_infer 目录是否存在且包含以下文件：
- inference.json
- inference.pdiparams
- inference.yml

### 提示缺少 vcomp140.dll

安装 Visual C++ 2015-2022 Redistributable。
