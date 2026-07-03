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

#pragma once
#pragma pack(push,1)
#include <vector>
using namespace std;

/// <summary>
/// PaddleOCR.dll C++识别参数,不可变更顺序
/// </summary>
struct OCRParameter
{
	// 前向相关
	bool det = true;  // 是否执行文字检测
	bool rec = true;  // 是否执行文字识别
	bool cls = false; // 是否执行文字方向分类

	// 通用参数
	bool use_gpu = false; //是否使用GPU
	int gpu_id = 0;         //GPU id，使用GPU时有效
	int gpu_mem = 4000;   //使用GPU时内存 
	bool use_tensorrt = false; //使用GPU预测时，是否启动tensorrt

	int cpu_mem = 0; //CPU内存占用上限，单位MB。-1表示不限制
	int cpu_threads = 10; //CPU预测时的线程数，在机器核数充足的情况下，该值越大，预测速度越快，默认10
	bool enable_mkldnn = true; //是否使用mkldnn库

	// 检测模型相关
	int max_side_len = 960; //输入图像长宽大于960时，等比例缩放图像，使得图像最长边为960
	float det_db_thresh = 0.3f; //用于过滤DB预测的二值化图像，设置为0.-0.3对结果影响不明显 
	float det_db_box_thresh = 0.5f; //DB后处理过滤box的阈值，如果检测存在漏框情况，可酌情减小
	float det_db_unclip_ratio = 1.6f; //表示文本框的紧致程度，越小则文本框更靠近文本
	bool use_dilation = false; //是否在输出映射上使用膨胀
	bool det_db_score_mode = true; //true:使用多边形框计算bbox score，false:使用矩形框计算。矩形框计算速度更快，多边形框对弯曲文本区域计算更准确。
	bool visualize = false; //是否对结果进行可视化，为true时，预测结果会保存在output文件夹下和输入图像同名的图像上。

	// 方向分类器相关
	bool use_angle_cls = false;//是否使用方向分类器
	float cls_thresh = 0.9f; //方向分类器的得分阈值
	int cls_batch_num = 1; //方向分类器批量识别数量

	// 识别模型相关
	int rec_batch_num = 10; //文字识别模型批量识别数量
	int rec_img_h = 48; // 识别模型输入图像高度
	int rec_img_w = 320;//识别模型输入图像宽度
	bool return_word_box = false; //是否返回单字坐标
	int ocr_instance_count = 1; // OCR引擎实例数量，默认1，最大10
};
/// <summary>
/// 版面结构识别参数（对应当前 DLL LayoutParameter,不可变更顺序）
/// </summary>
struct LayoutParameter
{

	// 基础运行参数（与 Init/Initjson 保持一致）
	bool use_gpu = false;                    // 是否使用GPU推理；false时走CPU
	int gpu_id = 0;                          // GPU设备编号（use_gpu=true时生效）
	int gpu_mem = 500;                       // GPU显存上限（MB，供推理引擎分配参考）
	bool use_tensorrt = false;               // 是否启用TensorRT加速（通常需GPU）
	int cpu_mem = 0;                         // CPU内存阈值（MB，0表示不限制）
	int cpu_threads = 30;                    // CPU推理线程数
	bool enable_mkldnn = true;               // CPU场景是否启用MKLDNN加速
	bool visualize = false;                  // 是否输出可视化结果图

	// 文档预处理参数
	bool use_doc_preprocessor = false;       // 是否启用文档预处理总开关
	bool use_doc_orientation_classify = false; // 文档方向分类开关（0/90/180/270）
	bool use_doc_unwarping = false;          // 文档去畸变/展平开关

	// 版面检测参数
	bool use_layout_detection = true;        // 是否执行版面检测；false时不产出版面框
	bool use_region_detection = false;       // 是否执行PaddleX RegionDetection大区块检测
	float layout_threshold = 0.5f;           // 版面检测全局阈值（初始化可传入）
	bool layout_nms = true;                  // 版面框后处理NMS开关
	float layout_unclip_ratio_w = 1.0f;      // 版面框宽度扩张系数
	float layout_unclip_ratio_h = 1.0f;      // 版面框高度扩张系数

	// OCR参数
	bool run_ocr_after_layout = true;        // 是否执行整图OCR（供版面块文本融合使用）
	float text_det_thresh = 0.3f;            // OCR检测阈值（越高召回越低、精度通常更高）
	float text_rec_score_thresh = 0.5f;      // OCR识别分数阈值（低于该值的文本会被过滤）
	bool use_textline_orientation = true;    // 文本行方向分类开关
	int max_side_len = 960;                  // OCR检测缩放最大长边限制

	// 条件识别参数,run_ocr_after_layout=true时才生效
	bool use_table_recognition = true;       // 是否启用表格结构识别
	bool use_table_cells_detection = false;  // 是否启用表格单元格检测和按单元格OCR    
	bool use_seal_recognition = false;       // 是否启用印章识别    
	bool use_formula_recognition = true;     // 是否启用公式识别（输出LaTeX）    
	int seal_det_limit_side_len = 736;       // 印章检测限边长
	int seal_det_limit_type = 0;             // 印章检测限边方式: 0=min, 1=max
	float seal_det_thresh = 0.2f;            // 印章检测阈值
	float seal_det_box_thresh = 0.6f;        // 印章检测框阈值
	float seal_det_unclip_ratio = 0.5f;      // 印章检测扩框系数
	float seal_rec_score_thresh = 0.0f;      // 印章识别分数阈值

	// 输出参数
	bool format_block_content = false;       // block_content是否按可读格式输出（如Markdown片段）
	bool output_markdown = true;            // 是否在DetectLayout返回的JSON中附带markdown字段
};
/// <summary>
/// OCR动态修改参数
/// </summary>
struct SyncParameter
{
	bool d_det = true;//动态修改是否检测
	bool d_rec = true; //输入图像长宽
	float d_det_db_box_thresh = 0.5f; // 表示文本框的紧致程度
	float d_det_db_unclip_ratio = 1.6f;//表示文本框的紧致程度，越小则文本框更靠近文本
	int d_max_side_len = 960;//输入图像长宽
	float d_det_db_thresh = 0.3f; // DB后处理过滤box的阈值
};
//UVDoc参数结构体
struct UVDocParameter {
	bool enable_mkldnn; // CPU模式下，是否使用mkldnn库
	int cpu_threads;    // CPU模式下，预测时的线程数
	bool use_gpu;       // 是否使用GPU
	int gpu_id;         // GPU id
	int gpu_mem;        // GPU内存
	bool use_tensorrt;  // 是否启动tensorrt
	int uvdoc_instance_count = 1; // UVDoc引擎实例数量
};
#pragma pack(pop)  
