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

using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.IO;

namespace PaddleOCRSDK
{
    /// <summary>
    /// PaddleTableCore帮助类
    /// </summary>
    public class PaddleTableCore:CoreOCRBase
    {
        #region PaddleOCR API
       
        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool InitTable(string det_infer, string rec_infer, string keys, string table_model_dir, string table_char_dict_path, TableParameter parameter);
        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool InitTablejson(string det_infer, string rec_infer, string keys, string table_model_dir, string table_char_dict_path, string parameter);

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectTable(string imagefile);
       
        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectTableByte(byte[] imagebytedata, long size);

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectTableBase64(string imagebase64);
       
        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void FreeTableEngine( );
        #endregion

        /// <summary>
        /// PaddleTableCore识别引擎对象初始化
        /// </summary>
        public PaddleTableCore() : this(null,new TableParameter())
        {  
        }
        /// <summary>
        /// PaddleTableCore识别引擎对象初始化
        /// </summary>
        /// <param name="config">模型配置对象，如果为空则按默认值</param>
        public PaddleTableCore(TableModelConfig config) : this(config, new TableParameter())
        {
        }
        /// <summary>
        /// PaddleTableCore初始化
        /// </summary>
        /// <param name="config">模型配置对象</param>
        /// <param name="para">识别参数</param>
        public PaddleTableCore(TableModelConfig config, TableParameter para) : base()
        {
            if (para == null) para = new TableParameter();
            if (config == null)
            {
                string root = GetModelPath();
                config = new TableModelConfig();
                string modelPathroot = root + @"\model";

                config.det_infer = modelPathroot + @"\ch_PP-OCRv4_det_infer";
                config.rec_infer = modelPathroot + @"\ch_PP-OCRv4_rec_infer";
                config.keys = modelPathroot + @"\ppocr_keys.txt";
                config.table_model_dir = modelPathroot + @"\ch_ppstructure_mobile_v2.0_SLANet_infer";
                config.table_char_dict_path = modelPathroot + @"\table_structure_dict_ch.txt";
            }
             bool sucess = InitTable(config.det_infer,  config.rec_infer, config.keys, config.table_model_dir, config.table_char_dict_path, para);
            if (!sucess) throw new Exception("Initialize err:" + GetLastError());
        }
        /// <summary>
        /// PaddleTableCore初始化
        /// </summary>
        /// <param name="config">模型对象设置</param>
        /// <param name="parajson">识别参数Json格式</param>
        public PaddleTableCore(TableModelConfig config, string parajson) : base()
        {
            if (config == null)
            {
                string root = GetModelPath();
                config = new TableModelConfig();
                string modelPathroot = root + @"\model";
                config.det_infer = modelPathroot + @"\ch_PP-OCRv4_det_infer";
                config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
                config.rec_infer = modelPathroot + @"\ch_PP-OCRv4_rec_infer";
                config.keys = modelPathroot + @"\ppocr_keys.txt";
                config.table_model_dir = modelPathroot + @"\ch_ppstructure_mobile_v2.0_SLANet_infer";
                config.table_char_dict_path = modelPathroot + @"\table_structure_dict_ch.txt";
            }
            if (string.IsNullOrEmpty(parajson))
            {
                parajson = GetModelPath();
                parajson += @"\model\PaddleOCRTable.config.json";
                if (!File.Exists(parajson)) throw new FileNotFoundException(parajson);
                parajson = File.ReadAllText(parajson);
            }
            bool sucess = InitTablejson(config.det_infer, config.rec_infer, config.keys, config.table_model_dir, config.table_char_dict_path, parajson);
            if (!sucess) throw new Exception("Initialize err:" + GetLastError());
        }
        /// <summary>
        /// 对图像文件进行表格文本识别
        /// </summary>
        /// <param name="imagefile">图像文件</param>
        /// <returns>表格识别结果</returns>
        public string TableDetectFile(string imagefile)
        {
            if (!System.IO.File.Exists(imagefile)) throw new Exception($"文件{imagefile}不存在");
            IntPtr presult = DetectTable( imagefile);
            return ConvertResult(presult);
        }



        /// <summary>
        ///对图像对象进行表格文本识别
        /// </summary>
        /// <param name="image">图像</param>
        /// <returns>表格识别结果</returns>
        public string TableDetect(Image image)
        {
            if (image == null) throw new ArgumentNullException("image");
            var imagebyte = ImageToBytes(image);
            var result = TableDetect(imagebyte);
            imagebyte = null;
            return result;
        }

        /// <summary>
        /// 对图像Byte数组进行表格文本识别
        /// </summary>
        /// <param name="imagebyte">图像字节数组</param>
        /// <returns>表格识别结果</returns>
        public string TableDetect(byte[] imagebyte)
        {
           if (imagebyte == null) throw new ArgumentNullException("imagebyte");
            IntPtr presult= DetectTableByte(imagebyte, imagebyte.LongLength); 
            return ConvertResult(presult);
        }
        /// <summary>
        /// 对图像Base64进行表格文本识别
        /// </summary>
        /// <param name="base64">图像Base64</param>
        /// <returns>表格识别结果</returns>
        public string TableDetectBase64(string base64)
        {
            if (base64 == null || base64 == "") throw new ArgumentNullException("imagebase64");
            IntPtr presult= DetectTableBase64( base64); 
            return ConvertResult(presult);
        }
        /// <summary>
         /// 结果解析
         /// </summary>
         /// <param name="ptrResult"></param>
         /// <returns></returns>
        private string ConvertResult(IntPtr ptrResult)
        {
            string result = "";
            if (ptrResult == IntPtr.Zero)
            {
                var err = GetLastError();
                if (!string.IsNullOrEmpty(err))
                {
                    throw new Exception("内部遇到错误：" + err);
                }
                return result;
            }
            try
            {
                result = Marshal.PtrToStringUni(ptrResult);
                if (!OperatingSystem.IsWindows())
                {
                    result = Marshal.PtrToStringAnsi(ptrResult);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("表格识别失败", ex);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrResult);
            }
            return result;
        }


        #region Dispose
        /// <summary>
        /// 释放对象
        /// </summary>
        public override void Dispose()
        {
            FreeTableEngine();
        }
        #endregion
    }
}
