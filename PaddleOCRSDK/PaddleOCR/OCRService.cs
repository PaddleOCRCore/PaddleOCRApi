// Copyright (c) 2025 PaddleOCRCore All Rights Reserved.
// https://github.com/PaddleOCRCore/PaddleOCRApi.git
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaddleOCRSDK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRSDK
{

    /// <summary>
    /// 初始化模型分类
    /// </summary>
    public enum EnumModelType
    {
        /// <summary>
        /// PP-OCRv6_tiny
        /// </summary>
        [Description("PP-OCRv6_tiny")]
        PPOCRv6_tiny,

        /// <summary>
        /// PP-OCRv6_small
        /// </summary>
        [Description("PP-OCRv6_small")]
        PPOCRv6_small,

        /// <summary>
        /// PP-OCRv5_mobile
        /// </summary>
        [Description("PP-OCRv5_mobile")]
        PPOCRv5_mobile,

        /// <summary>
        /// PP-OCRv4_mobile
        /// </summary>
        [Description("PP-OCRv4_mobile")]
        PPOCRv4_mobile
    }
    public class OCRException : Exception
    {
        public OCRException(string message) : base(message)
        {
        }
    }
    public class OCRService : IOCRService
    {
        /// <summary>
        /// 初始化OCR引擎默认V4模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath">模型所在目录，如models</param>
        /// <param name="modelType">模型枚举EnumModelType</param>
        /// <returns></returns>
        public string InitDefaultOCREngine(string modelsPath="models", EnumModelType modelType = EnumModelType.PPOCRv6_tiny)
        {
            string det_infer = "PP-OCRv6_tiny_det_infer";//OCR检测模型
            string rec_infer = "PP-OCRv6_tiny_rec_infer";//OCR识别模型
            string cls_infer = "PP-LCNet_x1_0_textline_ori";

            switch (modelType)
            {
                case EnumModelType.PPOCRv6_tiny:
                    det_infer = "PP-OCRv6_tiny_det_infer";
                    rec_infer = "PP-OCRv6_tiny_rec_infer";
                    break;
                case EnumModelType.PPOCRv6_small:
                    det_infer = "PP-OCRv6_small_det_infer";
                    rec_infer = "PP-OCRv6_small_rec_infer";
                    break;
                case EnumModelType.PPOCRv5_mobile:
                    det_infer = "PP-OCRv5_mobile_det_infer";
                    rec_infer = "PP-OCRv5_mobile_rec_infer";
                    break;
                case EnumModelType.PPOCRv4_mobile:
                    det_infer = "PP-OCRv4_mobile_det_infer";
                    rec_infer = "PP-OCRv4_mobile_rec_infer";
                    break;
                default:
                    det_infer = "PP-OCRv6_tiny_det_infer";
                    rec_infer = "PP-OCRv6_tiny_rec_infer";
                    break;
            }
            bool use_gpu = false;//是否使用GPU
            int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
            int gpu_id = 0;//GPUId
            bool enable_mkldnn = true;
            int cpu_threads = Environment.ProcessorCount; //CPU预测时的线程数
            InitParamater para = new InitParamater();
            para.det_infer = $"{modelsPath}/{det_infer}";
            para.cls_infer = $"{modelsPath}/{cls_infer}";
            para.rec_infer = $"{modelsPath}/{rec_infer}";

            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.use_gpu = use_gpu;
            oCRParameter.use_tensorrt = true;
            oCRParameter.gpu_id = gpu_id;
            oCRParameter.gpu_mem = 4000;
            oCRParameter.cpu_mem = cpu_mem;
            oCRParameter.cpu_threads = cpu_threads;//提升CPU速度，优化此参数
            oCRParameter.enable_mkldnn = enable_mkldnn;
            oCRParameter.cls = false;
            oCRParameter.det = true;
            oCRParameter.use_angle_cls = false;
            oCRParameter.det_db_score_mode = false;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;
            oCRParameter.det_db_box_thresh = 0.618f;
            oCRParameter.visualize = true;
            para.ocrpara = oCRParameter;
            para.paraType = EnumParaType.Class;
            string msg = "文本识别初始化成功";
            try
            {
                Init(para);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 初始化表格识别引擎默认V5模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath">模型所在目录，如models</param>
        /// <param name="modelType">模型枚举EnumModelType</param>
        /// <returns></returns>
        public string InitDefaultStructureEngine(string modelsPath = "models", EnumModelType modelType = EnumModelType.PPOCRv6_tiny)
        {
            string det_infer = "PP-OCRv5_mobile_det_infer";//OCR检测模型
            string rec_infer = "PP-OCRv5_mobile_rec_infer";//OCR识别模型

            switch (modelType)
            {
                case EnumModelType.PPOCRv6_tiny:
                    det_infer = "PP-OCRv6_tiny_det_infer";
                    rec_infer = "PP-OCRv6_tiny_rec_infer";
                    break;
                case EnumModelType.PPOCRv6_small:
                    det_infer = "PP-OCRv6_small_det_infer";
                    rec_infer = "PP-OCRv6_small_rec_infer";
                    break;
                case EnumModelType.PPOCRv5_mobile:
                    det_infer = "PP-OCRv5_mobile_det_infer";
                    rec_infer = "PP-OCRv5_mobile_rec_infer";
                    break;
                case EnumModelType.PPOCRv4_mobile:
                    det_infer = "PP-OCRv4_mobile_det_infer";
                    rec_infer = "PP-OCRv4_mobile_rec_infer";
                    break;
                default:
                    det_infer = "PP-OCRv6_tiny_det_infer";
                    rec_infer = "PP-OCRv6_tiny_rec_infer";
                    break;
            }

            string cls_infer = "PP-LCNet_x1_0_textline_ori";
            string layout_model_dir = "PP-DocLayoutV3_infer";
            string table_cls_model_dir = "PP-LCNet_x1_0_table_cls_infer";
            string wired_table_model_dir = "SLANeXt_wired_infer";
            string wireless_table_model_dir = "SLANeXt_wireless_infer";
            string wired_table_cell_det_model_dir = "RT-DETR-L_wired_table_cell_det_infer";
            string wireless_table_cell_det_model_dir = "RT-DETR-L_wireless_table_cell_det_infer";
            string formula_model_dir = "LaTeX_OCR_rec_infer";
            string seal_model_dir = "PP-OCRv4_mobile_seal_det_infer";
            string doc_cls_infer = "PP-LCNet_x1_0_doc_ori_infer";
            string doc_unwarp_model = "UVDoc_infer";
            string region_model_dir = "PP-DocBlockLayout";

            bool use_gpu = false;//是否使用GPU
            int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
            int gpu_id = 0;//GPUId
            bool enable_mkldnn = true;
            int cpu_threads = Environment.ProcessorCount;  //CPU预测时的线程数
            InitParamater para = new InitParamater();
            para.det_infer = $"{modelsPath}/{det_infer}";
            para.cls_infer = $"{modelsPath}/{cls_infer}";
            para.rec_infer = $"{modelsPath}/{rec_infer}";

            para.layout_model_dir = $"{modelsPath}/{layout_model_dir}";
            para.table_cls_model_dir = $"{modelsPath}/{table_cls_model_dir}";
            para.wired_table_model_dir = $"{modelsPath}/{wired_table_model_dir}";
            para.wireless_table_model_dir = $"{modelsPath}/{wireless_table_model_dir}";
            para.wired_table_cell_det_model_dir = $"{modelsPath}/{wired_table_cell_det_model_dir}";
            para.wireless_table_cell_det_model_dir = $"{modelsPath}/{wireless_table_cell_det_model_dir}";
            para.formula_model_dir = $"{modelsPath}/{formula_model_dir}";
            para.seal_model_dir = $"{modelsPath}/{seal_model_dir}";
            para.doc_cls_infer = $"{modelsPath}/{doc_cls_infer}";
            para.doc_unwarp_model = $"{modelsPath}/{doc_unwarp_model}";
            para.region_model_dir = $"{modelsPath}/{region_model_dir}";

            LayoutParameter oCRParameter = new LayoutParameter();
            oCRParameter.use_gpu = use_gpu;
            oCRParameter.use_tensorrt = true;
            oCRParameter.gpu_id = gpu_id;
            oCRParameter.gpu_mem = 4000;
            oCRParameter.cpu_mem = cpu_mem;
            oCRParameter.cpu_threads = cpu_threads;
            oCRParameter.enable_mkldnn = enable_mkldnn;
            oCRParameter.visualize = true;

            oCRParameter.use_doc_preprocessor = false;
            oCRParameter.use_doc_orientation_classify = false;
            oCRParameter.use_doc_unwarping = false;

            oCRParameter.use_layout_detection = true;
            oCRParameter.use_region_detection = false;
            oCRParameter.layout_nms = true;
            oCRParameter.layout_unclip_ratio_w = 1.0f;
            oCRParameter.layout_unclip_ratio_h = 1.0f;

            oCRParameter.run_ocr_after_layout = true;
            oCRParameter.text_det_thresh = 0.3f;
            oCRParameter.text_rec_score_thresh = 0.5f;
            oCRParameter.use_textline_orientation = false;
            oCRParameter.max_side_len = 960;

            oCRParameter.use_table_recognition = true;
            oCRParameter.use_table_cells_detection = false;
            oCRParameter.use_seal_recognition = false;
            oCRParameter.use_formula_recognition = true;

            oCRParameter.format_block_content = false;
            oCRParameter.output_markdown = true;
            para.layoutpara = oCRParameter;
            para.paraType = EnumParaType.StructureClass;
            string msg = "版面识别初始化成功";
            try
            {
                Init(para);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 初始化OCR引擎
        /// </summary>
        /// <param name="para"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Init(InitParamater para)
        {
            try
            {
                bool ret;
                if (para.paraType == EnumParaType.Class)
                {
                    ret = OCRSDK.Init(para.det_infer, para.cls_infer, para.rec_infer, para.ocrpara);
                }
                else if (para.paraType == EnumParaType.Json)
                {
                    ret = OCRSDK.Initjson(para.det_infer, para.cls_infer, para.rec_infer, para.json);
                }
                else if (para.paraType == EnumParaType.StructureClass)
                {
                    string json = JsonConvert.SerializeObject(para.layoutpara);
                    ret = OCRSDK.InitStructurejson(
                        para.det_infer,
                        para.cls_infer,
                        para.rec_infer,
                        para.layout_model_dir,
                        para.table_cls_model_dir,
                        para.wired_table_model_dir,
                        para.wireless_table_model_dir,
                        para.wired_table_cell_det_model_dir,
                        para.wireless_table_cell_det_model_dir,
                        para.formula_model_dir,
                        para.seal_model_dir,
                        para.doc_cls_infer,
                        para.doc_unwarp_model,
                        para.region_model_dir,
                        json);
                }
                else if (para.paraType == EnumParaType.StructureJson)
                {
                    ret = OCRSDK.InitStructurejson(
                        para.det_infer,
                        para.cls_infer,
                        para.rec_infer,
                        para.layout_model_dir,
                        para.table_cls_model_dir,
                        para.wired_table_model_dir,
                        para.wireless_table_model_dir,
                        para.wired_table_cell_det_model_dir,
                        para.wireless_table_cell_det_model_dir,
                        para.formula_model_dir,
                        para.seal_model_dir,
                        para.doc_cls_infer,
                        para.doc_unwarp_model,
                        para.region_model_dir,
                        para.json);
                }
                else
                {
                    throw new OCRException("不支持的参数类型");
                }

                if (!ret)
                {
                    var error = GetError();
                    throw new OCRException($"{error}");
                }

                return ret;
            }
            catch (OCRException)
            {
                // OCRException 已经包含了"初始化失败:"前缀，直接重新抛出
                throw;
            }
            catch (Exception ex)
            {
                throw new OCRException($"{ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前机器的加密授权申请码
        /// </summary>
        /// <returns></returns>
        public string GetLicenseRequestCode()
        {
            return OcrServiceHelper.ReadNativeString(OCRSDK.GetLicenseRequestCode, OCRSDK.FreeResultBuffer);
        }

        /// <summary>
        /// 激活授权文件
        /// </summary>
        /// <param name="licenseFile">授权文件路径</param>
        /// <returns></returns>
        public bool ActivateLicense(string licenseFile)
        {
            if (string.IsNullOrWhiteSpace(licenseFile))
            {
                return false;
            }

            return OCRSDK.ActivateLicense(licenseFile);
        }

        /// <summary>
        /// 获取当前授权状态JSON
        /// </summary>
        /// <returns></returns>
        public string GetLicenseStatus()
        {
            return OcrServiceHelper.ReadNativeString(OCRSDK.GetLicenseStatus, OCRSDK.FreeResultBuffer);
        }

        /// <summary>
        /// 获取当前授权状态对象
        /// </summary>
        /// <returns></returns>
        public LicenseStatus GetLicenseStatusInfo()
        {
            return OcrServiceHelper.DeserializeLicenseStatus(GetLicenseStatus());
        }

        /// <summary>
        /// 对图像文件进行文本识别
        /// </summary>
        /// <param name="imagefile">图像文件</param>
        /// <returns>OCR识别结果</returns>
        public OCRResult Detect(string imagefile)
        {
            var ptrResult = OCRSDK.Detect(imagefile);
            return GetResult(ptrResult);
        }
        /// <summary>
        /// 对图像文件进行文本识别
        /// </summary>
        /// <param name="imagebyte">图像文件</param>
        /// <returns>OCR识别结果</returns>
        public OCRResult Detect(byte[] imagebyte)
        {
            var ptrResult = OCRSDK.DetectByte(imagebyte, new UIntPtr((ulong)imagebyte.LongLength));
            return GetResult(ptrResult);
        }
        /// <summary>
        /// 对Mat进行文本识别
        /// </summary>
        /// <param name="ptr_cvmat">Mat</param>
        /// <returns>OCR识别结果</returns>
        public OCRResult DetectMat(IntPtr ptr_cvmat)
        {
            var ptrResult = OCRSDK.DetectMat(ptr_cvmat);
            return GetResult(ptrResult);
        }
        public OCRResult DetectBase64(string base64)
        {
            var ptrResult = OCRSDK.DetectBase64(base64);
            return GetResult(ptrResult);
        }
        public OCRResult DetectScreenShot(IntPtr screenshotData, int size)
        {
            if (screenshotData == IntPtr.Zero || size <= 0)
            {
                return new OCRResult
                {
                    Code = 0,
                    ErrorMsg = "截图数据地址为空或长度无效"
                };
            }

            var ptrResult = OCRSDK.DetectScreenShot(screenshotData, size);
            return GetResult(ptrResult);
        }
        private OCRResult GetResult(IntPtr ptrResult)
        {
            OCRResult result = new OCRResult();
            if (ptrResult == IntPtr.Zero)
            {
                var lastErr = GetError();
                if (!string.IsNullOrEmpty(lastErr))
                {
                    result.Code = 0;
                    result.ErrorMsg = "OCR内部错误：" + lastErr;
                    //throw new OCRException("OCR内部错误：" + lastErr);
                }
                return result;
            }
            string json = string.Empty;
            try
            {
                json = MarshalUtf8.PtrToStringUTF8(ptrResult);
                if (string.IsNullOrEmpty(json))
                {
                    var lastErr = GetError();
                    result.Code = 0;
                    result.ErrorMsg = "识别结果为空:" + lastErr;
                }
                else
                {
                    try
                    {
                        result.JsonText = json;
                        List<JsonResult> jonResult = OcrServiceHelper.DeserializeObject<List<JsonResult>>(json);
                        result.WordsResult = jonResult ?? new List<JsonResult>();
                    }
                    catch (Exception e)
                    {
                        result.JsonText = json + e.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                var lastErr = GetError();
                result.Code = 0;
                result.ErrorMsg = "OCR结果Json反序列化失败:" + ex.Message;
                //throw new OCRException("OCR结果Json反序列化失败:" + ex.Message);
            }
            finally
            {
                if (ptrResult != IntPtr.Zero)
                {
                    //Marshal.FreeCoTaskMem(ptrResult);改为调用SDK的释放接口
                    OCRSDK.FreeResultBuffer(ptrResult);
                }
            }
            return result;
        }
        /// <summary>
        /// 执行文档版面分析（包含版面检测、表格识别、公式识别等）
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        public string DetectLayout(string imagefile)
        {
            var ptrResult = OCRSDK.DetectLayout(imagefile);
            return ReadStructureResult(ptrResult);
        }
        /// <summary>
        /// 执行文档版面分析（字节数组输入）
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        public string DetectLayoutByte(byte[] imagebyte)
        {
            var ptrResult = OCRSDK.DetectLayoutByte(imagebyte, new UIntPtr((ulong)imagebyte.LongLength));
            return ReadStructureResult(ptrResult);
        }
        /// <summary>
        /// 执行文档版面分析（OpenCV Mat 输入）
        /// </summary>
        /// <param name="ptr_cvmat">OpenCV Mat 指针</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        public string DetectLayoutMat(IntPtr ptr_cvmat)
        {
            var ptrResult = OCRSDK.DetectLayoutMat(ptr_cvmat);
            return ReadStructureResult(ptrResult);
        }
        /// <summary>
        /// 执行文档版面分析（Base64 编码输入）
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        public string DetectLayoutBase64(string base64)
        {
            var ptrResult = OCRSDK.DetectLayoutBase64(base64);
            return ReadStructureResult(ptrResult);
        }

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（文件路径输入）
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>结构化的版面识别结果</returns>
        public LayoutDetectResult DetectLayoutParsed(string imagefile)
        {
            var json = DetectLayout(imagefile);
            return ParseLayoutResult(json);
        }

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（字节数组输入）
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>结构化的版面识别结果</returns>
        public LayoutDetectResult DetectLayoutByteParsed(byte[] imagebyte)
        {
            var json = DetectLayoutByte(imagebyte);
            return ParseLayoutResult(json);
        }

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（Base64 输入）
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>结构化的版面识别结果</returns>
        public LayoutDetectResult DetectLayoutBase64Parsed(string base64)
        {
            var json = DetectLayoutBase64(base64);
            return ParseLayoutResult(json);
        }

        /// <summary>
        /// 解析文档版面识别 JSON 结果为结构化对象
        /// 仅支持新版 DLL 输出结构
        /// </summary>
        /// <param name="json">DetectLayout 返回的 JSON 字符串</param>
        /// <returns>结构化的版面识别结果</returns>
        public LayoutDetectResult ParseLayoutResult(string json)
        {
            return OcrServiceHelper.ParseLayoutResult(json, message => new OCRException(message));
        }

        private string ReadStructureResult(IntPtr ptrResult)
        {
            return OcrServiceHelper.GetStructureResult(
                ptrResult,
                GetError,
                OCRSDK.FreeResultBuffer,
                ParseLayoutResult,
                message => new OCRException(message));
        }

        /// <summary>
        /// 获取错误原因
        /// </summary>
        /// <returns></returns>
        public string GetError()
        {
            string lastErr = "";
            try
            {
                var ret = OCRSDK.GetError();
                if (ret != IntPtr.Zero)
                {
                    lastErr = MarshalUtf8.PtrToStringUTF8(ret);
                    //Marshal.FreeCoTaskMem(ret); 改为调用SDK的释放接口
                    OCRSDK.FreeResultBuffer(ret);
                }
            }
            catch (Exception e)
            {
                lastErr = e.Message;
            }
            return lastErr;
        }
        /// <summary>
        /// 是否生成日志，默认为true
        /// </summary>
        /// <param name="useLog"></param>
        public void EnableLog(bool useLog)
        {
            OCRSDK.EnableLog(useLog);
        }
        /// <summary>
        /// JSON输出是否使用ASCII编码，为true是返回Ascii编码，默认为false
        /// </summary>
        /// <param name="useASCII"></param>
        public void EnableASCIIResult(bool useASCII)
        {
            OCRSDK.EnableASCIIResult(useASCII);
        }
        /// <summary>
        /// 是否使用json格式返回结果，默认true
        /// </summary>
        /// <param name="enableJson"></param>
        public void EnableJsonResult(bool enableJson)
        {
            OCRSDK.EnableJsonResult(enableJson);
        }
        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T">反序列化的目标类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>反序列化后的对象</returns>
        private static T DeObject<T>(string json)
        {
            return OcrServiceHelper.DeserializeObject<T>(json);
        }
        /// <summary>
        /// 释放OCR实例
        /// </summary>
        public void FreeEngine()
        {
            OCRSDK.FreeEngine();
        }
        /// <summary>
        /// 释放并关闭版面识别引擎，释放所有相关资源
        /// </summary>
        public void FreeStructureEngine()
        {
            OCRSDK.FreeStructureEngine();
        }

        /// <summary>
        /// 以图找图：在大图中查找小图
        /// </summary>
        /// <param name="bigImagePath">大图路径</param>
        /// <param name="smallImagePath">小图路径</param>
        /// <param name="threshold">匹配阈值 [0, 1]，默认0.8。滑块找图建议0.2左右</param>
        /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
        /// <param name="useSlideMatch">是否使用滑块验证匹配（边缘检测），默认false</param>
        /// <returns>返回FindImageResult对象，包含匹配结果和位置信息</returns>
        public FindImageResult FindImage(string bigImagePath, string smallImagePath, double threshold = 0.8, bool toGray = true, bool useSlideMatch = false)
        {
            return ReadFindImageResult(() => OCRSDK.FindImage(bigImagePath, smallImagePath, threshold, toGray, useSlideMatch));
        }

        /// <summary>
        /// 以图找图：传入图片字节，在大图中查找小图
        /// </summary>
        /// <param name="bigImageBytes">大图压缩图片字节</param>
        /// <param name="smallImageBytes">小图压缩图片字节</param>
        /// <param name="threshold">匹配阈值 [0, 1]，默认0.8。滑块找图建议0.2左右</param>
        /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
        /// <param name="useSlideMatch">是否使用滑块验证匹配（边缘检测），默认false</param>
        /// <returns>返回FindImageResult对象，包含匹配结果和位置信息</returns>
        public FindImageResult FindImage(byte[] bigImageBytes, byte[] smallImageBytes, double threshold = 0.8, bool toGray = true, bool useSlideMatch = false)
        {
            if (bigImageBytes == null || bigImageBytes.Length == 0 || smallImageBytes == null || smallImageBytes.Length == 0)
            {
                return new FindImageResult
                {
                    Success = false,
                    Message = "以图找图失败：图片字节不能为空",
                    Data = null
                };
            }

            return ReadFindImageResult(() => OCRSDK.FindImageByte(
                bigImageBytes,
                new UIntPtr((ulong)bigImageBytes.LongLength),
                smallImageBytes,
                new UIntPtr((ulong)smallImageBytes.LongLength),
                threshold,
                toGray,
                useSlideMatch));
        }

        /// <summary>
        /// 以图找图：传入OpenCV Mat指针，在大图中查找小图
        /// </summary>
        /// <param name="bigImageMat">大图OpenCV Mat指针</param>
        /// <param name="smallImageMat">小图OpenCV Mat指针</param>
        /// <param name="threshold">匹配阈值 [0, 1]，默认0.8。滑块找图建议0.2左右</param>
        /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
        /// <param name="useSlideMatch">是否使用滑块验证匹配（边缘检测），默认false</param>
        /// <returns>返回FindImageResult对象，包含匹配结果和位置信息</returns>
        public FindImageResult FindImageMat(IntPtr bigImageMat, IntPtr smallImageMat, double threshold = 0.8, bool toGray = true, bool useSlideMatch = false)
        {
            if (bigImageMat == IntPtr.Zero || smallImageMat == IntPtr.Zero)
            {
                return new FindImageResult
                {
                    Success = false,
                    Message = "以图找图失败：Mat指针不能为空",
                    Data = null
                };
            }

            return ReadFindImageResult(() => OCRSDK.FindImageMat(bigImageMat, smallImageMat, threshold, toGray, useSlideMatch));
        }

        private FindImageResult ReadFindImageResult(Func<IntPtr> findImageInvoker)
        {
            IntPtr result = IntPtr.Zero;
            try
            {
                result = findImageInvoker();
                
                if (result == IntPtr.Zero)
                {
                    var lastErr = GetError();
                    return new FindImageResult
                    {
                        Success = false,
                        Message = string.IsNullOrEmpty(lastErr) ? "查找失败，返回结果为空" : "查找失败：" + lastErr,
                        Data = null
                    };
                }

                string json = MarshalUtf8.PtrToStringUTF8(result);
                
                if (string.IsNullOrEmpty(json))
                {
                    return new FindImageResult
                    {
                        Success = false,
                        Message = "查找失败，返回结果为空",
                        Data = null
                    };
                }

                return DeObject<FindImageResult>(json);
            }
            catch (Exception ex)
            {
                return new FindImageResult
                {
                    Success = false,
                    Message = $"以图找图异常: {ex.Message}",
                    Data = null
                };
            }
            finally
            {
                if (result != IntPtr.Zero)
                {
                    OCRSDK.FreeResultBuffer(result);
                }
            }
        }
    }
}
