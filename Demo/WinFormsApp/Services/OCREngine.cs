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

using PaddleOCRSDK;

namespace WinFormsApp.Services
{
    /// <summary>
    /// OCR引擎依赖注入
    /// </summary>
    public class OCREngine : IOCREngine
    {
        private static string det_infer = "ch_PP-OCRv4_det_infer";
        private static string cls_infer = "ch_ppocr_mobile_v2.0_cls_infer";
        private static string rec_infer = "ch_PP-OCRv4_rec_infer";
        //private static string det_infer = "ch_PP-OCRv4_det_server_infer";
        //private static string cls_infer = "ch_ppocr_mobile_v2.0_cls_infer";
        //private static string rec_infer = "ch_PP-OCRv4_rec_server_infer";
        private static string keys = "ppocr_keys.txt";
        private static bool use_gpu = false;
        private static bool enable_mkldnn = true;
        private static int cpu_threads = 30; //CPU预测时的线程数
        /// <summary>
        /// 初始化OCR引擎
        /// </summary>
        /// <returns></returns>
        public PaddleOCRCore GetOCREngine()
        {
            //自带轻量版中英文模型V4模型
            OCRModelConfig config = new OCRModelConfig();
            string root = CoreOCRBase.GetModelPath();
            string modelPathroot = Path.Combine(root, "model");
            config.det_infer = Path.Combine(modelPathroot, det_infer);
            config.cls_infer = Path.Combine(modelPathroot, cls_infer);
            config.rec_infer = Path.Combine(modelPathroot, rec_infer);
            config.keys = Path.Combine(modelPathroot, keys);

            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.use_gpu = use_gpu;
            oCRParameter.cpu_threads = cpu_threads;
            oCRParameter.cpu_mem = 4000;
            oCRParameter.visualize = false;
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
            oCRParameter.use_tensorrt = false;//使用GPU预测时，是否启动tensorrt，默认false

            //string ocrJson = "{\"use_gpu\": true,\"gpu_id\": 0,\"gpu_mem\": 4000,\"enable_mkldnn\": true,\"rec_img_h\": 48,\"rec_img_w\": 320,\"cls\":false,\"det\":true,\"use_angle_cls\":false}";
            //初始化通用文字引擎
            return new PaddleOCRCore(config, oCRParameter);
        }

        /// <summary>
        /// 初始化Structure引擎
        /// </summary>
        /// <returns></returns>
        public PaddleTableCore GetTableEngine()
        {
            //自带轻量版中英文模型V4模型
            TableModelConfig config = new TableModelConfig();
            string root = CoreOCRBase.GetModelPath();
            string modelPathroot = Path.Combine(root, "model");
            config.det_infer = Path.Combine(modelPathroot, det_infer);
            config.cls_infer = Path.Combine(modelPathroot, cls_infer);
            config.rec_infer = Path.Combine(modelPathroot, rec_infer);
            config.keys = Path.Combine(modelPathroot, keys);
            config.table_model_dir = Path.Combine(modelPathroot, "ch_ppstructure_mobile_v2.0_SLANet_infer");
            config.table_char_dict_path = Path.Combine(modelPathroot, "table_structure_dict_ch.txt");

            TableParameter oCRParameter = new TableParameter();
            //oCRParameter.use_gpu = use_gpu;
            oCRParameter.cpu_threads = cpu_threads;
            oCRParameter.visualize = false;
            oCRParameter.enable_mkldnn = enable_mkldnn;
            //oCRParameter.cls = true;
            //oCRParameter.det = true;
            //oCRParameter.use_angle_cls = true;
            oCRParameter.det_db_score_mode = true;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;
            oCRParameter.det_db_box_thresh = 0.618f;
            oCRParameter.table_max_len = 488;
            oCRParameter.merge_empty_cell = true;
            oCRParameter.table_batch_num = 1;
            //string ocrJson = "{\"use_gpu\": true,\"gpu_id\": 0,\"gpu_mem\": 4000,\"enable_mkldnn\": true,\"rec_img_h\": 48,\"rec_img_w\": 320,\"cls\":false,\"det\":true,\"use_angle_cls\":false}";
            //初始化表格识别引擎
            return new PaddleTableCore(config, oCRParameter);
        }
    }
}

