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

using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using PaddleOCRSDK;

namespace OCRCoreService.Services
{
    /// <summary>
    /// OCR引擎依赖注入
    /// </summary>
    public class OCREngine
    {
        private readonly IOCRService _ocrService;
        public IOCRService OcrService => _ocrService;

        public OCREngine(IOCRService ocrService)
        {
            _ocrService = ocrService;
            GetOCREngine();
        }
        private static string det_infer = "ch_PP-OCRv4_det_infer";//OCR检测模型
        private static string rec_infer = "ch_PP-OCRv4_rec_infer";//OCR识别模型
        private static string cls_infer = "ch_ppocr_mobile_v2.0_cls_infer";
        private static string keys = "ppocr_keys.txt";
        private static string table_model_dir = "ch_ppstructure_mobile_v2.0_SLANet_infer";//表格识别模型inference
        private static string table_dict_path = "table_structure_dict_ch.txt";//表格识别字典文件
        public static bool use_gpu = false;//是否使用GPU
        public static int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        public static int gpu_id = 0;//GPUId
        private static bool enable_mkldnn = true;
        public static int cpu_threads = 30; //CPU预测时的线程数
        /// <summary>
        /// 初始化OCR引擎
        /// </summary>
        /// <returns></returns>
        public string GetOCREngine()
        {
            //自带轻量版中英文模型V4模型
            InitParamater para=new InitParamater();
            string root = AppDomain.CurrentDomain.BaseDirectory;
            string modelPathroot = Path.Combine(root, "models");
            para.det_infer = Path.Combine(modelPathroot, det_infer);
            para.cls_infer = Path.Combine(modelPathroot, cls_infer);
            para.rec_infer = Path.Combine(modelPathroot, rec_infer);
            para.keyFile = Path.Combine(modelPathroot, keys);

            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.use_gpu = use_gpu;
            oCRParameter.use_tensorrt = true;
            oCRParameter.gpu_id = gpu_id;
            oCRParameter.gpu_mem = 4000;
            oCRParameter.cpu_mem = cpu_mem;
            oCRParameter.cpu_threads = cpu_threads;//提升CPU速度，优化此参数
            oCRParameter.enable_mkldnn = enable_mkldnn;
            oCRParameter.rec_batch_num = 6;
            oCRParameter.cls = false;
            oCRParameter.det = true;
            oCRParameter.use_angle_cls = false;
            oCRParameter.det_db_score_mode = true;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;
            oCRParameter.det_db_box_thresh = 0.618f;
            oCRParameter.visualize = false;

            para.ocrpara = oCRParameter;
            para.paraType = EnumParaType.Class;
            //string ocrJson = "{\"use_gpu\": true,\"gpu_id\": 0,\"gpu_mem\": 4000,\"enable_mkldnn\": true,\"rec_img_h\": 48,\"rec_img_w\": 320,\"cls\":false,\"det\":true,\"use_angle_cls\":false}";
            //初始化通用文字引擎
            string msg = "";
            try
            {
                _ocrService.Init(para);
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
        public string GetOCRTableEngine(string modelsPath)
        {
            InitParamater para = new InitParamater();
            string root = AppDomain.CurrentDomain.BaseDirectory;
            string modelPathroot = Path.Combine(root, "models");//存放模型的目录，不允许修改
            para.det_infer = Path.Combine(modelPathroot, det_infer);
            para.rec_infer = Path.Combine(modelPathroot, rec_infer);
            para.keyFile = Path.Combine(modelPathroot, keys);
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
            oCRParameter.rec_batch_num = 6;
            oCRParameter.cls = false;
            oCRParameter.det = true;
            oCRParameter.use_angle_cls = false;
            oCRParameter.det_db_score_mode = true;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;
            oCRParameter.det_db_box_thresh = 0.618f;
            oCRParameter.visualize = false;
            para.ocrpara = oCRParameter;
            para.paraType = EnumParaType.TableClass;
            string msg = "初始化成功";
            try
            {
                _ocrService.Init(para);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
    }
}
