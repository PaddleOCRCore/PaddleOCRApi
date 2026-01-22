# 🔍 常见问题 (FAQ)

<details>
<summary><b>Q: 如何选择CPU版本还是GPU版本？</b></summary>

**A:** 
- CPU版本：适合小批量识别，部署简单，无需GPU环境
- GPU版本：适合大批量识别，速度快，需要CUDA12.9环境支持
</details>

<details>
<summary><b>Q: 如何提高识别准确率？</b></summary>

**A:** 
1. 选择合适的模型（mobile/server）
2. 调整`det_db_thresh`、`det_db_box_thresh`参数
3. 启用方向分类器`use_angle_cls=true`
4. 对图片进行预处理（去噪、二值化等）
</details>

<details>
<summary><b>Q: 支持哪些图片格式？</b></summary>

**A:** 支持常见的图片格式：jpg、jpeg、png、bmp、tiff等
</details>

<details>
<summary><b>Q: 如何在Linux上使用？</b></summary>

**A:** 
- 需要针对对应平台编译PaddleOCR.so动态库
- 或部署WebAPI服务
- 联系开发者定制
</details>

<details>
<summary><b>Q: 项目开源吗？商业使用是否免费？</b></summary>

**A:** 
- Win CPU-飞浆推理库版本完全开源免费，不限使用期限，可商业使用
- 其他版本（Onnx版、GPU版、Linux版）为付费版本
- 商业项目建议使用付费版本以获得更好的性能和技术支持
</details>

<details>
<summary><b>Q: 如何获取付费版本？</b></summary>

**A:** 
请联系开发者QQ：**2380243976** 或加入QQ群：**475159576** 咨询
</details>

<details>
<summary><b>Q: WebAPI服务如何部署？</b></summary>

**A:** 
1. 克隆项目到本地
2. 在OCRCoreService目录下执行 `dotnet run --urls http://*:5000`
3. 访问 `http://localhost:5000/swagger/index.html` 查看接口文档
4. 详细配置请参考：[WebApi接口文档](../OCRCoreService/README.md)
</details>

<details>
<summary><b>Q: 支持哪些OCR模型？</b></summary>

**A:** 
- 支持PP-OCRv5_mobile/PP-OCRv5_server最新模型
- 向下兼容PP-OCRv4/v3模型
- 支持自训练模型
- 模型下载地址：[PaddleOCR官网](https://www.paddleocr.ai/latest/version3.x/pipeline_usage/OCR.html)
</details>

<details>
<summary><b>Q: 如何进行文本图像矫正？</b></summary>

**A:** 
- 可通过WebAPI的UVDocServiceController接口调用
- 参考Demo/PaddleVisionWinForm示例进行集成
- 文本图像矫正可纠正文档扭曲、倾斜、透视变形等问题
</details>

<details>
<summary><b>Q: 遇到问题如何获取帮助？</b></summary>

**A:** 
1. 查看项目README和相关文档
2. 加入QQ群 **475159576** 交流讨论
3. 提交GitHub Issue
4. 联系开发者QQ **2380243976**
</details>
