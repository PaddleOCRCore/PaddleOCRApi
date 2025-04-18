﻿// Copyright (c) 2025 PaddleOCRCore All Rights Reserved.
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
/// PaddleOCR.dll C++识别参数
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
};
/// <summary>
/// 表格识别参数
/// </summary>
struct TableParameter :OCRParameter
{
	int table_max_len = 488;//输入图像长宽大于488时，等比例缩放图像,默认488
	bool merge_empty_cell = true; //是否合并空单元格
	int table_batch_num = 1; //批量识别数量
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
#pragma pack(pop)  
