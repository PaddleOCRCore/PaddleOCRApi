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
using System;

namespace OCRCoreService.Services
{
    /// <summary>
    /// OCR引擎依赖注入
    /// </summary>
    public class OCREngine
    {
        private readonly IOCRService _ocrService;
        private readonly OCRConfig _ocrConfig;
        private readonly LayoutConfig _layoutConfig;
        private readonly object _structureEngineLock = new object();
        private bool _structureEngineInitialized;
        public IOCRService OcrService => _ocrService;

        public OCREngine(IOCRService ocrService, OCRConfig ocrConfig, LayoutConfig layoutConfig)
        {
            _ocrService = ocrService;
            _ocrConfig = ocrConfig;
            _layoutConfig = layoutConfig;
            GetOCREngine();
        }
        /// <summary>
        /// 初始化OCR引擎
        /// </summary>
        /// <returns></returns>
        public string GetOCREngine()
        {
            //自带轻量版中英文模型V5模型
            InitParamater para=new InitParamater();
            //改为相对路径，避免中文路径问题
            para.det_infer = $"models/{_ocrConfig.det_infer}";
            para.cls_infer = $"models/{_ocrConfig.cls_infer}";
            para.rec_infer = $"models/{_ocrConfig.rec_infer}";
            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.use_gpu = _ocrConfig.use_gpu;
            oCRParameter.use_tensorrt = false;
            oCRParameter.gpu_id = _ocrConfig.gpu_id;
            oCRParameter.gpu_mem = 4000;
            oCRParameter.cpu_mem = _ocrConfig.cpu_mem;
            oCRParameter.cpu_threads = _ocrConfig.cpu_threads;//提升CPU速度，优化此参数
            oCRParameter.enable_mkldnn = _ocrConfig.enable_mkldnn;
            oCRParameter.rec_batch_num = _ocrConfig.rec_batch_num;
            oCRParameter.det = true;
            oCRParameter.cls = _ocrConfig.use_cls;
            oCRParameter.use_angle_cls = _ocrConfig.use_cls;
            oCRParameter.det_db_score_mode = true;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;
            oCRParameter.det_db_box_thresh = 0.618f;
            oCRParameter.visualize = false;
            oCRParameter.return_word_box = _ocrConfig.return_word_box;
            oCRParameter.ocr_instance_count = _ocrConfig.ocr_instance_count;

            para.ocrpara = oCRParameter;
            para.paraType = EnumParaType.Class;
            //string ocrJson = "{\"use_gpu\": true,\"gpu_id\": 0,\"gpu_mem\": 4000,\"enable_mkldnn\": true,\"rec_img_h\": 48,\"rec_img_w\": 320,\"cls\":false,\"det\":true,\"use_angle_cls\":false}";
            //初始化通用文字引擎
            string msg = "";
            try
            {
                _ocrService.EnableLog(_ocrConfig.enableLog);
                _ocrService.Init(para);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 初始化表格识别引擎默认V5模型，使用CPU及mkldnn
        /// Api接口未使用，若使用可将此加到OCREngine中
        /// </summary>
        /// <param name="modelsPath"></param>
        /// <returns></returns>
        public string GetOCRTableEngine()
        {
            lock (_structureEngineLock)
            {
                if (_structureEngineInitialized)
                {
                    return "初始化成功";
                }

                InitParamater para = new InitParamater();

                //改为相对路径，避免中文路径问题
                para.det_infer = $"models/{_layoutConfig.det_infer}";
                para.cls_infer = $"models/{_layoutConfig.cls_infer}";
                para.doc_cls_infer = $"models/{_layoutConfig.doc_cls_infer}";
                para.rec_infer = $"models/{_layoutConfig.rec_infer}";
                para.layout_model_dir = $"models/{_layoutConfig.layout_model_dir}";
                para.table_model_dir = $"models/{_layoutConfig.table_model_dir}";
                para.formula_model_dir = $"models/{_layoutConfig.formula_model_dir}";
                para.doc_unwarp_model = $"models/{_layoutConfig.doc_unwarp_model}";
                para.region_model_dir = $"models/{_layoutConfig.region_model_dir}";
                para.seal_model_dir = $"models/{_layoutConfig.seal_model_dir}";
                LayoutParameter oCRParameter = new LayoutParameter();
                oCRParameter.use_gpu = _layoutConfig.use_gpu;
                oCRParameter.use_tensorrt = _layoutConfig.use_tensorrt;
                oCRParameter.gpu_id = _layoutConfig.gpu_id;
                oCRParameter.gpu_mem = _layoutConfig.gpu_mem;
                oCRParameter.cpu_mem = _layoutConfig.cpu_mem;
                oCRParameter.cpu_threads = _layoutConfig.cpu_threads;
                oCRParameter.enable_mkldnn = _layoutConfig.enable_mkldnn;
                oCRParameter.visualize = _layoutConfig.visualize;

                oCRParameter.use_doc_preprocessor = _layoutConfig.use_doc_preprocessor;
                oCRParameter.use_doc_orientation_classify = _layoutConfig.use_doc_orientation_classify;
                oCRParameter.use_doc_unwarping = _layoutConfig.use_doc_unwarping;

                oCRParameter.use_layout_detection = _layoutConfig.use_layout_detection;
                oCRParameter.use_region_detection = _layoutConfig.use_region_detection;
                oCRParameter.layout_nms = _layoutConfig.layout_nms;
                oCRParameter.layout_unclip_ratio_w = _layoutConfig.layout_unclip_ratio_w;
                oCRParameter.layout_unclip_ratio_h = _layoutConfig.layout_unclip_ratio_h;

                oCRParameter.run_ocr_after_layout = _layoutConfig.run_ocr_after_layout;
                oCRParameter.text_det_thresh = _layoutConfig.text_det_thresh;
                oCRParameter.text_rec_score_thresh = _layoutConfig.text_rec_score_thresh;
                oCRParameter.use_textline_orientation = _layoutConfig.use_textline_orientation;
                oCRParameter.max_side_len = _layoutConfig.max_side_len;

                oCRParameter.use_table_recognition = _layoutConfig.use_table_recognition;
                oCRParameter.use_seal_recognition = _layoutConfig.use_seal_recognition;
                oCRParameter.use_formula_recognition = _layoutConfig.use_formula_recognition;

                oCRParameter.format_block_content = _layoutConfig.format_block_content;
                oCRParameter.output_markdown = _layoutConfig.output_markdown;

                para.layoutpara = oCRParameter;
                para.paraType = EnumParaType.StructureClass;
                string msg = "初始化成功";
                try
                {
                    _ocrService.EnableLog(_ocrConfig.enableLog);
                    _ocrService.Init(para);
                    _structureEngineInitialized = true;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }

                return msg;
            }
        }

        /// <summary>
        /// 确保版面识别引擎已初始化
        /// </summary>
        /// <returns>成功返回空字符串，失败返回错误信息</returns>
        public string EnsureStructureEngine()
        {
            string message = GetOCRTableEngine();
            if (string.IsNullOrWhiteSpace(message) || message.Contains("初始化成功", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return message;
        }
    }
}
