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
        public static string cls_infer = "PP-LCNet_x1_0_textline_ori";
        private static string table_model_dir = "PP-SLANet_plus_infer";//表格识别模型inference
        private static bool enable_mkldnn = true;
        public static int cpu_threads = 30; //CPU预测时的线程数
        private static bool visualize = true;//是否对结果进行可视化，为true时，预测结果会保存在output文件夹下和输入图像同名的文件上。

        public static bool use_gpu = false;//是否使用GPU
        public static int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        public static int gpu_id = 0;//GPUId
        public static int gpu_mem = 4000;//GPU显存上限
        public static bool use_cls = true;//是否执行文字方向分类
        public static bool use_angle_cls = true;//是否使用方向分类器

        /// <summary>
        /// 初始化OCR引擎
        /// </summary>
        /// <returns></returns>
        public static string GetOCREngine()
        {
            InitParamater para = new InitParamater();
            //string root = AppDomain.CurrentDomain.BaseDirectory;
            //string modelsPath = Path.Combine(root, "models");//存放模型的目录，不允许修改
            //para.det_infer = Path.Combine(modelsPath, det_infer);
            //para.cls_infer = Path.Combine(modelsPath, cls_infer);
            //para.rec_infer = Path.Combine(modelsPath, rec_infer);
            //para.keyFile = Path.Combine(modelsPath, keys);
            //改为相对路径，避免中文路径问题
            para.det_infer = $"models/{det_infer}";
            para.cls_infer = $"models/{cls_infer}";
            para.rec_infer = $"models/{ rec_infer}";

            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.use_gpu = use_gpu;
            oCRParameter.use_tensorrt = false;
            oCRParameter.gpu_id = gpu_id;
            oCRParameter.gpu_mem = gpu_mem;
            oCRParameter.cpu_mem = cpu_mem;
            oCRParameter.cpu_threads = cpu_threads;//提升CPU速度，优化此参数
            oCRParameter.enable_mkldnn = enable_mkldnn;
            oCRParameter.rec_batch_num = 6;
            oCRParameter.cls = use_cls;
            oCRParameter.det = true;
            oCRParameter.use_angle_cls = use_angle_cls;
            oCRParameter.det_db_score_mode = true;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;//用于过滤DB预测的二值化图像，设置为0.-0.3对结果影响不明显 
            oCRParameter.det_db_box_thresh = 0.3f;//DB后处理过滤box的阈值，如果检测存在漏框情况，可酌情减小
            oCRParameter.det_db_unclip_ratio = 1.6f;//表示文本框的紧致程度，越小则文本框更靠近文本
            oCRParameter.visualize = visualize;

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
        /// 初始化表格识别引擎默认V5模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath"></param>
        /// <returns></returns>
        public static string GetOCRTableEngine()
        {
            InitParamater para = new InitParamater();
            string root = AppDomain.CurrentDomain.BaseDirectory;
            string modelsPath = Path.Combine(root, "models");//存放模型的目录，不允许修改
            //para.det_infer = Path.Combine(modelsPath, det_infer);
            //para.cls_infer = Path.Combine(modelsPath, cls_infer);
            //para.rec_infer = Path.Combine(modelsPath, rec_infer);
            //para.keyFile = Path.Combine(modelsPath, keys);
            //para.table_model_dir = Path.Combine(modelsPath, table_model_dir);
            //para.table_dict_path = Path.Combine(modelsPath, table_dict_path);

            para.det_infer = $"models/{det_infer}";
            para.cls_infer = $"models/{cls_infer}";
            para.rec_infer = $"models/{rec_infer}";
            para.table_model_dir = $"models/{table_model_dir}";

            TableParameter oCRParameter = new TableParameter();
            oCRParameter.use_gpu = use_gpu;
            oCRParameter.use_tensorrt = false;
            oCRParameter.gpu_id = gpu_id;
            oCRParameter.gpu_mem = gpu_mem;
            oCRParameter.cpu_mem = cpu_mem;
            oCRParameter.cpu_threads = cpu_threads;//提升CPU速度，优化此参数
            oCRParameter.enable_mkldnn = enable_mkldnn;
            oCRParameter.rec_batch_num = 6;
            oCRParameter.cls = use_cls;
            oCRParameter.det = true;
            oCRParameter.use_angle_cls = use_angle_cls;
            oCRParameter.det_db_score_mode = true;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;
            oCRParameter.det_db_box_thresh = 0.618f;
            oCRParameter.visualize = visualize;
            para.tablepara = oCRParameter;
            para.paraType = EnumParaType.TableClass;
            string msg = "表格识别初始化成功";
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
                ocrService.FreeTableEngine();
            }
            catch (Exception)
            {
            }
        }
    }
}

