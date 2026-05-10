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
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRSDK
{
    public enum EnumParaType
    {
        /// <summary>
        /// 实体参数类型
        /// </summary>
        Class,
        /// <summary>
        /// Json类型
        /// </summary>
        Json,
        /// <summary>
        /// 版面识别实体参数类型
        /// </summary>
        StructureClass,
        /// <summary>
        /// 版面识别Json类型
        /// </summary>
        StructureJson,
    }
    public class InitParamater
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
        /// 文档方向分类模型路径
        /// </summary>
        public string doc_cls_infer { get; set; }
        /// <summary>
        /// rec_infer模型路径
        /// </summary>
        public string rec_infer { get; set; }
        /// <summary>
        /// 版面识别模型inference model地址
        /// </summary>
        public string layout_model_dir { get; set; }
        /// <summary>
        /// 表格识别模型inference model地址
        /// </summary>
        public string table_model_dir { get; set; }
        /// <summary>
        /// 公式识别模型路径（可选）
        /// </summary>
        public string formula_model_dir { get; set; }
        /// <summary>
        /// 印章识别模型路径（可选）
        /// </summary>
        public string seal_model_dir { get; set; }
        /// <summary>
        /// 文档图像矫正模型路径（可选）
        /// </summary>
        public string doc_unwarp_model { get; set; }
        /// <summary>
        /// 文档图像版面子模块检测模型路径（可选）
        /// </summary>
        public string region_model_dir { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public EnumParaType paraType { get; set; }
        /// <summary>
        /// 参数列表
        /// </summary>
        public OCRParameter ocrpara { get; set; }
        /// <summary>
        /// 参数列表
        /// </summary>
        public LayoutParameter layoutpara { get; set; }
        /// <summary>
        /// json字符串
        /// </summary>
        public string json { get; set; }
    }
    /// <summary>
    /// OCR识别参数，OCRParameter类的属性定义顺序不可随便更改，与PdddleOCR.dll传入参数保持一致
    /// 参数调用参考github文档https://github.com/PaddlePaddle/PaddleOCR/blob/main/deploy/cpp_infer/readme_ch.md
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class OCRParameter
    {
        #region 文字识别设置
        /// <summary>
        ///是否执行文字检测
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool det { get; set; } = true;
        /// <summary>
        /// 是否执行文字识别
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool rec { get; set; } = true;
        /// <summary>
        /// 是否执行文字方向分类
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool cls { get; set; } = false;
        #endregion

        #region 通用参数设置
        /// <summary>
        /// 是否使用GPU
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_gpu { get; set; } = false;
        /// <summary>
        /// GPU id，使用GPU时有效
        /// </summary>
        [field: MarshalAs(UnmanagedType.I4)]
        public int gpu_id { get; set; } = 0;
        /// <summary>
        /// 使用GPU时内存
        /// </summary>
        [field: MarshalAs(UnmanagedType.I4)]
        public int gpu_mem { get; set; } = 4000;
        /// <summary>
        /// 使用GPU预测时，是否启动tensorrt
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_tensorrt { get; set; } = false;
        /// <summary>
        /// CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        /// </summary>
        [field: MarshalAs(UnmanagedType.I4)]
        public int cpu_mem { get; set; } = 2000;
        /// <summary>
        /// CPU预测时的线程数，在机器核数充足的情况下，该值越大，预测速度越快，默认30
        /// </summary>
        public int cpu_threads { get; set; } = 30;
        /// <summary>
        /// 是否使用mkldnn库
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool enable_mkldnn { get; set; } = true;
        #endregion

        #region 检测模型设置
        /// <summary>
        /// 输入图像长宽大于960时，等比例缩放图像，使得图像最长边为960
        /// </summary>
        public int max_side_len { get; set; } = 960;
        /// <summary>
        /// 用于过滤DB预测的二值化图像，设置为0.-0.3对结果影响不明显 
        /// </summary>
        public float det_db_thresh { get; set; } = 0.3f;
        /// <summary>
        /// DB后处理过滤box的阈值，如果检测存在漏框情况，可酌情减小
        /// </summary>
        public float det_db_box_thresh { get; set; } = 0.5f;
        /// <summary>
        /// 表示文本框的紧致程度，越小则文本框更靠近文本
        /// </summary>
        public float det_db_unclip_ratio { get; set; } = 1.3f;
        /// <summary>
        /// 是否在输出映射上使用膨胀
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_dilation { get; set; } = false;
        /// <summary>
        /// false:使用多边形框计算bbox score，false:使用矩形框计算。矩形框计算速度更快，多边形框对弯曲文本区域计算更准确。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool det_db_score_mode { get; set; } = false;
        /// <summary>
        /// 是否对结果进行可视化，为true时，预测结果会保存在output文件夹下和输入图像同名的文件上。
        /// </summary>

        [field: MarshalAs(UnmanagedType.I1)]
        public bool visualize { get; set; } = false;

        #endregion

        #region 方向分类器设置

        /// <summary>
        /// 是否使用方向分类器
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_angle_cls { get; set; } = false;
        /// <summary>
        /// 方向分类器的得分阈值
        /// </summary>
        public float cls_thresh { get; set; } = 0.9f;
        /// <summary>
        /// 方向分类器批量识别数量
        /// </summary>
        public int cls_batch_num { get; set; } = 1;
        #endregion

        #region 文字识别模型设置
        /// <summary>
        /// 文字识别模型批量识别数量
        /// </summary>
        public int rec_batch_num { get; set; } = 6;
        /// <summary>
        /// 识别模型输入图像高度
        /// </summary>
        public int rec_img_h { get; set; } = 48;
        /// <summary>
        /// 识别模型输入图像宽度
        /// </summary>
        public int rec_img_w { get; set; } = 320;
        #endregion
        /// <summary>
        /// 是否返回单字坐标，默认false，开启后json中会增加WordList字段
        /// </summary>

        [field: MarshalAs(UnmanagedType.I1)]
        public bool return_word_box { get; set; } = false;
        /// <summary>
        /// OCR引擎实例数量，默认1，最大10
        /// </summary>
        public int ocr_instance_count { get; set; } = 1;
    }
    /// <summary>
    /// OCR版面结构识别模型参数（对应C++ LayoutParameter，字段顺序必须一致）
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class LayoutParameter
    {
        #region 基础运行参数
        /// <summary>
        /// 是否使用GPU推理；false时使用CPU。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_gpu { get; set; } = false;
        /// <summary>
        /// GPU设备编号，use_gpu为true时生效。
        /// </summary>
        public int gpu_id { get; set; } = 0;
        /// <summary>
        /// GPU显存上限，单位MB，供推理引擎分配参考。
        /// </summary>
        public int gpu_mem { get; set; } = 4000;
        /// <summary>
        /// 是否启用TensorRT加速，通常需要GPU。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_tensorrt { get; set; } = false;
        /// <summary>
        /// CPU内存阈值，单位MB，0表示不限制。
        /// </summary>
        public int cpu_mem { get; set; } = 0;
        /// <summary>
        /// CPU推理线程数。
        /// </summary>
        public int cpu_threads { get; set; } = 30;
        /// <summary>
        /// CPU场景是否启用MKLDNN加速。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool enable_mkldnn { get; set; } = true;
        /// <summary>
        /// 是否输出可视化结果图。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool visualize { get; set; } = false;
        #endregion

        #region 文档预处理参数
        /// <summary>
        /// 是否启用文档预处理总开关。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_doc_preprocessor { get; set; } = true;
        /// <summary>
        /// 是否启用文档方向分类，支持0/90/180/270度方向。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_doc_orientation_classify { get; set; } = true;
        /// <summary>
        /// 是否启用文档去畸变或展平。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_doc_unwarping { get; set; } = false;
        #endregion

        #region 版面检测参数
        /// <summary>
        /// 是否执行版面检测；false时不产出版面框。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_layout_detection { get; set; } = true;
        /// <summary>
        /// 是否执行PaddleX RegionDetection大区块检测。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_region_detection { get; set; } = false;
        /// <summary>
        /// 版面检测全局阈值。
        /// </summary>
        public float layout_threshold { get; set; } = 0.5f;
        /// <summary>
        /// 版面框后处理NMS开关。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool layout_nms { get; set; } = true;
        /// <summary>
        /// 版面框宽度扩张系数。
        /// </summary>
        public float layout_unclip_ratio_w { get; set; } = 1.0f;
        /// <summary>
        /// 版面框高度扩张系数。
        /// </summary>
        public float layout_unclip_ratio_h { get; set; } = 1.0f;
        #endregion

        #region OCR参数
        /// <summary>
        /// 是否执行整图OCR，供版面块文本融合使用。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool run_ocr_after_layout { get; set; } = true;
        /// <summary>
        /// OCR检测阈值，越高召回越低，精度通常更高。
        /// </summary>
        public float text_det_thresh { get; set; } = 0.3f;
        /// <summary>
        /// OCR识别分数阈值，低于该值的文本会被过滤。
        /// </summary>
        public float text_rec_score_thresh { get; set; } = 0.5f;
        /// <summary>
        /// 文本行方向分类开关。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_textline_orientation { get; set; } = true;
        /// <summary>
        /// OCR检测缩放最大长边限制。
        /// </summary>
        public int max_side_len { get; set; } = 960;
        #endregion

        #region 条件识别参数
        /// <summary>
        /// 是否启用表格结构识别，run_ocr_after_layout为true时生效。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_table_recognition { get; set; } = true;
        /// <summary>
        /// 是否启用印章识别，run_ocr_after_layout为true时生效。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_seal_recognition { get; set; } = false;
        /// <summary>
        /// 是否启用公式识别并输出LaTeX，run_ocr_after_layout为true时生效。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_formula_recognition { get; set; } = true;
        /// <summary>
        /// 印章检测限边长。
        /// </summary>
        public int seal_det_limit_side_len { get; set; } = 736;
        /// <summary>
        /// 印章检测限边方式：0=min，1=max。
        /// </summary>
        public int seal_det_limit_type { get; set; } = 0;
        /// <summary>
        /// 印章检测阈值。
        /// </summary>
        public float seal_det_thresh { get; set; } = 0.2f;
        /// <summary>
        /// 印章检测框阈值。
        /// </summary>
        public float seal_det_box_thresh { get; set; } = 0.6f;
        /// <summary>
        /// 印章检测扩框系数。
        /// </summary>
        public float seal_det_unclip_ratio { get; set; } = 0.5f;
        /// <summary>
        /// 印章识别分数阈值。
        /// </summary>
        public float seal_rec_score_thresh { get; set; } = 0.0f;
        #endregion

        #region 输出参数
        /// <summary>
        /// block_content是否按可读格式输出，例如Markdown片段。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool format_block_content { get; set; } = false;
        /// <summary>
        /// 是否在DetectLayout返回的JSON中附带markdown字段。
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool output_markdown { get; set; } = true;
        #endregion
    }
    /// <summary>
    /// OCR动态可修改参数
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SyncParameter
    {
        /// <summary>
        ///动态修改是否检测。在OCRParameter.det=true时，d_det可动态关闭参数det
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool d_det { get; set; } = true;
        /// <summary>
        ///动态修改是否识别。在OCRParameter.rec=true时，d_rec可动态关闭参数rec
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool d_rec { get; set; } = true;
        /// <summary>
        /// DB后处理过滤box的阈值，如果检测存在漏框情况，可酌情减小；默认0.5。当d_det=true时有效
        /// </summary>
        public float d_det_db_box_thresh { get; set; } = 0.5f;
        /// <summary>
        /// 表示文本框的紧致程度，越小则文本框更靠近文本
        /// </summary>
        public float d_det_db_unclip_ratio { get; set; } = 1.6f;
        /// <summary>
        /// 输入图像长宽大于960时，等比例缩放图像，使得图像最长边为960,；默认960
        /// </summary>
        public int d_max_side_len { get; set; } = 960;
        /// <summary>
        /// 用于过滤DB预测的二值化图像，设置为0.-0.3对结果影响不明显；默认0.3。当d_det=true时有效
        /// </summary>
        public float d_det_db_thresh { get; set; } = 0.3f;

    }

}
