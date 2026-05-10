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
using PaddleOCRSDK;

namespace WinFormsApp.Services
{
    /// <summary>
    /// OCR引擎依赖注入
    /// </summary>
    public class OCREngine
    {
        private static readonly Lazy<IOCRService> _ocrService = new Lazy<IOCRService>(() => new OCRService());
        public static IOCRService ocrService => _ocrService.Value;
        public static string det_infer = "PP-OCRv5_mobile_det_infer";//OCR检测模型
        public static string rec_infer = "PP-OCRv5_mobile_rec_infer";//OCR识别模型
        public static string cls_infer = "PP-LCNet_x1_0_textline_ori";//文本方向分类模块
        public static string doc_cls_infer = "PP-LCNet_x1_0_doc_ori_infer";//文档图像方向分类模块
        public static string layout_model_dir = "PP-DocLayoutV3_infer";//版面识别模型inference
        public static string table_model_dir = "PP-SLANet_plus_infer";//表格识别模型inference
        public static string formula_model_dir = "LaTeX_OCR_rec_infer";//公式识别模型
        public static string seal_model_dir = "PP-OCRv4_mobile_seal_det_infer";//印章检测模型  
        public static string doc_unwarp_model = "UVDoc_infer";//文档矫正模型
        public static string region_model_dir = "PP-DocBlockLayout_infer";//区域检测模型
        private static bool enable_mkldnn = true;
        public static int cpu_threads = 30; //CPU预测时的线程数
        private static bool visualize = true;//是否对结果进行可视化，为true时，预测结果会保存在output文件夹下和输入图像同名的文件上。

        public static bool use_gpu = false;//是否使用GPU
        public static int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        public static int gpu_id = 0;//GPUId
        public static int gpu_mem = 4000;//GPU显存上限
        public static bool use_cls = true;//是否执行文字方向分类
        public static bool use_det = true;//是否文本检测模型
        public static bool use_angle_cls = true;//是否使用方向分类器
        public static bool return_word_box = false;//是否返回单字坐标
        public static bool use_tensorrt = false;//使用GPU预测时，是否启动tensorrt

        /// <summary>
        /// 初始化OCR引擎
        /// </summary>
        /// <returns></returns>
        public static string GetOCREngine()
        {
            InitParamater para = new InitParamater();
            para.det_infer = $"models/{det_infer}";
            para.cls_infer = $"models/{cls_infer}";
            para.rec_infer = $"models/{ rec_infer}";

            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.use_gpu = use_gpu;
            if(use_gpu)
                oCRParameter.use_tensorrt = use_tensorrt;
            oCRParameter.gpu_id = gpu_id;
            oCRParameter.gpu_mem = gpu_mem;
            oCRParameter.cpu_mem = cpu_mem;
            oCRParameter.cpu_threads = cpu_threads;//提升CPU速度，优化此参数
            oCRParameter.enable_mkldnn = enable_mkldnn;
            oCRParameter.rec_batch_num = 6;
            oCRParameter.cls = use_cls;
            oCRParameter.det = use_det;
            oCRParameter.use_angle_cls = use_angle_cls;
            oCRParameter.det_db_score_mode = false;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;//用于过滤DB预测的二值化图像，设置为0.-0.3对结果影响不明显 
            oCRParameter.det_db_box_thresh = 0.5f;//DB后处理过滤box的阈值，如果检测存在漏框情况，可酌情减小
            oCRParameter.det_db_unclip_ratio = 1.6f;//表示文本框的紧致程度，越小则文本框更靠近文本
            oCRParameter.visualize = visualize;
            oCRParameter.return_word_box = return_word_box;

            para.ocrpara = oCRParameter;
            para.paraType = EnumParaType.Class;
            //string ocrJson = "{\"use_gpu\": true,\"gpu_id\": 0,\"gpu_mem\": 4000,\"enable_mkldnn\": true,\"rec_img_h\": 48,\"rec_img_w\": 320,\"cls\":false,\"det\":true,\"use_angle_cls\":false}";
            //初始化通用文字引擎
            string msg = "文本识别初始化成功";
            try
            {
                ocrService.EnableLog(true);//关闭Log日志
                ocrService.Init(para);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 初始化版面识别引擎默认V5模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath"></param>
        /// <returns></returns>
        public static string GetOCRStructureEngine()
        {
            InitParamater para = new InitParamater();
            string root = AppDomain.CurrentDomain.BaseDirectory;
            string modelsPath = Path.Combine(root, "models");//存放模型的目录，不允许修改

            para.det_infer = $"models/{det_infer}";
            para.cls_infer = $"models/{cls_infer}";
            para.doc_cls_infer = $"models/{doc_cls_infer}";
            para.rec_infer = $"models/{rec_infer}";
            para.layout_model_dir = $"models/{layout_model_dir}";
            para.table_model_dir = $"models/{table_model_dir}";
            para.formula_model_dir = $"models/{formula_model_dir}";
            para.doc_unwarp_model = $"models/{doc_unwarp_model}";
            para.region_model_dir = $"models/{region_model_dir}";
            para.seal_model_dir = $"models/{seal_model_dir}";

            LayoutParameter oCRParameter = new LayoutParameter();
            oCRParameter.use_gpu = use_gpu;
            oCRParameter.use_tensorrt = use_tensorrt;
            oCRParameter.gpu_id = gpu_id;
            oCRParameter.gpu_mem = gpu_mem;
            oCRParameter.cpu_mem = cpu_mem;
            oCRParameter.cpu_threads = cpu_threads;
            oCRParameter.enable_mkldnn = enable_mkldnn;
            oCRParameter.visualize = visualize;

            oCRParameter.use_doc_preprocessor = true;
            oCRParameter.use_doc_orientation_classify = true;
            oCRParameter.use_doc_unwarping = false;

            oCRParameter.use_layout_detection = true;
            oCRParameter.use_region_detection = false;
            oCRParameter.layout_nms = true;
            oCRParameter.layout_unclip_ratio_w = 1.0f;
            oCRParameter.layout_unclip_ratio_h = 1.0f;

            oCRParameter.run_ocr_after_layout = true;
            oCRParameter.text_det_thresh = 0.3f;
            oCRParameter.text_rec_score_thresh = 0.5f;
            oCRParameter.use_textline_orientation = use_cls;
            oCRParameter.max_side_len = 960;

            oCRParameter.use_table_recognition = true;
            oCRParameter.use_seal_recognition = true;
            oCRParameter.use_formula_recognition = true;

            oCRParameter.format_block_content = false;
            oCRParameter.output_markdown = true;
            para.layoutpara = oCRParameter;
            para.paraType = EnumParaType.StructureClass;
            string msg = "版面识别初始化成功";
            try
            {
                ocrService.Init(para);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 释放OCR引擎
        /// </summary>
        public static void FreeEngine()
        {
            try
            {
                ocrService.FreeEngine();
                ocrService.FreeStructureEngine();
            }
            catch (Exception)
            {
            }
        }
    }
}

