# 🔍 FAQ (Frequently Asked Questions)

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
<summary><b>Q: How to use on Linux?</b></summary>

**A:** 
- Need to compile PaddleOCR.so dynamic library for the corresponding platform
- Or deploy WebAPI service
- Contact developer for customization
</details>

<details>
<summary><b>Q: Is the project open source? Is commercial use free?</b></summary>

**A:** 
- Win CPU-PaddleInference version is completely open source and free with unlimited usage
- Other versions (Onnx, GPU, Linux) are paid versions
- Paid versions are recommended for commercial projects to get better performance and technical support
</details>

<details>
<summary><b>Q: How to get the paid version?</b></summary>

**A:** 
Please contact developer QQ: **2380243976** or join QQ group: **475159576** for consultation
</details>

<details>
<summary><b>Q: How to deploy WebAPI service?</b></summary>

**A:** 
1. Clone the project locally
2. Execute `dotnet run --urls http://*:5000` in OCRCoreService directory
3. Visit `http://localhost:5000/swagger/index.html` to view API documentation
4. For detailed configuration, refer to: [WebAPI Documentation](../OCRCoreService/README.md)
</details>

<details>
<summary><b>Q: Which OCR models are supported?</b></summary>

**A:** 
- Supports the latest PP-OCRv5_mobile/PP-OCRv5_server models
- Backward compatible with PP-OCRv4/v3 models
- Supports custom trained models
- Model download: [PaddleOCR Official Website](https://www.paddleocr.ai/latest/version3.x/pipeline_usage/OCR.html)
</details>

<details>
<summary><b>Q: How to perform document image correction?</b></summary>

**A:** 
- Use the PaddleDocVision.dll in PaddleOCRSDK
- Can be called via WebAPI's UVDocServiceController interface
- Refer to Demo/PaddleVisionWinForm example for integration
- Document image correction can fix distortion, tilt, and perspective deformation
</details>

<details>
<summary><b>Q: How to get help when encountering problems?</b></summary>

**A:** 
1. Check project README and related documentation
2. Join QQ group **475159576** for discussion
3. Submit GitHub Issue
4. Contact developer QQ **2380243976**
</details>
