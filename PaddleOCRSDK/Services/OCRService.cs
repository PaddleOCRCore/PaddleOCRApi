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
using System.Data.Common;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
            catch (Exception)
            {
                throw new OCRException($"初始化失败: {GetError()}");
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
