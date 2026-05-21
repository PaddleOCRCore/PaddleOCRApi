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

using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.InteropServices;

namespace OCRCoreService
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionBase : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OResult"></param>
        /// <returns></returns>
        protected ObjectResult OKResult(object OResult)
        {
            ApiResult result = new ApiResult();
            result.Status = HttpStatusCode.OK;
            result.Data = OResult;
            result.ErrorMessage = "";
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OResult"></param>
        /// <returns></returns>
        protected ObjectResult BadResult(object OResult)
        {
            ApiResult result = new ApiResult();
            result.Status = HttpStatusCode.BadRequest;
            result.Data = "";
            result.ErrorMessage = OResult.ToString();
            return Ok(result);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestIdOcr
    {
        /// <summary>
        /// 图片Base64字符串
        /// </summary>
        public string Base64String { get; set; }
        /// <summary>
        /// front：身份证含照片的一面；back：身份证带国徽的一面。
        /// </summary>
        public string id_card_side { get; set; } = "front";
    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestOcr
    {
        /// <summary>
        /// 图片Base64字符串
        /// </summary>
        public string Base64String { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public string ResultType { get; set; } = "text";
    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestOcrByte
    {
        /// <summary>
        /// 图片byte[]字节码
        /// </summary>
        public byte[] ImageByte { get; set; } = Array.Empty<byte>();
        /// <summary>
        /// 返回类型
        /// </summary>
        public string ResultType { get; set; } = "text";
    }

    /// <summary>
    /// 版面识别请求参数
    /// </summary>
    public class RequestLayoutOcr
    {
        /// <summary>
        /// 图片Base64字符串
        /// </summary>
        public string Base64String { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OCRConfig
    {
        /// <summary>
        /// det_infer模型路径
        /// </summary>
        public string det_infer { get; set; }
        /// <summary>
        /// 文本行方向分类模型路径
        /// </summary>
        public string cls_infer { get; set; }
        /// <summary>
        /// rec_infer模型路径
        /// </summary>
        public string rec_infer { get; set; }
        /// <summary>
        /// 授权文件路径，支持绝对路径或相对程序目录路径
        /// </summary>
        public string OCRLicense { get; set; } = @"models\paddleocr.lic";
        /// <summary>
        /// 是否使用GPU
        /// </summary>
        public bool use_gpu { get; set; } = false;
        /// <summary>
        /// 是否使用方向分类器
        /// </summary>
        public bool use_cls { get; set; } = true;
        /// <summary>
        /// 是否使用mkldnn库
        /// </summary>
        public bool enable_mkldnn { get; set; } = true;
        /// <summary>
        /// 使用GPU预测时，是否启动tensorrt
        /// </summary>
        public bool use_tensorrt { get; set; } = false;
        /// <summary>
        /// GPU id，使用GPU时有效
        /// </summary>
        public int gpu_id { get; set; } = 0;
        /// <summary>
        /// 使用GPU时内存
        /// </summary>
        public int gpu_mem { get; set; } = 4000;
        /// <summary>
        /// CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        /// </summary>
        public int cpu_mem { get; set; } = 0;
        /// <summary>
        /// CPU预测时的线程数，在机器核数充足的情况下，该值越大，预测速度越快
        /// </summary>
        public int cpu_threads { get; set; } = 30;
        /// <summary>
        /// 文字识别模型批量识别数量
        /// </summary>
        public int rec_batch_num { get; set; } = 7;
        /// <summary>
        /// 是否用启日志
        /// </summary>
        public bool enableLog { get; set; } = false;
        /// <summary>
        /// 是否启用单字坐标
        /// </summary>
        public bool return_word_box { get; set; } = false;
        /// <summary>
        /// OCR引擎实例数量，默认1，最大10，适用于高并发时使用。
        /// </summary>
        public int ocr_instance_count { get; set; } = 1;
    }

    /// <summary>
    /// 版面识别配置
    /// </summary>
    public class LayoutConfig
    {
        /// <summary>
        /// det_infer模型路径
        /// </summary>
        public string det_infer { get; set; } = "PP-OCRv5_mobile_det_infer";
        /// <summary>
        /// 文本行方向分类模型路径
        /// </summary>
        public string cls_infer { get; set; } = "PP-LCNet_x1_0_textline_ori";
        /// <summary>
        /// 文档方向分类模型路径
        /// </summary>
        public string doc_cls_infer { get; set; } = "PP-LCNet_x1_0_doc_ori_infer";
        /// <summary>
        /// rec_infer模型路径
        /// </summary>
        public string rec_infer { get; set; } = "PP-OCRv5_mobile_rec_infer";
        /// <summary>
        /// 版面识别模型inference model地址
        /// </summary>
        public string layout_model_dir { get; set; } = "PP-DocLayoutV3_infer";
        /// <summary>
        /// 表格识别模型inference model地址
        /// </summary>
        public string table_model_dir { get; set; } = "PP-SLANet_plus_infer";
        /// <summary>
        /// 公式识别模型路径
        /// </summary>
        public string formula_model_dir { get; set; } = "LaTeX_OCR_rec_infer";
        /// <summary>
        /// 印章检测模型路径
        /// </summary>
        public string seal_model_dir { get; set; } = "PP-OCRv4_mobile_seal_det_infer";
        /// <summary>
        /// 文档图像矫正模型路径
        /// </summary>
        public string doc_unwarp_model { get; set; } = "UVDoc_infer";
        /// <summary>
        /// 文档图像版面子模块检测模型路径
        /// </summary>
        public string region_model_dir { get; set; } = "PP-DocBlockLayout";
        /// <summary>
        /// 是否使用GPU
        /// </summary>
        public bool use_gpu { get; set; } = false;
        /// <summary>
        /// 使用GPU预测时，是否启动tensorrt
        /// </summary>
        public bool use_tensorrt { get; set; } = false;
        /// <summary>
        /// GPU id，使用GPU时有效
        /// </summary>
        public int gpu_id { get; set; } = 0;
        /// <summary>
        /// 使用GPU时内存
        /// </summary>
        public int gpu_mem { get; set; } = 4000;
        /// <summary>
        /// CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        /// </summary>
        public int cpu_mem { get; set; } = 0;
        /// <summary>
        /// CPU预测时的线程数
        /// </summary>
        public int cpu_threads { get; set; } = 30;
        /// <summary>
        /// 是否使用mkldnn库
        /// </summary>
        public bool enable_mkldnn { get; set; } = true;
        /// <summary>
        /// 是否对结果进行可视化
        /// </summary>
        public bool visualize { get; set; } = false;
        /// <summary>
        /// 是否启用文档预处理
        /// </summary>
        public bool use_doc_preprocessor { get; set; } = true;
        /// <summary>
        /// 是否启用文档方向分类
        /// </summary>
        public bool use_doc_orientation_classify { get; set; } = true;
        /// <summary>
        /// 是否启用文档图像矫正
        /// </summary>
        public bool use_doc_unwarping { get; set; } = false;
        /// <summary>
        /// 是否启用版面检测
        /// </summary>
        public bool use_layout_detection { get; set; } = true;
        /// <summary>
        /// 是否启用文档图像版面子模块检测
        /// </summary>
        public bool use_region_detection { get; set; } = false;
        /// <summary>
        /// 是否启用版面NMS
        /// </summary>
        public bool layout_nms { get; set; } = true;
        /// <summary>
        /// 版面检测框宽度扩张比例
        /// </summary>
        public float layout_unclip_ratio_w { get; set; } = 1.0f;
        /// <summary>
        /// 版面检测框高度扩张比例
        /// </summary>
        public float layout_unclip_ratio_h { get; set; } = 1.0f;
        /// <summary>
        /// 是否在版面识别后运行OCR
        /// </summary>
        public bool run_ocr_after_layout { get; set; } = true;
        /// <summary>
        /// 文本检测阈值
        /// </summary>
        public float text_det_thresh { get; set; } = 0.3f;
        /// <summary>
        /// 文本识别得分阈值
        /// </summary>
        public float text_rec_score_thresh { get; set; } = 0.5f;
        /// <summary>
        /// 是否启用文本行方向分类
        /// </summary>
        public bool use_textline_orientation { get; set; } = true;
        /// <summary>
        /// 输入图像最长边
        /// </summary>
        public int max_side_len { get; set; } = 960;
        /// <summary>
        /// 是否启用表格识别
        /// </summary>
        public bool use_table_recognition { get; set; } = true;
        /// <summary>
        /// 是否启用印章识别
        /// </summary>
        public bool use_seal_recognition { get; set; } = true;
        /// <summary>
        /// 是否启用公式识别
        /// </summary>
        public bool use_formula_recognition { get; set; } = true;
        /// <summary>
        /// 是否格式化块内容
        /// </summary>
        public bool format_block_content { get; set; } = false;
        /// <summary>
        /// 是否输出markdown
        /// </summary>
        public bool output_markdown { get; set; } = true;
    }

    /// <summary>
    /// UVDoc文档矫正配置
    /// </summary>
    public class UVDocConfig
    {
        /// <summary>
        /// 是否启用UVDoc文档矫正服务
        /// </summary>
        public bool enabled { get; set; } = false;

        /// <summary>
        /// UVDoc模型路径
        /// </summary>
        public string uvdoc_infer { get; set; } = "UVDoc_infer";

        /// <summary>
        /// CPU模式下，是否使用mkldnn库
        /// </summary>
        public bool enable_mkldnn { get; set; } = true;

        /// <summary>
        /// CPU模式下，预测时的线程数
        /// </summary>
        public int cpu_threads { get; set; } = 10;

        /// <summary>
        /// 是否使用GPU
        /// </summary>
        public bool use_gpu { get; set; } = false;

        /// <summary>
        /// GPU id，使用GPU时有效
        /// </summary>
        public int gpu_id { get; set; } = 0;

        /// <summary>
        /// 使用GPU时内存
        /// </summary>
        public int gpu_mem { get; set; } = 2000;

        /// <summary>
        /// 使用GPU预测时，是否启动tensorrt
        /// </summary>
        public bool use_tensorrt { get; set; } = false;
    }

    /// <summary>
    /// OCR-VL 视觉语言识别服务配置
    /// </summary>
    public class OCRVLConfig
    {
        /// <summary>
        /// 是否启用OCR-VL服务
        /// </summary>
        public bool enabled { get; set; } = false;

        /// <summary>
        /// yaml配置文件路径
        /// </summary>
        public string yaml_path { get; set; } = "configs/PaddleOCR-VL-1.5.yaml";
    }

    /// <summary>
    /// API认证配置
    /// </summary>
    public class ApiAuthConfig
    {
        /// <summary>
        /// 是否启用API认证鉴权
        /// </summary>
        public bool enabled { get; set; } = false;

        /// <summary>
        /// API Key请求头名称
        /// </summary>
        public string header_name { get; set; } = "PaddleOCR-Api-Key";

        /// <summary>
        /// API Key密钥
        /// </summary>
        public string api_key { get; set; } = "";
    }
}
