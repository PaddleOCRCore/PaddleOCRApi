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
using PaddleOCRSDK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRSDK
{
    /// <summary>
    /// 跨版本 Marshal.UTF8 工具类
    /// .NET Framework 没有 PtrToStringUTF8，这里统一提供。
    /// </summary>
    public static class MarshalUtf8
    {
        /// <summary>
        /// 将 UTF-8 零结尾字节序列转换为托管字符串。
        /// 支持 .NET Framework 2.0+ / .NET Core 1.x+ / .NET 5+
        /// </summary>
        public static string PtrToStringUTF8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;

            // .NET 5+ 原生支持 PtrToStringUTF8
#if NET5_0_OR_GREATER
        return Marshal.PtrToStringUTF8(ptr);
#else
            return PtrToStringUTF8_Manual(ptr);
#endif
        }

#if !NET5_0_OR_GREATER
        /// <summary>
        /// .NET Framework / Core 的手动实现
        /// </summary>
        private static string PtrToStringUTF8_Manual(IntPtr ptr)
        {
            // 1. 计算长度（到零字节为止）
            int len = 0;
            while (Marshal.ReadByte(ptr, len) != 0) len++;

            // 2. 复制到托管数组
            byte[] bytes = new byte[len];
            Marshal.Copy(ptr, bytes, 0, len);

            // 3. UTF-8 解码
            return Encoding.UTF8.GetString(bytes);
        }
#endif
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
        /// <param name="modelsPath"></param>
        /// <returns></returns>
        public string InitDefaultOCREngine(string modelsPath)
        {
            string det_infer = "PP-OCRv5_mobile_det_infer";//OCR检测模型
            string rec_infer = "PP-OCRv5_mobile_rec_infer";//OCR识别模型
            string cls_infer = "PP-LCNet_x1_0_textline_ori";
            bool use_gpu = false;//是否使用GPU
            int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
            int gpu_id = 0;//GPUId
            bool enable_mkldnn = true;
            int cpu_threads = 30; //CPU预测时的线程数
            InitParamater para = new InitParamater();
            para.det_infer = Path.Combine(modelsPath, det_infer);
            para.cls_infer = Path.Combine(modelsPath, cls_infer);
            para.rec_infer = Path.Combine(modelsPath, rec_infer);

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
            oCRParameter.det_db_score_mode = true;
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
        /// 初始化表格识别引擎默认V4模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath"></param>
        /// <returns></returns>
        public string InitDefaultTableEngine(string modelsPath)
        {
            string det_infer = "PP-OCRv4_mobile_det_infer";//OCR检测模型
            string rec_infer = "PP-OCRv4_mobile_rec_infer";//OCR识别模型
            string cls_infer = "PP-LCNet_x1_0_textline_ori";//表格分类模块模型
            string table_model_dir = "PP-SLANet_plus_infer";//表格识别模型inference
            bool use_gpu = false;//是否使用GPU
            int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
            int gpu_id = 0;//GPUId
            bool enable_mkldnn = true;
            int cpu_threads = 30; //CPU预测时的线程数
            InitParamater para = new InitParamater();
            para.det_infer = Path.Combine(modelsPath, det_infer);
            para.rec_infer = Path.Combine(modelsPath, rec_infer);
            para.cls_infer = Path.Combine(modelsPath, cls_infer);
            para.table_model_dir = Path.Combine(modelsPath, table_model_dir);
            TableParameter oCRParameter = new TableParameter();
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
            oCRParameter.det_db_score_mode = true;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;
            oCRParameter.det_db_box_thresh = 0.618f;
            oCRParameter.visualize = true;
            para.tablepara = oCRParameter;
            para.paraType = EnumParaType.TableClass;
            string msg = "表格识别初始化成功";
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
                else if (para.paraType == EnumParaType.TableClass)
                {
                    ret = OCRSDK.InitTable(para.det_infer, para.cls_infer, para.rec_infer, para.table_model_dir,para.tablepara);
                }
                else if (para.paraType == EnumParaType.TableJson)
                {
                    ret = OCRSDK.InitTablejson(para.det_infer, para.cls_infer, para.rec_infer, para.table_model_dir, para.json);
                }
                else
                {
                    throw new OCRException("不支持的参数类型");
                }

                if (!ret)
                {
                    var error = GetError();
                    throw new OCRException($"初始化失败:{error}");
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
                throw new OCRException($"初始化失败:{ex.Message}");
            }
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
            var ptrResult = OCRSDK.DetectByte(imagebyte, imagebyte.LongLength);
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
                        List<JsonResult> jonResult = DeObject<List<JsonResult>>(json);
                        result.WordsResult = jonResult;
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
        /// 对图像文件进行表格识别
        /// </summary>
        /// <param name="imagefile">图像文件</param>
        /// <returns>OCR识别结果</returns>
        public string DetectTable(string imagefile)
        {
            var ptrResult = OCRSDK.DetectTable(imagefile);
            return GetTableResult(ptrResult);
        }
        /// <summary>
        /// 对图像文件进行表格识别
        /// </summary>
        /// <param name="imagebyte">图像文件</param>
        /// <returns>OCR识别结果</returns>
        public string DetectTableByte(byte[] imagebyte)
        {
            var ptrResult = OCRSDK.DetectByte(imagebyte, imagebyte.LongLength);
            return GetTableResult(ptrResult);
        }
        /// <summary>
        /// 对图像文件进行表格识别
        /// </summary>
        /// <param name="imagebyte">图像文件</param>
        /// <returns>OCR识别结果</returns>
        public string DetectTableBase64(string base64)
        {
            var ptrResult = OCRSDK.DetectBase64(base64);
            return GetTableResult(ptrResult);
        }

        private string GetTableResult(IntPtr ptrResult)
        {
            string result = string.Empty;
            if (ptrResult == IntPtr.Zero)
            {
                var lastErr = GetError();
                if (!string.IsNullOrEmpty(lastErr))
                {
                    throw new OCRException("OCR内部错误：" + lastErr);
                }
                return result;
            }
            try
            {
                result = MarshalUtf8.PtrToStringUTF8(ptrResult);
            }
            catch (Exception ex)
            {
                throw new OCRException("OCR表格识别失败:" + ex.Message);
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
            if (string.IsNullOrEmpty(json)) return default(T);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
            return (T)JsonConvert.DeserializeObject(json, typeof(T), settings);
        }
        /// <summary>
        /// 释放OCR实例
        /// </summary>
        public void FreeEngine()
        {
            OCRSDK.FreeEngine();
        }
        /// <summary>
        /// 释放OCR表格识别实例
        /// </summary>
        public void FreeTableEngine()
        {
            OCRSDK.FreeTableEngine();
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
            IntPtr result = IntPtr.Zero;
            try
            {
                result = OCRSDK.FindImage(bigImagePath, smallImagePath, threshold, toGray, useSlideMatch);
                
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
