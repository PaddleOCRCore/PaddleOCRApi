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

namespace OCRCoreService.Services
{
    /// <summary>
    /// OCR引擎依赖注入
    /// </summary>
    public class OCREngine
    {
        private readonly IOCRService _ocrService;
        private readonly OCRConfig _ocrConfig;
        public IOCRService OcrService => _ocrService;

        public OCREngine(IOCRService ocrService, OCRConfig ocrConfig)
        {
            _ocrService = ocrService;
            _ocrConfig = ocrConfig;
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
            //string root = AppDomain.CurrentDomain.BaseDirectory;
            string root = AppContext.BaseDirectory;
            string modelPathroot = Path.Combine(root, "models");
            para.det_infer = Path.Combine(modelPathroot, _ocrConfig.det_infer);
            para.cls_infer = Path.Combine(modelPathroot, _ocrConfig.cls_infer);
            para.rec_infer = Path.Combine(modelPathroot, _ocrConfig.rec_infer);
            para.keyFile = Path.Combine(modelPathroot, _ocrConfig.keyFile);

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
            InitParamater para = new InitParamater();
            //string root = AppDomain.CurrentDomain.BaseDirectory;
            string root = AppContext.BaseDirectory;
            string modelsPath = Path.Combine(root, "models");//存放模型的目录，不允许修改
            para.det_infer = Path.Combine(modelsPath, _ocrConfig.det_infer);
            para.cls_infer = Path.Combine(modelsPath, _ocrConfig.cls_infer);
            para.rec_infer = Path.Combine(modelsPath, _ocrConfig.rec_infer);
            para.keyFile = Path.Combine(modelsPath, _ocrConfig.keyFile);
            para.table_model_dir = Path.Combine(modelsPath, _ocrConfig.table_model_dir);
            para.table_dict_path = Path.Combine(modelsPath, _ocrConfig.table_dict_path);
            TableParameter oCRParameter = new TableParameter();
            oCRParameter.use_gpu = _ocrConfig.use_gpu;
            oCRParameter.use_tensorrt = false;
            oCRParameter.gpu_id = _ocrConfig.gpu_id;
            oCRParameter.gpu_mem = _ocrConfig.gpu_mem;
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
            para.ocrpara = oCRParameter;
            para.paraType = EnumParaType.TableClass;
            string msg = "初始化成功";
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
    }
}
