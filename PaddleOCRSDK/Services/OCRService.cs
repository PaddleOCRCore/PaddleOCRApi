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
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace PaddleOCRSDK
{
    public class OCRException : Exception
    {
        public OCRException(string message) : base(message)
        {
        }
    }
    public class OCRService:IOCRService
    {
        /// <summary>
        /// 初始化OCR引擎默认V4模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath"></param>
        /// <returns></returns>
        public string InitDefaultOCREngine(string modelsPath)
        {
            string det_infer = "ch_PP-OCRv4_det_infer";//OCR检测模型
            string rec_infer = "ch_PP-OCRv4_rec_infer";//OCR识别模型
            string cls_infer = "ch_ppocr_mobile_v2.0_cls_infer";
            string keys = "ppocr_keys.txt";
            bool use_gpu = false;//是否使用GPU
            int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
            int gpu_id = 0;//GPUId
            bool enable_mkldnn = true;
            int cpu_threads = 30; //CPU预测时的线程数
            InitParamater para = new InitParamater();
            para.det_infer = Path.Combine(modelsPath, det_infer);
            para.cls_infer = Path.Combine(modelsPath, cls_infer);
            para.rec_infer = Path.Combine(modelsPath, rec_infer);
            para.keyFile = Path.Combine(modelsPath, keys);

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
            string det_infer = "ch_PP-OCRv4_det_infer";//OCR检测模型
            string rec_infer = "ch_PP-OCRv4_rec_infer";//OCR识别模型
            string keys = "ppocr_keys.txt";
            string table_model_dir = "ch_ppstructure_mobile_v2.0_SLANet_infer";//表格识别模型inference
            string table_dict_path = "table_structure_dict_ch.txt";//表格识别字典文件
            bool use_gpu = false;//是否使用GPU
            int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
            int gpu_id = 0;//GPUId
            bool enable_mkldnn = true;
            int cpu_threads = 30; //CPU预测时的线程数
            InitParamater para = new InitParamater();
            para.det_infer = Path.Combine(modelsPath, det_infer);
            para.rec_infer = Path.Combine(modelsPath, rec_infer);
            para.keyFile = Path.Combine(modelsPath, keys);
            para.table_model_dir = Path.Combine(modelsPath, table_model_dir);
            para.table_dict_path = Path.Combine(modelsPath, table_dict_path);
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
                    ret = OCRSDK.Init(para.det_infer, para.cls_infer, para.rec_infer, para.keyFile, para.ocrpara);
                }
                else if (para.paraType == EnumParaType.Json)
                {
                    ret = OCRSDK.Initjson(para.det_infer, para.cls_infer, para.rec_infer, para.keyFile, para.json);
                }
                else if (para.paraType == EnumParaType.TableClass)
                {
                    ret = OCRSDK.InitTable(para.det_infer, para.rec_infer, para.keyFile, para.table_model_dir,para.table_dict_path, para.tablepara);
                }
                else if (para.paraType == EnumParaType.TableJson)
                {
                    ret = OCRSDK.InitTablejson(para.det_infer, para.rec_infer, para.keyFile, para.table_model_dir, para.table_dict_path, para.json);
                }
                else
                {
                    throw new OCRException("不支持的参数类型");
                }

                if (!ret)
                {
                    var error = GetError();
                    throw new OCRException($"初始化失败: {error}");
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new OCRException($"初始化失败: {ex.Message}");
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
                    throw new OCRException("OCR内部错误：" + lastErr);
                }
                return result;
            }
            string json = string.Empty;
            try
            {
                json = Marshal.PtrToStringUni(ptrResult);
                List<JsonResult> jonResult = DeObject<List<JsonResult>>(json);
                result.WordsResult = jonResult;
                result.JsonText = json;
            }
            catch (Exception ex)
            {
                throw new OCRException("OCR结果Json反序列化失败:" + ex.Message);
            }            
            finally
            {
                if (ptrResult != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrResult);
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
                result = Marshal.PtrToStringUni(ptrResult);
            }
            catch (Exception ex)
            {
                throw new OCRException("OCR表格识别失败:" + ex.Message);
            }
            finally
            {
                if (ptrResult != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrResult);
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
                    lastErr = Marshal.PtrToStringAnsi(ret);
                    Marshal.FreeHGlobal(ret);
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
        /// 是否使用单字节编码，默认为false
        /// </summary>
        /// <param name="useANSI"></param>
        public void EnableANSIResult(bool useANSI)
        {
            OCRSDK.EnableANSIResult(useANSI);
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
    }
}
