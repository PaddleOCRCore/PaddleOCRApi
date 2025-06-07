[<img src="https://img.shields.io/badge/Language-简体中文-red.svg">](README.md)
# PaddleOCRSDK离线OCR组件 支持C#/C++/java/Python/Go语言开发
<p align="center">
    <a href="https://discord.gg/z9xaRVjdbD"><img src="https://img.shields.io/badge/Chat-on%20discord-7289da.svg?sanitize=true" alt="Chat"></a>
    <a href="./LICENSE"><img src="https://img.shields.io/badge/license-Apache%202-dfd.svg"></a>
    <a href="https://github.com/PaddleOCRCore/PaddleOCRApi/releases"><img src="https://img.shields.io/github/v/release/PaddleOCRCore/PaddleOCRApi?color=ffa"></a>
    <a href=""><img src="https://img.shields.io/badge/os-linux%2C%20win%2C%20mac-pink.svg"></a>
    <a href="https://github.com/PaddleOCRCore/PaddleOCRApi/stargazers"><img src="https://img.shields.io/github/stars/PaddleOCRCore/PaddleOCRApi?color=ccf"></a>
</p>

## 一、简介
免费离线OCR组件,支持CPU/GPU，免费使用CPU普通版、CPU加速版，免费升级，支持C#/C++/java/Python/Go语言开发，支持多线程并发，支持内存自动回收， 基于百度飞桨PaddleOCR封装的C++动态链接库，支持paddle_inference2.6.2及3.0推理库。
喜欢的请给本项目点一个免费的Star

支持最新PP-OCRv5_mobile/PP-OCRv5_server模型，向下兼容V4/V3模型

## 二、运行环境
项目运行环境为VS2022+.net8.0：

1、默认[paddle_inference3.0](https://www.paddlepaddle.org.cn/inference/v3.0/guides/install/download_lib.html#windows)版本CPU版本推理库，其它推理库请手动下载

- paddle_inference2.6.2版本推理库下载

- CPU版本(PaddleOCRRuntime_x64已包含)：
https://paddle-inference-lib.bj.bcebos.com/2.6.2/cxx_c/Windows/CPU/x86-64_avx-mkl-vs2019/paddle_inference.zip

2、核心文件PaddleOCR.dll为C++动态链接库，支持CPU/GPU模式(GPU需接说明安装对应环境)

3、.net引用(支持netstandard2.0;net45;net461;net47;net48;net6.0;net7.0;net8.0;net9.0)

`<PackageReference Include="PaddleOCRSDK" Version="2.0.2" />`PaddleOCRSDK2.0.2兼容PaddleOCRRuntime_x64 v1.0.5版本

`<PackageReference Include="PaddleOCRRuntime_x64" Version="2.0.0" />`

若使用paddle_inference2.6.2版本推理库引用PaddleOCRRuntime_x64 v1.0.5版本

PaddleOCRRuntime_x64支持Python、Go、C++等环境

### [WebApi接口文档](./OCRCoreService/README.md)
WebApi部署后可供前端调用。

### WinFormDemo预览：

<img src="./Demo/PaddleOCRRuntime/ocrDemo.jpg" width="800px;" />

依赖库列表参考：


## 三、调用参数说明
| 参数名称                     | 默认值 | 值说明                                                                                   |
| ---------------------------- | ------ | ---------------------------------------------------------------------------------------- |
| det_model_dir                | -      | 检测模型inference model地址                                                              |
| cls_model_dir                | -      | 方向分类器inference model地址                                                            |
| rec_infer                    | -      | 文字识别模型inference model地址                                                          |
| keys                         | -      | 文字识别字典文件                                                                         |
| table_model_dir              | -      | 表格识别模型inference model地址                                                          |
| table_char_dict_path         | -      | 表格识别字典文件                                                                         |
| 通用参数                 | --     | -- |
| det                          | true   | 是否执行文字检测                                                                         |
| rec                          | true   | 是否执行文字识别                                                                         |
| cls                          | false  | 是否执行文字方向分类                                                                     |
| use_gpu                      | false  | 是否使用GPU                                                                              |
| gpu_id                       | 0      | GPU id，使用GPU时有效                                                                    |
| gpu_mem                      | 4000   | 使用GPU时内存                                                                            |
| use_tensorrt                 | false  | 使用GPU预测时，是否启动tensorrt                                                          |
| cpu_mem                      | 4000   | CPU内存占用上限，单位MB。-1表示不限制                                                    |
| cpu_math_library_num_threads | 10     | CPU预测时的线程数，在机器核数充足的情况下，该值越大，预测速度越快                        |
| enable_mkldnn                | true   | 是否使用mkldnn库，关掉可以减少内存占用，但会降低速度                                     |
| 检测模型相关                 | --     | -- |
| max_side_len                 | 960    | 输入图像长宽大于960时，等比例缩放图像，使得图像最长边为960                               |
| det_db_thresh                | 0.3    | 用于过滤DB预测的二值化图像，设置为0.-0.3对结果影响不明显                                 |
| det_db_box_thresh            | 0.5    | DB后处理过滤box的阈值，如果检测存在漏框情况，可酌情减小                                  |
| det_db_unclip_ratio          | 1.6    | 表示文本框的紧致程度，越小则文本框更靠近文本                                             |
| use_dilation                 | false  | 是否在输出映射上使用膨胀                                                                 |
| det_db_score_mode            | true   | true:使用多边形框计算bbox score，false:使用矩形框计算。矩形框计算速度更快，多边形框对弯曲文本区域计算更准确。                                                        |
| visualize                    | false  | 是否对结果进行可视化，为false时，预测结果会保存在output文件夹下和输入图像同名的图像上。  |
|方向分类器相关                | --     | -- |
| use_angle_cls                | false  | 是否使用方向分类器                                                                       |
| cls_thresh                   | 0.9    | 方向分类器的得分阈值                                                                     |
| cls_batch_num                | 1      | 方向分类器批量识别数量                                                                   |
| 识别模型相关                 | --     | -- |
| rec_batch_num                | 6      | 文字识别模型批量识别数量                                                                 |
| rec_img_h                    | 48     | 文字识别模型输入图像高度                                                                 |
| rec_img_w                    | 320    | 文字识别模型输入图像宽度                                                                 |
| 表格识别模型相关             | --     | -- |
| table_max_len                | 488    | 表格识别模型输入图像长边大小，最终网络输入图像大小为（table_max_len，table_max_len）     |
| merge_empty_cell             | true   | 是否合并空单元格                                                                         |
| table_batch_num              | 1      | table_batch_num                                                                          |

## 四、GPU环境配置说明
### paddle_inference2.6.2版本GPU推理库下载及配置

下载地址：[paddle_inference2.6.2](https://www.paddlepaddle.org.cn/inference/v2.6/guides/install/download_lib.html#windows)版本推理库，
- https://paddle-inference-lib.bj.bcebos.com/2.6.2/cxx_c/Windows/GPU/x86-64_cuda12.0_cudnn8.9.1_trt8.6.1.6_mkl_avx_vs2019/paddle_inference.zip

解压后将以下dll文件复制到程序运行文件夹中：
-  paddle\lib目录下的common.dll和paddle_inference.dll
- third_party\install\mkldnn\lib目录下的mkldnn.dll
- third_party\install\mklml\lib目录下的libiomp5md.dll和mklml.dll
#### 安装指定版本的CUDA以及CUDNN
复制对应的CUDNN中的cudnn64_8.dll到程序运行文件夹中
- 位于：C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v12.4\\bin\\cudnn64_8.dll

### paddle_inference3.0版本GPU推理库下载及配置
首先将[paddle_inference3.0](https://www.paddlepaddle.org.cn/inference/master/guides/install/download_lib.html#windows)版本，请将paddleinference3目录中的PaddleOCR.dll替换当前目录文件

- paddle_inference3.0版本推理库下载

- CPU版本(WebApi模式CPU加速不可用，请关闭enable_mkldnn)
3.0.0正式版本：https://paddle-inference-lib.bj.bcebos.com/3.0.0/cxx_c/Windows/CPU/x86-64_avx-mkl-vs2019/paddle_inference.zip

- GPU版本--官方推理库暂时不可用
3.0.0正式版本：https://paddle-inference-lib.bj.bcebos.com/3.0.0/cxx_c/Windows/GPU/x86-64_cuda12.6_cudnn9.5.1_trt10.5.0.18_mkl_avx_vs2019/paddle_inference.zip

paddle_inference解压后将以下dll文件复制到程序运行文件夹中：
-  paddle\lib目录下的common.dll和paddle_inference.dll
- third_party\install\mkldnn\lib目录下的mkldnn.dll
- third_party\install\mklml\lib目录下的libiomp5md.dll和mklml.dll

#### 安装指定版本的CUDA以及CUDNN
复制对应的CUDNN中的cudnn64_9.dll到程序运行文件夹中
- 位于：C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v12.3\\bin\\cudnn64_9.dll

- [cuda下载](https://developer.nvidia.com/cuda-12-0-0-download-archive)
- [cudnn下载](https://developer.nvidia.cn/rdp/cudnn-archive)
- [TensorRT下载](https://developer.nvidia.com/nvidia-tensorrt-download)


- [PP-OCRv4模型下载地址](https://gitee.com/paddlepaddle/PaddleOCR/blob/release/2.7/doc/doc_ch/models_list.md)
- [PP-OCRv5模型下载地址](https://gitee.com/paddlepaddle/PaddleOCR/blob/release/3.0/docs/version3.x/model_list.md)

## 开发交流群

欢迎加入QQ群475159576交流,或者添加QQ：2380243976,若您喜欢本项目，请点击免费的Star

<img src="./Demo/PaddleOCRRuntime/qq.png" width="382px;" />

## 捐助

如果这个项目对您有所帮助，请扫下方二维码打赏一杯咖啡。

<img src="./Demo/PaddleOCRRuntime/donate.jpg" width="382px;" />

## 更新日志
### v2.0.0 `2025.6.4`
- 修改PaddleOCR.dll接口，增加支持PP-OCRv5模型
- WinForm Demo增加V5/V4模型选择下拉选项
### v1.0.5 `2025.4.1`
- 优化PaddleOCR.dll接口，Demo增加表格识别功能
### v1.0.4 `2025.3.29`
- 优化PaddleOCR.dll，增加日志输出开关，OCR识别提速
- WebApi接口优化，增加OCR初始化及参数设置
### v1.0.2 `2025.3.23`
- 优化PaddleOCR.dll，增加多线程队列支持，增加内存达到上限自动回收
- WinFormDemo功能强化，增加初始化选项，增加多图选择及模拟并发测试
### v1.0.1 `2025.3.5`
- 优化PaddleOCR.dll，提高识别速度，增加智能指针
### v1.0 `2025.1.22`
- 初版发行: PaddleOCRApi

## ⭐️ Star

[![Star History Chart](https://api.star-history.com/svg?repos=PaddleOCRCore/PaddleOCRApi&type=Date)](https://star-history.com/#PaddleOCRCore/PaddleOCRApi&Date)

## 📄 许可证书

本项目的发布受 [Apache License Version 2.0](./LICENSE) 许可认证, 欢迎大家使用和贡献。