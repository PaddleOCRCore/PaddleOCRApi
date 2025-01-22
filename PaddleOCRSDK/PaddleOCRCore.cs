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

using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System;
using System.IO;

namespace PaddleOCRSDK
{
    /// <summary>
    /// PaddleOCR识别引擎对象
    /// </summary>
    public class PaddleOCRCore : CoreOCRBase
    {
        #region SDK API

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool Init(string det_infer, string cls_infer, string rec_infer, string keys, OCRParameter parameter);
        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool Initjson(string det_infer, string cls_infer, string rec_infer, string keys, string parameterjson);

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr Detect(string imagefile);

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectByte(byte[] imagebytedata, long size);

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectBase64(string imagebase64);

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern int FreeEngine();

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool DynamicInit(SyncParameter parameter);
  
        #endregion

        #region 文本识别
        /// <summary>
        /// 使用默认参数初始化OCR引擎对象
        /// </summary>
        public PaddleOCRCore() : this(null,new OCRParameter())
        {
        }
        /// <summary>
        /// 使用默认参数初始化OCR引擎对象
        /// </summary>
        /// <param name="config">模型配置对象，如果为空则按默认值</param>
        public PaddleOCRCore(OCRModelConfig config) : this(config, new OCRParameter())
        {
        }
        /// <summary>
        /// PaddleOCR识别引擎对象初始化
        /// </summary>
        /// <param name="config">模型配置对象，如果为空则按默认值</param>
        /// <param name="parameter">识别参数，为空均按缺省值</param>
        public PaddleOCRCore(OCRModelConfig config, OCRParameter parameter) : base()
        {
           
            if (parameter == null) parameter = new OCRParameter();
            if (config == null)
            {
                string root = GetModelPath().TrimEnd('\\');
                config = new OCRModelConfig();
                string modelPathroot = Path.Combine(root, "inference");
                config.det_infer = Path.Combine(modelPathroot, "ch_PP-OCRv4_det_infer");
                config.cls_infer = Path.Combine(modelPathroot, "ch_ppocr_mobile_v2.0_cls_infer");
                config.rec_infer = Path.Combine(modelPathroot, "ch_PP-OCRv4_rec_infer");
                config.keys = Path.Combine(modelPathroot, "ppocr_keys.txt");
            }
            if (!Directory.Exists(config.det_infer) && parameter.det) throw new DirectoryNotFoundException(config.det_infer);
            if (!Directory.Exists(config.cls_infer) && parameter.cls) throw new DirectoryNotFoundException(config.cls_infer);
            if (!Directory.Exists(config.rec_infer)) throw new DirectoryNotFoundException(config.rec_infer);
            if (!File.Exists(config.keys)) throw new FileNotFoundException(config.keys);
            bool sucess = Init(config.det_infer, config.cls_infer, config.rec_infer, config.keys, parameter);
            if(!sucess)
            {
                throw  new Exception("Initialize err:" + GetLastError());
            }
        }
        /// <summary>
        /// PaddleOCR识别引擎对象初始化
        /// </summary>
        /// <param name="config">模型配置对象，如果为空则按默认值</param>
        /// <param name="parameterjson">识别参数json字符串</param>
        public PaddleOCRCore(OCRModelConfig config, string parameterjson) : base()
        {
            if (config == null)
            {
                string root = GetModelPath().TrimEnd('\\');
                config = new OCRModelConfig();
                string modelPathroot = Path.Combine(root, "model");
                config.det_infer = Path.Combine(modelPathroot, "ch_PP-OCRv4_det_infer");
                config.cls_infer = Path.Combine(modelPathroot, "ch_ppocr_mobile_v2.0_cls_infer");
                config.rec_infer = Path.Combine(modelPathroot, "ch_PP-OCRv4_rec_infer");
                config.keys = Path.Combine(modelPathroot, "ppocr_keys.txt");
            }
            if (string.IsNullOrEmpty(parameterjson))
            {
                parameterjson = GetModelPath().TrimEnd('\\');
                parameterjson += @"\model\PaddleOCR.config.json";
                if (!File.Exists(parameterjson)) throw new FileNotFoundException(parameterjson);
                parameterjson = File.ReadAllText(parameterjson);
            }
            var parameter = JsonHelper.DeObject<OCRParameter>(parameterjson);

            if (!Directory.Exists(config.det_infer) && parameter.det) throw new DirectoryNotFoundException(config.det_infer);
            if (!Directory.Exists(config.cls_infer) && parameter.cls) throw new DirectoryNotFoundException(config.cls_infer);
            if (!Directory.Exists(config.rec_infer)) throw new DirectoryNotFoundException(config.rec_infer);
            if (!File.Exists(config.keys)) throw new FileNotFoundException(config.keys);
            parameter = null;
            bool sucess = Initjson(config.det_infer, config.cls_infer, config.rec_infer, config.keys, parameterjson);
            if (!sucess)
            {
                throw new Exception("Init err:" + GetLastError());
            }
        }
        /// <summary>
        /// 对图像文件进行文本识别
        /// </summary>
        /// <param name="imagefile">图像文件</param>
        /// <returns>OCR识别结果</returns>
        public OCRResult DetectText(string imagefile)
        {
            if (!File.Exists(imagefile)) throw new Exception($"文件{imagefile}不存在");
            var ptrResult = Detect(imagefile);
            return ConvertResult(ptrResult);
        }


 
        /// <summary>
        ///对图像对象进行文本识别
        /// </summary>
        /// <param name="image">图像</param>
        /// <returns>OCR识别结果</returns>
        public OCRResult Detect(Image image)
        {
            if (image == null) throw new ArgumentNullException("image");
            var imagebyte = ImageToBytes(image);
            var result = DetectText(imagebyte);
            imagebyte = null;
            return result;
        }
      
        /// <summary>
        ///文本识别
        /// </summary>
        /// <param name="imagebyte">图像内存流</param>
        /// <returns>OCR识别结果</returns>
        public OCRResult DetectText(byte[] imagebyte)
        {
            if (imagebyte == null) throw new ArgumentNullException("imagebyte");
            var ptrResult = DetectByte(imagebyte, imagebyte.LongLength);
            return ConvertResult(ptrResult);
        }

        /// <summary>
        ///文本识别Base64图片
        /// </summary>
        /// <param name="imagebase64">图像base64</param>
        /// <returns>OCR识别结果</returns>
        public OCRResult DetectTextBase64(string imagebase64)
        {
            if (imagebase64 == null || imagebase64 == "") throw new ArgumentNullException("imagebase64");
            IntPtr ptrResult = DetectBase64(imagebase64);
            return ConvertResult(ptrResult);
        }

        /// <summary>
        /// Json结果处理
        /// </summary>
        /// <param name="ptrResult"></param>
        /// <returns></returns>
        private OCRResult ConvertResult(IntPtr ptrResult)
        {
            OCRResult result = new OCRResult();
            if (ptrResult == IntPtr.Zero)
            {
                var err = GetLastError();
                if (!string.IsNullOrEmpty(err))
                {
                    throw new Exception("内部遇到错误：" + err);
                }
                return result;
            }
            string json = string.Empty;
            try
            {
                json = Marshal.PtrToStringUni(ptrResult);
                List<TextArea> textBlocks = JsonHelper.DeObject<List<TextArea>>(json);
                result.TextBlocks = textBlocks;
                result.JsonText = json;
                Marshal.FreeHGlobal(ptrResult);
            }
            catch (Exception ex)
            {
                throw new Exception("OCR结果Json反序列化失败:"+ json, ex);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 在初始化后动态修改参数
        /// </summary>
        /// <param name="parameter">可修改参数对象</param>
        /// <returns>是否成功，在初始化前调用会导致失败</returns>
        public bool DynamicParameter(SyncParameter parameter)
        {
            return DynamicInit(parameter);
        }
       
        #region Dispose
        /// <summary>
        /// 释放对象
        /// </summary>
        public override void Dispose()
        {
            FreeEngine();
        }
        #endregion
    }
}